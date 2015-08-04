using AC_SessionReportPlugin;
using acPlugins4net.messages;
using System;
using AC_ServerStarter;
using AC_SessionReport;

namespace AC_TrackCycle
{
    public class GuiLogWriter : LogWriter
    {
        private readonly TrackCyclerForm form;

        public bool LogMessagesToFile = true;

        public GuiLogWriter(TrackCyclerForm form, string logDirectory) : base(logDirectory)
        {
            this.form = form;
        }

        public override void LogMessage(string message)
        {
            if (LogMessagesToFile)
            {
                base.LogMessage(message);
            }
            this.form.BeginInvoke(new Action<string>(this.form.WriteMessage), message);
        }

        public override void LogException(Exception ex)
        {
            base.LogException(ex);
            this.form.BeginInvoke(new Action<string>(this.form.WriteMessage), "Error: " + ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }
}
