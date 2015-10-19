﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using acPlugins4net;
using acPlugins4net.info;
using acPlugins4net.messages;

namespace AC_TrackCycle
{
    public partial class TrackCyclerForm : Form
    {
        private readonly GuiLogWriter logWriter;
        private readonly AcServerPluginManager pluginManager;
        private readonly GuiTrackCyclePlugin trackCycler;
        private int logLength = 0;
        private MsgSessionInfo currentSessionInfo;
        private Image trackMap;
        private double trackMapScale, trackMapOffsetX, trackMapOffsetY;

        public TrackCyclerForm()
        {
            this.InitializeComponent();

            this.logWriter = new GuiLogWriter(
                this,
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "logs"),
                DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + "_Startup.log")
            { LogWithTimestamp = true };
            this.logWriter.LogMessagesToFile = this.checkBoxCreateLogs.Checked;

            this.trackCycler = new GuiTrackCyclePlugin(this);

            this.pluginManager = new AcServerPluginManager(this.logWriter);
            this.pluginManager.LoadInfoFromServerConfig();
            this.pluginManager.AddPlugin(this.trackCycler);
            this.pluginManager.LoadPluginsFromAppConfig();

            if (!this.trackCycler.HasCycle)
            {
                this.buttonNextTrack.Enabled = false;
                this.checkBoxAutoChangeTrack.Checked = false;
                this.checkBoxAutoChangeTrack.Enabled = false;
                this.button_ChangeTrack.Enabled = false;
            }
            else
            {
                this.listBox_CycleSessions.DataSource = this.trackCycler.Sessions;
            }

            this.trackCycler.AutoChangeTrack = this.checkBoxAutoChangeTrack.Checked;

            this.dataGridView_connections.AutoGenerateColumns = false;
        }

        public void SetSessionInfo(MsgSessionInfo msg)
        {
            if (msg != null)
            {
                this.currentSessionInfo = msg;
                if (msg.SessionType == (byte)MsgSessionInfo.SessionTypeEnum.Race)
                {
                    this.textBox_sessionInfo.Text = msg.Name + " " + msg.Laps + " laps, " + msg.Weather + ", ambient " + msg.AmbientTemp
                                                    + "°, road " + msg.RoadTemp + "°";
                }
                else
                {
                    this.textBox_sessionInfo.Text = msg.Name + " " + msg.SessionDuration + " min, " + msg.Weather + ", ambient " + msg.AmbientTemp
                                                    + "°, road " + msg.RoadTemp + "°";
                }
                this.textBoxCurrentCycle.Text = msg.Track + " " + msg.TrackConfig;

                if (this.trackMap != null)
                {
                    this.trackMap.Dispose();
                    this.trackMap = null;
                }
                string trackName = msg.Track;
                if (!string.IsNullOrWhiteSpace(msg.TrackConfig))
                {
                    trackName += "-" + msg.TrackConfig;
                }
                if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data", trackName + ".png")) &&
                    File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data", trackName + ".ini")))
                {
                    this.trackMap = Bitmap.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data",
                            trackName + ".png"));

                    this.pictureBox_positionGraph.Image = this.trackMap;

                    StreamReader reader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data", trackName + ".ini"));
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        if (line.StartsWith("SCALE_FACTOR="))
                        {
                            this.trackMapScale = double.Parse(line.Replace("SCALE_FACTOR=", ""), CultureInfo.InvariantCulture);
                        }
                        if (line.StartsWith("X_OFFSET="))
                        {
                            this.trackMapOffsetX = double.Parse(line.Replace("X_OFFSET=", ""), CultureInfo.InvariantCulture);
                        }
                        if (line.StartsWith("Z_OFFSET="))
                        {
                            this.trackMapOffsetY = double.Parse(line.Replace("Z_OFFSET=", ""), CultureInfo.InvariantCulture);
                        }

                        line = reader.ReadLine();
                    }
                    reader.Dispose();
                }
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

        public void UpdateGui()
        {
            List<DriverInfo> connectedDrivers = this.pluginManager.CurrentSession.Drivers.Where(d => d.IsConnected).ToList();
            this.textBox_ConnectionCount.Text = connectedDrivers.Count + " drivers currently connected";
            this.dataGridView_connections.DataSource = connectedDrivers;
        }

        public void UpdatePositionGraph()
        {
            if (tabControl1.SelectedIndex == 3)
            {
                if (this.trackMap == null)
                {
                    Bitmap bitmap = new Bitmap(this.pictureBox_positionGraph.Width, this.pictureBox_positionGraph.Height);
                    Graphics graphics = Graphics.FromImage(bitmap);
                    graphics.Clear(Color.White);
                    float radius = Math.Min(bitmap.Width, bitmap.Height) * 0.9f / 2.0f;
                    PointF center = new PointF(bitmap.Width / 2.0f, bitmap.Height / 2.0f);
                    graphics.DrawEllipse(new Pen(Color.DarkGray, 15f), center.X - radius, center.Y - radius, radius * 2, radius * 2);

                    foreach (DriverInfo driver in this.pluginManager.CurrentSession.Drivers.Where(d => d.IsConnected))
                    {
                        PointF pos = new PointF((float)(center.X - Math.Sin(driver.LastSplinePosition * Math.PI * 2)) * radius,
                            (float)(center.Y + Math.Cos(driver.LastSplinePosition * Math.PI * 2) * radius));

                        DrawDriverPos(pos, driver.CarId.ToString(), graphics);
                    }

                    graphics.Dispose();
                    this.pictureBox_positionGraph.Image = bitmap;
                }
                else
                {
                    Bitmap bitmap = new Bitmap(trackMap, trackMap.Width, trackMap.Height);
                    Graphics graphics = Graphics.FromImage(bitmap);

                    foreach (DriverInfo driver in this.pluginManager.CurrentSession.Drivers.Where(d => d.IsConnected))
                    {
                        PointF pos = new PointF((float)(driver.LastPosition.X / trackMapScale + trackMapOffsetX), (float)(driver.LastPosition.Z / trackMapScale + trackMapOffsetY));
                        DrawDriverPos(pos, driver.CarId.ToString(), graphics);
                    }

                    graphics.Dispose();
                    this.pictureBox_positionGraph.Image = bitmap;
                }
            }
        }

        private static void DrawDriverPos(PointF pos, string id, Graphics graphics)
        {
            graphics.FillEllipse(Brushes.DarkRed, pos.X - 20, pos.Y - 20, 40, 40);
            SizeF labelSize = graphics.MeasureString(id, DefaultFont);

            graphics.DrawString(id, DefaultFont, Brushes.White, pos.X - labelSize.Width / 2,
                pos.Y - labelSize.Height / 2);
        }

        private void button_ChangeTrack_Click(object sender, EventArgs e)
        {
            if (this.listBox_CycleSessions.SelectedIndex > -1)
            {
                this.trackCycler.ChangeTrack(this.listBox_CycleSessions.SelectedIndex);
            }
        }

        private void button_RestartSession_Click(object sender, EventArgs e)
        {
            this.pluginManager.BroadcastChatMessage("/RESTART_SESSION");
        }

        private void button_NextSession_Click(object sender, EventArgs e)
        {
            this.pluginManager.BroadcastChatMessage("/NEXT_SESSION");
        }

        private void dataGridView_connections_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.dataGridView_connections.SelectedRows.Count == 1)
            {
                this.contextMenuStrip_driver.Show(Cursor.Position);
            }
        }

        private void sendChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.dataGridView_connections.SelectedRows.Count == 1)
            {
                this.pluginManager.RequestKickDriverById(((DriverInfo)this.dataGridView_connections.SelectedRows[0].DataBoundItem).CarId);
            }
        }

        private void kickDriverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.dataGridView_connections.SelectedRows.Count == 1)
            {
                GetStringForm form = new GetStringForm("Chat");
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    this.pluginManager.SendChatMessage(((DriverInfo)this.dataGridView_connections.SelectedRows[0].DataBoundItem).CarId, form.String);
                }
            }
        }

        private void button_SetLength_Click(object sender, EventArgs e)
        {
            if (this.currentSessionInfo != null)
            {
                RequestSetSession requestSetSession = this.currentSessionInfo.CreateSetSessionRequest();
                requestSetSession.Time = (uint)this.numericUpDown_Length.Value;
                requestSetSession.Laps = (uint)this.numericUpDown_Length.Value;
                this.pluginManager.RequestSetSession(requestSetSession);
            }
        }
    }
}