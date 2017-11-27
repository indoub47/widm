using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Widm
{
    public enum LogFile { VerifyLog, ProcessLog, DbUpdateReport};
    public static class LogWriterFactory
    {
        public static LogWriter Create(LogFile logType, string outputFolder = "")
        {
            string fileName = string.Empty;
            switch (logType)
            {
                case (LogFile.VerifyLog):
                    fileName = Properties.Settings.Default.VerifyLogFileName;
                    break;
                case (LogFile.ProcessLog):
                    fileName = Properties.Settings.Default.ProcessLogFileName;
                    break;
                case (LogFile.DbUpdateReport):
                    fileName = Properties.Settings.Default.DbUpdateReportFileName;
                    break;
            }

            if (outputFolder != "")
            {
                fileName = Path.Combine(outputFolder, fileName);
            }
            else
            {
                fileName = Path.Combine(Properties.Settings.Default.OutputDir, fileName);
            }
            
            fileName = string.Format(fileName, DateTime.Now);

            if (isFileOk(fileName))
                return new LogWriter(fileName);
            else
            {
                string error = $"Nepavyksta sukurti log failo \"{fileName}\"";
                Console.WriteLine(error);
                throw new Exception(error);
            }
        }

        private static bool isFileOk(string fileName)
        {
            string outputDir = Path.GetDirectoryName(fileName);
            try
            {
                checkOutputFolder(outputDir);
            }
            catch (Exception)
            {
                string error = $"Nepavyksta sukurti output directory \"{outputDir}\"";
                Console.WriteLine(error);
                return false;
            }

            if (!dirIsWritable(outputDir))
            {
                string error = $"Output directory \"{outputDir}\" is not writable";
                Console.WriteLine(error);
                return false;
            }

            if (!File.Exists(fileName))
            {
                try
                {
                    FileStream fs = File.Create(fileName);
                    fs.Close();
                }
                catch
                {
                    string error = $"Unable to create file \"{outputDir}\"";
                    Console.WriteLine(error);
                    return false;
                }
            }

            return true;
        }

        private static void checkOutputFolder(string outputDir)
        {
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
        }


        /// <summary>
        /// Checks the ability to create and write to a file in the supplied directory.
        /// </summary>
        /// <param name="directory">String representing the directory path to check.</param>
        /// <returns>True if successful; otherwise false.</returns>
        private static bool dirIsWritable(string directory)
        {
            const string TEMP_FILE = "tempFile.tmp";
            bool success = false;
            string fullPath = Path.Combine(directory, TEMP_FILE);

            if (Directory.Exists(directory))
            {
                try
                {
                    using (FileStream fs = new FileStream(fullPath, FileMode.Create,
                                                                    FileAccess.Write))
                    {
                        fs.WriteByte(0xff);
                    }

                    if (File.Exists(fullPath))
                    {
                        success = true;
                        try
                        {
                            File.Delete(fullPath);
                        }
                        catch
                        {
                            // neleido ištrinti tmp, tai ir chuj s nim
                        }
                    }
                }
                catch (Exception)
                {
                    success = false;
                }
            }

            return success;
        }
    }
}
