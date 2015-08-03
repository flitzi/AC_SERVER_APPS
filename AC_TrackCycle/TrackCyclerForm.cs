using AC_SessionReportPlugin;
using acPlugins4net.messages;
using AC_ServerStarter;
using System;
using System.IO;
using System.Windows.Forms;
using System.Configuration;

namespace AC_TrackCycle_Console
{
    public partial class TrackCyclerForm : Form
    {
        private readonly TrackCycler trackCycler;
        MyPlugin myPlugin;
        private int logLength = 0;

        public TrackCyclerForm()
        {
            this.InitializeComponent();

            myPlugin = new MyPlugin(this);

            try
            {
                ReportHandlerLoader.LoadHandler(myPlugin);
            }
            catch
            {
                MessageBox.Show("Error", "Could not load SessionReportHandler");
            }

            myPlugin.Connect();

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

            this.trackCycler = new TrackCycler(serverfolder, myPlugin);

            if (trackCycler.Sessions.Count < 2)
            {
                this.buttonNextTrack.Enabled = false;
                this.checkBoxAutoChangeTrack.Checked = false;
                this.checkBoxAutoChangeTrack.Enabled = false;
            }

            foreach (RaceSession session in trackCycler.Sessions)
            {
                this.textBoxOutput.Text += session.ToString() + Environment.NewLine;
            }

            this.trackCycler.AutoChangeTrack = this.checkBoxAutoChangeTrack.Checked;
            this.trackCycler.CreateLogs = this.checkBoxCreateLogs.Checked;

            this.trackCycler.MessageReceived += trackCycler_MessageReceived;
            this.trackCycler.TrackChanged += trackCycler_TrackChanged;
        }

        public void SetSessionInfo(MsgNewSession newSession)
        {
            if (newSession != null)
            {
                if (newSession.SessionType == 3)
                {
                    this.textBox_sessionInfo.Text = newSession.Name + " " + newSession.Laps + " laps, " + newSession.Weather + ", ambient " + newSession.AmbientTemp + "°, road " + newSession.RoadTemp + "°";
                }
                else
                {
                    this.textBox_sessionInfo.Text = newSession.Name + " " + newSession.TimeOfDay + " min, " + newSession.Weather + ", ambient " + newSession.AmbientTemp + "°, road " + newSession.RoadTemp + "°";
                }
            }
        }

        void trackChanged(RaceSession session)
        {
            this.textBoxCurrentCycle.Text = session.Track + " " + session.Layout;
        }

        void trackCycler_TrackChanged(RaceSession session)
        {
            BeginInvoke(new TrackChanged(trackChanged), session);
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

        void trackCycler_MessageReceived(string message)
        {
            BeginInvoke(new MessageReceived(WriteMessage), message);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.trackCycler.StopServer();
            this.myPlugin.Disconnect();
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
            this.trackCycler.CreateLogs = this.checkBoxCreateLogs.Checked;
        }

        private void textBox_chat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && !string.IsNullOrEmpty(textBox_chat.Text))
            {
                this.myPlugin.BroadcastChatMessage(textBox_chat.Text);
                textBox_chat.Text = string.Empty;
            }
        }
    }
}
