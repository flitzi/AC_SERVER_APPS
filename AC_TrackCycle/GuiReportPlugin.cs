using System;
using acPlugins4net;
using acPlugins4net.messages;
using AC_SessionReportPlugin;
using AC_ServerStarter;

namespace AC_TrackCycle
{
    public class GuiTrackCyclePlugin : TrackCyclePlugin
    {
        private readonly TrackCyclerForm form;

        public GuiTrackCyclePlugin(TrackCyclerForm form)
        {
            this.form = form;
        }

        protected override void OnNewSessionBase(MsgSessionInfo msg)
        {
            base.OnNewSessionBase(msg);
            this.form.BeginInvoke(new Action<MsgSessionInfo>(this.form.SetSessionInfo), msg);
        }
    }
}