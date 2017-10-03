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

            // get action from user

            if (action != Actions.ValidateOnly)
            {
                // create db backup
                bool dbBackupSuccess = CreateDbBackup(logger);
                // ask user if he wants to continue despite db backup failure
            }

            // GoogleSheets config
            SpreadSheetConfig[] spreadsheets = SpreadSheetConfig.GetConfig();
            Dictionary<string, SheetConfig> sheetData = SheetConfig.GetConfig();

            List<Tuple<string, string>> skipped = new List<Tuple<string, string>>();

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
                pSheetConf.Mapping, new PirmRecTagMaker());
            GSRecBatchValidator pRecBatchValidator = new GSRecBatchValidator(
                pRecValidator, pSheetConf.SheetName);
            InspValidator pInspValidator = new InspValidator(
                InspValidationMethods.ValidatePirm, inspPoolCommunicator.FetchByVieta, inspTagMaker);
            InspBatchValidator pInspBatchValidator = new InspBatchValidator(pInspValidator);

            // nepirmieji specific objects
            SheetConfig nSheetConf = sheetData["nepirmieji"];
            NepirmRecValidator nRecValidator = new NepirmRecValidator(
                nSheetConf.Mapping, new NepirmRecTagMaker());
            GSRecBatchValidator nRecBatchValidator = new GSRecBatchValidator(
                nRecValidator, nSheetConf.SheetName);
            InspValidator nInspValidator = new InspValidator(
                InspValidationMethods.ValidateNepirm, inspPoolCommunicator.FetchById, inspTagMaker);
            InspBatchValidator nInspBatchValidator = new InspBatchValidator(nInspValidator);

            // accumulators
            StringBuilder recValidationLog = new StringBuilder();
            StringBuilder inspValidationLog = new StringBuilder();
            List<InvalidInfo> allRecInvalids = new List<InvalidInfo>();
            List<InvalidInfo> allInspInvalids = new List<InvalidInfo>();
            List<Insp> allInsps = new List<Insp>();

            SheetConfig sheet = sheetData["pirmieji"];
            foreach (var spreadsheet in spreadsheets)
            {

                var pirmValidate = fetchAndValidate(
                    sheetData["pirmieji"], spreadsheet, recFetcher, 
                    service, pRecBatchValidator, pInspBatchValidator);

                bool toUpdateDb = CollectLogThenDecideIfUpdateDb(
                    pirmValidate, action, logger, reporter, allRecInvalids, allInspInvalids);

                if (! toUpdateDb)
                    continue;

                updateDb(
                    pirmValidate.Item2, inspPoolCommunicator.BatchInsertInsp, 
                    allInsps, gsUpdater, service, sheet, spreadsheet);               
            }

            sheet = sheetData["nepirmieji"];
            foreach (var spreadsheet in spreadsheets)
            {

                var nepirmValidate = fetchAndValidate(
                    sheetData["nepirmieji"], spreadsheet, recFetcher,
                    service, nRecBatchValidator, nInspBatchValidator);

                bool toUpdateDb = CollectLogThenDecideIfUpdateDb(
                    nepirmValidate, action, logger, reporter, allRecInvalids, allInspInvalids);

                if (!toUpdateDb)
                    continue;

                updateDb(
                    nepirmValidate.Item2, inspPoolCommunicator.BatchUpdateInsp,
                    allInsps, gsUpdater, service, sheet, spreadsheet);
            }

            RepeatFinder repFinder = new RepeatFinder();
            Dictionary<string, List<Insp>> repeats = repFinder.FindRepeats(allInsps);

            // create reports:
            //  allRecInvalids, allInspInvalids, inspDuplicates
            LogWriter verifyLog = LogWriterFactory.Create(LogFile.VerifyLog);
            verifyLog.Log("Critical:\n" + reporter.ReportRecValidation(allRecInvalids));
            verifyLog.Log("Suspicious:\n" + reporter.ReportInspValidation(allInspInvalids));
            verifyLog.Log("Repeated Inspections:\n" + reporter.ReportRepeatedInsps(repeats));

            if (allInsps.Count > 0)
            {
                LogWriter dbUpdateReport = LogWriterFactory.Create(LogFile.DbUpdateReport);
                dbUpdateReport.Log(reporter.ReportDbUpdate(allInsps));
            }
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
        private static bool CollectLogThenDecideIfUpdateDb(
            Tuple<List<InvalidInfo>, List<Insp>, List<InvalidInfo>> validateResult, Actions action,
            LogWriter tempLogger, PlainTxtReporter1 reporter, 
            List<InvalidInfo> allRecInvalids, List<InvalidInfo> allInspInvalids)
        {
            // no records
            if (validateResult.Item1 == null && validateResult.Item2 == null)
                return false;

            // no invalid records && validate only
            if (validateResult.Item1 == null && action == Actions.ValidateOnly)
                return false;

            // invalid records
            if (validateResult.Item1 != null)
            {
                // log, console, collect invalid info
                StringBuilder nRecReport = reporter.ReportRecValidation(validateResult.Item1);
                tempLogger.Log(nRecReport);
                Console.WriteLine(nRecReport);
                allRecInvalids.Concat(validateResult.Item1);
                return false;
            }

            // invalid insps
            if (validateResult.Item3 != null)
            {
                // log, console, collect invalid info
                StringBuilder nInspReport = reporter.ReportInspValidation(validateResult.Item3);
                tempLogger.Log(nInspReport);
                Console.WriteLine(nInspReport);
                allInspInvalids.Concat(validateResult.Item3);

                if (action != Actions.ForceUpdate)
                    return false;
            }

            return true;
        }

        private static int updateDb(
            List<Insp> insps, DbUpdateMethod dbUpdateMethod, List<Insp> allInsps, 
            GSheetsUpdater gsUpdater, SheetsService service, 
            SheetConfig sheet, SpreadSheetConfig spreadsheet)
        {
            // -- update database
            int result = dbUpdateMethod(insps);
            // -- accumulate
            allInsps.Concat(insps);
            // -- update google sheet
            gsUpdater.BatchUpdateSheet(spreadsheet.Id, sheet, service);

            return result;
        }

        private static bool CreateDbBackup(LogWriter logger)
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

        private static string ReadUserInput()
        {
            Console.BufferWidth = 120;
            Console.WindowWidth = Console.BufferWidth;
            Console.TreatControlCAsInput = true;

            string inputString = String.Empty;
            ConsoleKeyInfo keyInfo;
            do
            {
                Console.WriteLine("v - verify, s - settings, Esc - exit");
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
            } while (inputString != "v" && inputString != "s");
            return inputString;
        }
        

    }
}
