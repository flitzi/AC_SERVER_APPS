using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using acPlugins4net;
using acPlugins4net.messages;
using AC_ServerStarter;

namespace AC_TrackCycle
{
    public partial class TrackCyclerForm : Form
    {
        private readonly GuiLogWriter logWriter;
        private readonly AcServerPluginManager pluginManager;
        private readonly TrackCycler trackCycler;
        private int logLength = 0;

        public TrackCyclerForm()
        {
            this.InitializeComponent();

            this.logWriter = new GuiLogWriter(
                this,
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "logs"),
                DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + "_Startup.log")
            { LogWithTimestamp = true };
            this.logWriter.LogMessagesToFile = this.checkBoxCreateLogs.Checked;

            this.pluginManager = new AcServerPluginManager(this.logWriter);
            this.pluginManager.LoadInfoFromServerConfig();
            this.pluginManager.LoadPluginsFromAppConfig();
            this.pluginManager.AddPlugin(new GuiTrackCyclePlugin(this));

            this.trackCycler = new TrackCycler(this.pluginManager, this.logWriter);

            if (!this.trackCycler.HasCycle)
            {
                this.buttonNextTrack.Enabled = false;
                this.checkBoxAutoChangeTrack.Checked = false;
                this.checkBoxAutoChangeTrack.Enabled = false;
            }

            this.trackCycler.AutoChangeTrack = this.checkBoxAutoChangeTrack.Checked;
        }

        public void SetSessionInfo(MsgSessionInfo msg)
        {
            if (msg != null)
            {
                if (msg.SessionType == (byte)MsgSessionInfo.SessionTypeEnum.Race)
                {
                    this.textBox_sessionInfo.Text = msg.Name + " " + msg.Laps + " laps, " + msg.Weather + ", ambient " + msg.AmbientTemp
                                                    + "°, road " + msg.RoadTemp + "°";
                }
                else
                {
                    this.textBox_sessionInfo.Text = msg.Name + " " + msg.TimeOfDay + " min, " + msg.Weather + ", ambient " + msg.AmbientTemp
                                                    + "°, road " + msg.RoadTemp + "°";
                }
                this.textBoxCurrentCycle.Text = this.pluginManager.Track + " " + this.pluginManager.TrackLayout;
            }
        }

        public void WriteMessage(string message)
        {
            if (this.logLength > 50000)
            {
                this.textBoxOutput.Text = string.Empty;
                this.logLength = 0;
            }

            this.textBoxOutput.AppendText(message + Environment.NewLine);
            this.logLength++;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.trackCycler.StopServer();
            this.logWriter.StopLoggingToFile();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.trackCycler.StartServer();
        }

        private void buttonNextTrack_Click(object sender, EventArgs e)
        {
            this.trackCycler.NextTrack();
        }

        private void checkBoxAutoChangeTrack_CheckedChanged(object sender, EventArgs e)
        {
            this.trackCycler.AutoChangeTrack = this.checkBoxAutoChangeTrack.Checked;
        }

        private void checkBoxCreateLogs_CheckedChanged(object sender, EventArgs e)
        {
            this.logWriter.LogMessagesToFile = this.checkBoxCreateLogs.Checked;
        }

        private void textBox_chat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && !string.IsNullOrEmpty(this.textBox_chat.Text) && this.pluginManager.IsConnected)
            {
                this.pluginManager.BroadcastChatMessage(this.textBox_chat.Text);
                this.textBox_chat.Text = string.Empty;
            }
        }
    }
}