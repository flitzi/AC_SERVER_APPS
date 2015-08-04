using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AC_SessionReport;

namespace AC_SessionReportPlugin
{
    public class LogWriter
    {
        private readonly object lockObject = new object();

        private readonly string logDirectory;
        private StreamWriter log;
        private string currentFile;

        public LogWriter(string logDirectory)
        {
            this.logDirectory = logDirectory;
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(this.logDirectory);
            }
        }

        public virtual void StartLoggingToFile(SessionReport sessionReport)
        {
            lock (lockObject)
            {
                this.StopLogging();
                this.currentFile = Path.Combine(this.logDirectory,
                    sessionReport.TimeStamp.ToString("yyyyMMdd_HHmmss" + "_" + sessionReport.SessionName + ".log"));
            }
        }

        public virtual void LogMessage(string message)
        {
            lock (lockObject)
            {
                if (currentFile != null && this.log == null)
                {
                    try
                    {
                        this.log = new StreamWriter(currentFile);
                        this.log.AutoFlush = true;
                    }
                    catch (Exception ex)
                    {
                        currentFile = null;
                        Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                }
                if (this.log != null)
                {
                    this.log.WriteLine(message);
                }
            }
        }

        public virtual void LogException(Exception ex)
        {
            this.LogMessage("Error: " + ex.Message + Environment.NewLine + ex.StackTrace);
        }

        public virtual void StopLogging()
        {
            lock (lockObject)
            {
                if (this.log != null)
                {
                    this.log.Close();
                    this.log.Dispose();
                    this.log = null;
                    this.currentFile = null;
                }
            }
        }
    }
}
