using System;
using acPlugins4net;
using acPlugins4net.messages;

namespace AC_TrackCycle
{
    public class GuiTrackCyclePlugin : AcServerPluginBase
    {
        private readonly TrackCyclerForm form;

        public GuiTrackCyclePlugin(TrackCyclerForm form)
        {
            this.form = form;
        }

        protected override void OnNewSessionBase(MsgSessionInfo msg)
        {
            this.form.BeginInvoke(new Action<MsgSessionInfo>(this.form.SetSessionInfo), msg);
        }
    }
}