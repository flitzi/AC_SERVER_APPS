using System;
using acPlugins4net.helpers;

namespace AC_TrackCycle
{
    public class GuiLogWriter : FileLogWriter
    {
        private readonly TrackCyclerForm form;
        public bool LogMessagesToFile = true;

        public GuiLogWriter(TrackCyclerForm form, string defaultLogDirectory, string filePath)
            : base(defaultLogDirectory, filePath)
        {
            this.form = form;
        }

        public override void Log(string message)
        {
            if (this.LogMessagesToFile)
            {
                base.Log(message);
            }
            else if (this.LogToConsole)
            {
                Console.WriteLine(message);
            }
            this.form.BeginInvoke(new Action<string>(this.form.WriteMessage), message);
        }

        public override void Log(Exception ex)
        {
            string str = GetExceptionString(ex);
            this.LogMessageToFile(str);
            if (this.LogToConsole)
            {
                Console.WriteLine(str);
            }
            this.form.BeginInvoke(new Action<string>(this.form.WriteMessage), str);
        }
    }
}