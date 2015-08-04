using AC_SessionReportPlugin;
using acPlugins4net.messages;
using System;
using AC_ServerStarter;
using AC_SessionReport;

namespace AC_TrackCycle_Console
{
    public class GuiLogWriter : LogWriter
    {
        private readonly TrackCyclerForm form;

        public GuiLogWriter(TrackCyclerForm form, string logDirectory) : base(logDirectory)
        {
            this.form = form;
        }

        public override void LogMessage(string message)
        {
            base.LogMessage(message);
            this.form.BeginInvoke(new Action<string>(this.form.WriteMessage), message);
        }

        public override void StartLoggingToFile(SessionReport sessionReport)
        {
            base.StartLoggingToFile(sessionReport);
            this.form.BeginInvoke(new Action<SessionReport>(this.form.SetSessionInfo), sessionReport);
        }
    }
}
