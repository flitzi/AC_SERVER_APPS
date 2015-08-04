using AC_SessionReportPlugin;
using acPlugins4net.messages;
using AC_ServerStarter;
using System;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;
using AC_SessionReport;

namespace AC_TrackCycle_Console
{
    public partial class TrackCyclerForm : Form
    {
        private readonly GuiLogWriter logWriter;
        private readonly ReportPlugin plugin;
        private readonly TrackCycler trackCycler;
        
        private int logLength = 0;

        public TrackCyclerForm()
        {
            this.InitializeComponent();

            this.logWriter = new GuiLogWriter(this, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "logs"));
            this.plugin = new ReportPlugin(this.logWriter);

            try
            {
                ReportHandlerLoader.LoadHandler(this.plugin);
            }
            catch
            {
                MessageBox.Show("Error", "Could not load SessionReportHandler");
            }

            plugin.Connect();

            string serverfolder;
            if (File.Exists(Path.Combine(Application.StartupPath, "acServer.exe")))
            {
                serverfolder = Application.StartupPath;
            }
            else
            {
                //not used if acServer.exe is found next to this app
                serverfolder = ConfigurationManager.AppSettings["acServerDirectory"];
            }

            this.trackCycler = new TrackCycler(serverfolder, plugin, plugin.LogWriter);

            if (trackCycler.HasCycle)
            {
                this.buttonNextTrack.Enabled = false;
                this.checkBoxAutoChangeTrack.Checked = false;
                this.checkBoxAutoChangeTrack.Enabled = false;
            }

            this.trackCycler.AutoChangeTrack = this.checkBoxAutoChangeTrack.Checked;
            this.trackCycler.WriteAllMessages = this.checkBoxCreateLogs.Checked;
        }

        public void SetSessionInfo(SessionReport newSession)
        {
            if (newSession != null)
            {
                if (newSession.Type == 3)
                {
                    this.textBox_sessionInfo.Text = newSession.SessionName + " " + newSession.Laps + " laps, " + newSession.Weather + ", ambient " + newSession.AmbientTemp + "°, road " + newSession.RoadTemp + "°";
                }
                else
                {
                    this.textBox_sessionInfo.Text = newSession.SessionName + " " + newSession.DurationSecs / 60 + " min, " + newSession.Weather + ", ambient " + newSession.AmbientTemp + "°, road " + newSession.RoadTemp + "°";
                }
                this.textBoxCurrentCycle.Text = newSession.TrackName + " " + newSession.TrackConfig;
            }
        }

        public void WriteMessage(string message)
        {
            if (logLength > 50000)
            {
                this.textBoxOutput.Text = string.Empty;
                logLength = 0;
            }

            this.textBoxOutput.AppendText(message + Environment.NewLine);
            logLength++;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.trackCycler.StopServer();
            this.plugin.Disconnect();
            this.logWriter.StopLogging();
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
            this.trackCycler.WriteAllMessages = this.checkBoxCreateLogs.Checked;
        }

        private void textBox_chat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && !string.IsNullOrEmpty(textBox_chat.Text))
            {
                this.plugin.BroadcastChatMessage(textBox_chat.Text);
                textBox_chat.Text = string.Empty;
            }
        }
    }
}
