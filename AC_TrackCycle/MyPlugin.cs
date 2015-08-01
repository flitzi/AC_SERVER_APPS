using AC_SessionReportPlugin;
using acPlugins4net.messages;
using System;

namespace AC_TrackCycle_Console
{
    public class MyPlugin : ReportPlugin
    {
        TrackCyclerForm form;
        public MyPlugin(TrackCyclerForm form)
        {
            this.form = form;
        }

        public override void OnNewSession(MsgNewSession msg)
        {
            base.OnNewSession(msg);
            form.BeginInvoke(new Action<MsgNewSession>(form.SetSessionInfo), msg);
        }

        protected override void OnError(Exception ex)
        {
            // TODO also log to TrackCycler log file
            form.BeginInvoke(new Action<string>(form.WriteMessage), ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }
}
