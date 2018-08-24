using System;
using acPlugins4net;
using acPlugins4net.info;
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

        public override void ChangeTrack(int index, bool broadcastResults)
        {
            this.form.BeginInvoke(new Action(this.form.TrackChanging), null);
            base.ChangeTrack(index, broadcastResults);
            this.form.BeginInvoke(new Action(this.form.TrackChanged), null);
        }

        protected override void OnNewSession(MsgSessionInfo msg)
        {
            base.OnNewSession(msg);
            this.form.BeginInvoke(new Action<MsgSessionInfo>(this.form.SetSessionInfo), msg);
            this.form.BeginInvoke(new Action(this.form.UpdateGui), null);
        }

        protected override void OnSessionInfo(MsgSessionInfo msg)
        {
            base.OnNewSession(msg);
            this.form.BeginInvoke(new Action<MsgSessionInfo>(this.form.SetSessionInfo), msg);
            this.form.BeginInvoke(new Action(this.form.UpdateGui), null);
        }

        protected override void OnClientLoaded(MsgClientLoaded msg)
        {
            base.OnClientLoaded(msg);
            this.form.BeginInvoke(new Action(this.form.UpdateGui), null);
            this.form.BeginInvoke(new Action<MsgClientLoaded>(this.form.OnClientLoadedG), msg);
        }

        protected override void OnConnectionClosed(MsgConnectionClosed msg)
        {
            base.OnConnectionClosed(msg);
            this.form.BeginInvoke(new Action(this.form.UpdateGui), null);
            this.form.BeginInvoke(new Action(this.form.UpdatePositionGraph), null);
        }

        protected override void OnBulkCarUpdateFinished()
        {
            base.OnBulkCarUpdateFinished();
            this.form.BeginInvoke(new Action(this.form.UpdatePositionGraph), null);
        }

        protected override void OnLapCompleted(LapInfo lap)
        {
            base.OnLapCompleted(lap);
            this.form.BeginInvoke(new Action(this.form.UpdateGui), null);
        }
    }
}