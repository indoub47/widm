using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSheetsActions;
using Google.Apis.Sheets.v4;
using RecordValidation;
using TagMaking;
using WidmShared;
using Reporting;
using InspectionValidation;
using InspPoolCommunication;
using DbMappings;
using InspectionLib;
using System.IO;

namespace Widm
{
    class Program
    {
        private enum Actions { ValidateOnly, UpdateFaultless, ForceUpdate };

        static void Main(string[] args)
        {
            Actions action = Actions.ValidateOnly;            

            // loggers
            LogWriter logger = LogWriterFactory.Create(LogFile.ProcessLog);            

            // GoogleSheets config
            SpreadSheetConfig[] spreadsheets = SpreadSheetConfig.GetConfig();
            Dictionary<string, SheetConfig> sheetData = SheetConfig.GetConfig();

            // common objects
            SheetsService service = new GSheetsConnector().Connect();
            GSRecordFetcher recFetcher = new GSRecordFetcher();
            PlainTxtReporter1 reporter = new PlainTxtReporter1();
            OleDbInspPoolCommunicator inspPoolCommunicator = 
                new OleDbInspPoolCommunicator(Properties.Settings.Default.DbPath);
            InspTagMaker inspTagMaker = new InspTagMaker();
            GSheetsUpdater gsUpdater = new GSheetsUpdater();


            // pirmieji specific objects
            SheetConfig pSheetConf = sheetData["pirmieji"];
            PirmRecValidator pRecValidator = new PirmRecValidator(
                pSheetConf.Mapping, new PirmRecTagMaker(pSheetConf.Mapping));
            GSRecBatchValidator pRecBatchValidator = new GSRecBatchValidator(
                pRecValidator, pSheetConf.SheetName);
            InspValidator pInspValidator = new InspValidator(
                InspValidationMethods.ValidatePirm, inspPoolCommunicator.FetchByVieta, inspTagMaker);
            InspBatchValidator pInspBatchValidator = new InspBatchValidator(pInspValidator);

            // nepirmieji specific objects
            SheetConfig nSheetConf = sheetData["nepirmieji"];
            NepirmRecValidator nRecValidator = new NepirmRecValidator(
                nSheetConf.Mapping, new NepirmRecTagMaker(nSheetConf.Mapping));
            GSRecBatchValidator nRecBatchValidator = new GSRecBatchValidator(
                nRecValidator, nSheetConf.SheetName);
            InspValidator nInspValidator = new InspValidator(
                InspValidationMethods.ValidateNepirm, inspPoolCommunicator.FetchById, inspTagMaker);
            InspBatchValidator nInspBatchValidator = new InspBatchValidator(nInspValidator);

            // accumulators
            StringBuilder recValidationLog;
            StringBuilder inspValidationLog;
            List<InvalidInfo> allRecInvalids;
            List<InvalidInfo> allInspInvalids;
            List<Insp> allCreatedInsps;
            List<Insp> allUpdatedInsps;

            do
            {
                Console.WriteLine($"DB file name: \"{Properties.Settings.Default.DbPath}\"");
                // get action from user
                string input = readUserInput();
                switch (input)
                {
                    case "v":
                        action = Actions.ValidateOnly;
                        break;
                    case "u":
                        action = Actions.UpdateFaultless;
                        break;
                    case "fu":
                        action = Actions.ForceUpdate;
                        break;
                }

                if (action != Actions.ValidateOnly)
                {
                    // create db backup
                    bool dbBackupSuccess = createDbBackup(logger);
                }

                recValidationLog = new StringBuilder();
                inspValidationLog = new StringBuilder();
                allRecInvalids = new List<InvalidInfo>();
                allInspInvalids = new List<InvalidInfo>();
                allCreatedInsps = new List<Insp>();
                allUpdatedInsps = new List<Insp>();

                SheetConfig sheet = sheetData["pirmieji"];
                foreach (var spreadsheet in spreadsheets)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Operatorius: {spreadsheet.OperatorId}, lentelė: {sheet.SheetName}");
                    Console.WriteLine("fetching and validating records...");

                    pInspBatchValidator.Context["operatorId"] = spreadsheet.OperatorId;
                    pInspBatchValidator.Context["sheetName"] = sheet.SheetName;

                    var pirmValidate = fetchAndValidate(
                        sheetData["pirmieji"], spreadsheet, recFetcher,
                        service, pRecBatchValidator, pInspBatchValidator);

                    bool toUpdateDb = collectLogThenDecideIfUpdateDb(
                        pirmValidate, action, logger, reporter, allRecInvalids, allInspInvalids, allCreatedInsps);

                    if (!toUpdateDb)
                        continue;

                    Console.WriteLine("updating db...");
                    updateDb(
                        pirmValidate.Item2, inspPoolCommunicator.BatchInsertInsp,
                        allUpdatedInsps, gsUpdater, service, sheet, spreadsheet);
                }


                sheet = sheetData["nepirmieji"];
                foreach (var spreadsheet in spreadsheets)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Operatorius: {spreadsheet.OperatorId}, lentelė: {sheet.SheetName}");
                    Console.WriteLine("fetching and validating records...");

                    nInspBatchValidator.Context["operatorId"] = spreadsheet.OperatorId;
                    nInspBatchValidator.Context["sheetName"] = sheet.SheetName;

                    var nepirmValidate = fetchAndValidate(
                        sheetData["nepirmieji"], spreadsheet, recFetcher,
                        service, nRecBatchValidator, nInspBatchValidator);

                    bool toUpdateDb = collectLogThenDecideIfUpdateDb(
                        nepirmValidate, action, logger, reporter, allRecInvalids, allInspInvalids, allCreatedInsps);

                    if (!toUpdateDb)
                        continue;

                    Console.WriteLine("updating db...");
                    updateDb(
                        nepirmValidate.Item2, inspPoolCommunicator.BatchUpdateInsp,
                        allUpdatedInsps, gsUpdater, service, sheet, spreadsheet);
                }


                RepeatFinder repFinder = new RepeatFinder();
                Dictionary<string, List<Insp>> repeats = repFinder.FindRepeats(allCreatedInsps);

                // create reports:
                //  allRecInvalids, allInspInvalids, inspDuplicates
                Console.WriteLine();
                LogWriter verifyLog = LogWriterFactory.Create(LogFile.VerifyLog);
                verifyLog.Log("Critical:\r\n" + reporter.ReportRecValidation(allRecInvalids));
                verifyLog.Log("Suspicious:\r\n" + reporter.ReportInspValidation(allInspInvalids));
                string repInspsReport = "Repeated Inspections:\r\n" + reporter.ReportRepeatedInsps(repeats);
                Console.WriteLine(repInspsReport);
                verifyLog.Log(repInspsReport);

                if (allUpdatedInsps.Count > 0)
                {
                    LogWriter dbUpdateReport = LogWriterFactory.Create(LogFile.DbUpdateReport);
                    dbUpdateReport.Log(reporter.ReportDbUpdate(allUpdatedInsps));
                }
                Console.WriteLine("Done");
                //Console.ReadKey();
            } while (true);
        }

        /// <summary>
        /// Fetches records from one sheet of Google Sheets spreadsheet, validates them. 
        /// If invalid records encoutered, puts InvalidInfo data into supplied
        /// allRecInvalids list and returns null.
        /// If invalid records not encountered, converts them to Insp objects and validates them.
        /// </summary>
        /// <param name="sheet">SheetConfig object</param>
        /// <param name="spreadsheet">SpreadsheetConfig object</param>
        /// <param name="recFetcher"></param>
        /// <param name="service"></param>
        /// <param name="recBatchValidator"></param>
        /// <param name="inspBatchValidator"></param>
        /// <returns>Tuple<List<InvalidInfo>, List<Insp>, List<InvalidInfo>>:
        /// Item1 - InvalidInfo list if invalid records found
        /// Item2 - Insp list if invalid records not found
        /// Item3 - InvalidInfo list if invalid records not found and if invalid Insps found</returns>
        private static Tuple<List<InvalidInfo>, List<Insp>, List<InvalidInfo>> fetchAndValidate(
            SheetConfig sheet, SpreadSheetConfig spreadsheet, GSRecordFetcher recFetcher, 
            SheetsService service, GSRecBatchValidator recBatchValidator, InspBatchValidator inspBatchValidator)
        {
            // fetch records
            recFetcher.Initialize(spreadsheet.Id, sheet, service);
            List<IList<object>> recs = recFetcher.Fetch();

            Console.WriteLine($" ...{recs.Count} records fetched");
            // if no records in this sheet - return nulls
            if (recs.Count == 0)
                return new Tuple<List<InvalidInfo>, List<Insp>, List<InvalidInfo>>(null, null, null);

            // validate records
            recBatchValidator.OperatorId = spreadsheet.OperatorId;
            List<InvalidInfo> recInvalids = recBatchValidator.ValidateBatch(recs);

            // if invalid records found - return InvalidInfo 
            if (recInvalids.Count > 0)
            {
                return new Tuple<List<InvalidInfo>, List<Insp>, List<InvalidInfo>>(recInvalids, null, null);
            }
            
            // convert records to insps
            var insps = recs.Select(r => Insp.FromValidRecord(r, sheet.Mapping, spreadsheet.OperatorId)).ToList();

            // validate insps
            var inspInvalids = inspBatchValidator.Validate(insps).ToList();

            if (inspInvalids.Count == 0)
            {
                return new Tuple<List<InvalidInfo>, List<Insp>, List<InvalidInfo>>(null, insps, null);
            }

            return new Tuple<List<InvalidInfo>, List<Insp>, List<InvalidInfo>>(null, insps, inspInvalids);
        }

        /// <summary>
        /// Logs validation result, accumulates InvalidInfo data,
        /// decides whether to update database
        /// </summary>
        /// <param name="validateResult"></param>
        /// <param name="action"></param>
        /// <param name="tempLogger"></param>
        /// <param name="reporter"></param>
        /// <param name="allRecInvalids"></param>
        /// <param name="allInspInvalids"></param>
        /// <returns>True if database should be updated and False if not</returns>
        private static bool collectLogThenDecideIfUpdateDb(
            Tuple<List<InvalidInfo>, List<Insp>, List<InvalidInfo>> validateResult, Actions action,
            LogWriter tempLogger, PlainTxtReporter1 reporter, 
            List<InvalidInfo> allRecInvalids, List<InvalidInfo> allInspInvalids, List<Insp> allCreatedInsps)
        {

            // if no rec invalids, collects insps for repeat finding
            if (validateResult.Item2 != null && validateResult.Item1 == null)
            {
                allCreatedInsps.AddRange(validateResult.Item2);
            }

            // log invalid records
            if (validateResult.Item1 != null)
            {
                StringBuilder nRecReport = reporter.ReportRecValidation(validateResult.Item1, false);
                tempLogger.Log(nRecReport);
                Console.WriteLine("... critical:");
                Console.WriteLine(nRecReport);
                allRecInvalids.AddRange(validateResult.Item1);
            }

            // log invalid insps
            if (validateResult.Item3 != null)
            {
                // log, console, collect invalid info
                StringBuilder nInspReport = reporter.ReportInspValidation(validateResult.Item3, false);
                tempLogger.Log(nInspReport);
                Console.WriteLine("... suspicious:");
                Console.WriteLine(nInspReport);
                allInspInvalids.AddRange(validateResult.Item3);
            }

            // no records
            if (validateResult.Item1 == null && validateResult.Item2 == null)
                return false;

            // no invalid records && validate only
            if (validateResult.Item1 == null && action == Actions.ValidateOnly)
                return false;

            // invalid records
            if (validateResult.Item1 != null)
                return false;

            // invalid insps && don't force update
            if (validateResult.Item3 != null && action != Actions.ForceUpdate)
                return false;

            return true;
        }

        private static int updateDb(
            List<Insp> insps, DbUpdateMethod dbUpdateMethod, List<Insp> allInsps, 
            GSheetsUpdater gsUpdater, SheetsService service, 
            SheetConfig sheet, SpreadSheetConfig spreadsheet)
        {
            // -- update database
            int result = dbUpdateMethod(insps);
            Console.WriteLine($" ... {result} of {insps.Count} DB updates committed");
            if (result != insps.Count)
            {
                Console.WriteLine("ATTENTION: DB UPDATE RESULT DOESN'T MATCH INSPECTION COUNT!");
            }
            // -- accumulate
            allInsps.AddRange(insps);
            // -- update google sheet
            gsUpdater.BatchUpdateSheet(spreadsheet.Id, sheet, service);

            return result;
        }

        private static bool createDbBackup(LogWriter logger)
        {
            // create db backup
            if (!Properties.Settings.Default.CreateDBBackup)
                return true;
            string dbBackupFileName = string.Format(
                      Path.Combine(
                          Properties.Settings.Default.OutputDir,
                          Properties.Settings.Default.DbBackupFileName),
                      DateTime.Now);

            string dbBackupDir = Path.GetDirectoryName(dbBackupFileName);

            if (!Directory.Exists(dbBackupDir))
            {
                try
                {
                    Directory.CreateDirectory(dbBackupDir);
                }
                catch (Exception ex)
                {
                    string error = $"Can not create DB backup directory \"{dbBackupDir}\"";
                    Console.WriteLine(error);
                    logger.Log(new Exception(error, ex));
                    return false;
                }
            }

            try
            {
                File.Copy(Properties.Settings.Default.DbPath, dbBackupFileName, true);
                Console.WriteLine($"DB file backup created: \"{dbBackupFileName}\"");
            }
            catch (Exception ex)
            {
                string error = $"Can not create DB backup \"{dbBackupFileName}\"";
                Console.WriteLine(error);
                logger.Log(new Exception(error, ex));
                return false;
            }

            return true;
        }

        private static string readUserInput()
        {
            Console.BufferWidth = 120;
            Console.WindowWidth = Console.BufferWidth;
            Console.TreatControlCAsInput = true;

            string inputString = String.Empty;
            string query = " v - validate only,\n u - update if no flaws,\n fu - force update,\n Esc - exit";
            ConsoleKeyInfo keyInfo;
            do
            {
                Console.WriteLine(query);
                inputString = String.Empty;
                do
                {
                    keyInfo = Console.ReadKey(true);
                    // Ignore if Alt or Ctrl is pressed.
                    if ((keyInfo.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt)
                        continue;
                    if ((keyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                        continue;
                    // Ignore if KeyChar value is \u0000.
                    if (keyInfo.KeyChar == '\u0000') continue;
                    // Ignore tab key.
                    if (keyInfo.Key == ConsoleKey.Tab) continue;
                    // Handle backspace.
                    if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        // Are there any characters to erase?
                        if (inputString.Length >= 1)
                        {
                            // Determine where we are in the console buffer.
                            int cursorCol = Console.CursorLeft - 1;
                            int oldLength = inputString.Length;
                            int extraRows = oldLength / 80;

                            inputString = inputString.Substring(0, oldLength - 1);
                            Console.CursorLeft = 0;
                            Console.CursorTop = Console.CursorTop - extraRows;
                            Console.Write(inputString + new String(' ', oldLength - inputString.Length));
                            Console.CursorLeft = cursorCol;
                        }
                        continue;
                    }
                    // Handle Escape key.
                    if (keyInfo.Key == ConsoleKey.Escape)
                        System.Environment.Exit(1);
                    // Handle key by adding it to input string.
                    Console.Write(keyInfo.KeyChar);
                    inputString += keyInfo.KeyChar;
                } while (keyInfo.Key != ConsoleKey.Enter);

                inputString = inputString.Trim();
                Console.WriteLine("{0}", inputString);
            } while (inputString != "v" && inputString != "u" && inputString != "fu");
            return inputString;
        }
        

    }
}
