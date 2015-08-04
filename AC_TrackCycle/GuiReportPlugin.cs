using AC_SessionReportPlugin;
using acPlugins4net.configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using acPlugins4net.messages;
using AC_SessionReport;

namespace AC_TrackCycle
{
    public class GuiReportPlugin : ReportPlugin
    {
        private readonly TrackCyclerForm form;

        public GuiReportPlugin(TrackCyclerForm form, LogWriter logWriter, IConfigManager config = null)
            : base(logWriter, config)
        {
            this.form = form;
        }

        public override void OnNewSessionMsg(MsgNewSession msg)
        {
            base.OnNewSessionMsg(msg);

            if (msg != null)
            {
                this.form.BeginInvoke(new Action<SessionReport>(this.form.SetSessionInfo), this.currentSession);
            }
        }
    }
}
