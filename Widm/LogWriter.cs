using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Widm
{
    public class LogWriter
    {
        public string FileName { get; }

        public LogWriter(string fileName)
        {
            this.FileName = fileName;
        }

        public void Log(Exception exception)
        {
            try
            {
                using (StreamWriter w = File.AppendText(FileName))
                {
                    writeLog(exception, w);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Log(StringBuilder strbuilder)
        {
            Log(strbuilder.ToString());
        }

        public void Log(string logText)
        {
            try
            {
                using (StreamWriter w = File.AppendText(FileName))
                {
                    writeLog(logText, w);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(logText);
                Console.WriteLine(ex.Message);
            }
        }

        private void writeLog(Exception exception, TextWriter writer)
        {
            StringBuilder sb = new StringBuilder();
            exceptionRecord(exception, sb);
            try
            {
                writer.Write("\r\nException Log Entry: ");
                writer.WriteLine("{0} {1}",
                    DateTime.Now.ToShortDateString(),
                    DateTime.Now.ToLongTimeString());
                writer.WriteLine(sb);
                writer.WriteLine("-------------------------------");
            }
            catch
            {
            }
        }

        private void writeLog(string logText, TextWriter writer)
        {
            try
            {
                writer.Write("\r\nLog Entry: ");
                writer.WriteLine("{0} {1}",
                    DateTime.Now.ToShortDateString(),
                    DateTime.Now.ToLongTimeString());
                writer.WriteLine(" : ");
                writer.WriteLine(" : " + logText);
                writer.WriteLine("------------------------------------");
            }
            catch
            {
            }
        }

        private void exceptionRecord(Exception exception, StringBuilder sb)
        {
            if (exception.InnerException != null)
            {
                 exceptionRecord(exception.InnerException, sb);
            }

            sb.AppendFormat("\t\t------- Exception Type: {0}", exception.GetType()).AppendLine();
            sb.AppendFormat("Message: {0}", exception.Message).AppendLine();
            sb.AppendFormat("Source: {0}", exception.Source).AppendLine();
            sb.AppendFormat("Stack trace: {0}", exception.StackTrace).AppendLine();
            sb.AppendFormat("Target: {0}", exception.TargetSite).AppendLine();
        }
    }

}
