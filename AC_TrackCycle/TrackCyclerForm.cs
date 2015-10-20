using System;
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
using System.Threading;

namespace AC_TrackCycle
{
    public partial class TrackCyclerForm : Form
    {
        private readonly GuiLogWriter logWriter;
        private readonly AcServerPluginManager pluginManager;
        private readonly GuiTrackCyclePlugin trackCycler;
        private readonly Dictionary<byte, SolidBrush> carColors = new Dictionary<byte, SolidBrush>();
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

                string[] possibleFolders = new string[]
                {
                    Path.Combine(this.trackCycler.ServerFolder,"content", "tracks", msg.Track, msg.TrackConfig),
                    Path.Combine(Path.GetDirectoryName(this.trackCycler.ServerFolder.Substring(0, this.trackCycler.ServerFolder.Length - 1)),"content", "tracks", msg.Track, msg.TrackConfig),
                };

                foreach (string possibleFolder in possibleFolders)
                {
                    if (File.Exists(Path.Combine(possibleFolder, "map.png")) &&
                        File.Exists(Path.Combine(possibleFolder, "Data", "map.ini")))
                    {
                        this.trackMap = Bitmap.FromFile(Path.Combine(possibleFolder, "map.png"));

                        StreamReader reader = new StreamReader(Path.Combine(possibleFolder, "Data", "map.ini"));
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
                        break;
                    }
                }
                this.UpdatePositionGraph();
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
            List<DriverInfo> connectedDrivers = this.pluginManager.GetDriverInfos().Where(d => d.IsConnected).ToList();
            this.textBox_ConnectionCount.Text = connectedDrivers.Count + " driver(s) currently connected";
            this.dataGridView_connections.DataSource = connectedDrivers;
            foreach (DataGridViewRow row in this.dataGridView_connections.Rows)
            {
                row.DefaultCellStyle.BackColor = this.getCarColor(((DriverInfo)row.DataBoundItem).CarId).Color;
            }
        }

        private SolidBrush getCarColor(byte carId)
        {
            SolidBrush color;
            if (!this.carColors.TryGetValue(carId, out color))
            {
                color = new SolidBrush(getRandomColor(carId));
                this.carColors.Add(carId, color);
            }
            return color;
        }

        public static Color getRandomColor(int seed)
        {
            Random random = new Random(seed);
            AHSV hsv = new AHSV(255, random.Next(360), 0.5f + random.Next(50) / 100f, 0.5f + random.Next(50) / 100f);
            return hsv.ToColor();
        }

        public void UpdatePositionGraph()
        {
            if (this.tabControl1.SelectedTab == this.tabPage_PositionGraph)
            {
                Bitmap bitmap = new Bitmap(this.pictureBox_positionGraph.Width, this.pictureBox_positionGraph.Height);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.Clear(Color.White);
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                if (this.trackMap == null)
                {
                    float radius = Math.Min(bitmap.Width, bitmap.Height) * 0.9f / 2.0f;
                    PointF center = new PointF(bitmap.Width / 2.0f, bitmap.Height / 2.0f);
                    graphics.DrawEllipse(new Pen(Color.DarkGray, 10f), center.X - radius, center.Y - radius, radius * 2, radius * 2);

                    foreach (DriverInfo driver in this.pluginManager.GetDriverInfos().Where(d => d.IsConnected))
                    {
                        Point pos = new Point((int)(center.X + Math.Sin(driver.LastSplinePosition * Math.PI * 2) * radius),
                            (int)(center.Y - Math.Cos(driver.LastSplinePosition * Math.PI * 2) * radius));

                        DrawDriverPos(pos, driver.CarId, graphics);
                    }
                }
                else
                {
                    float screenRatio = (float)bitmap.Width / bitmap.Height;
                    float mapRatio = (float)trackMap.Width / trackMap.Height;
                    float mapScreenScale;
                    if (screenRatio > mapRatio)
                    {
                        mapScreenScale = (float)bitmap.Height / trackMap.Height * 0.95f;

                    }
                    else
                    {
                        mapScreenScale = (float)bitmap.Width / trackMap.Width * 0.95f;

                    }

                    float mapScreenOffsetX = (bitmap.Width - trackMap.Width * mapScreenScale) / 2;
                    float mapScreenOffsetY = (bitmap.Height - trackMap.Height * mapScreenScale) / 2;

                    graphics.DrawImage(this.trackMap, mapScreenOffsetX, mapScreenOffsetY, trackMap.Width * mapScreenScale, trackMap.Height * mapScreenScale);

                    foreach (DriverInfo driver in this.pluginManager.GetDriverInfos().Where(d => d.IsConnected))
                    {
                        Point pos = new Point(
                            (int)((driver.LastPosition.X + trackMapOffsetX) / trackMapScale * mapScreenScale + mapScreenOffsetX),
                            (int)((driver.LastPosition.Z + trackMapOffsetY) / trackMapScale * mapScreenScale + mapScreenOffsetY));
                        DrawDriverPos(pos, driver.CarId, graphics);
                    }
                }
                graphics.Dispose();
                this.pictureBox_positionGraph.Image = bitmap;
            }
        }

        private void DrawDriverPos(Point pos, byte carId, Graphics graphics)
        {
            graphics.FillEllipse(this.getCarColor(carId), pos.X - 10, pos.Y - 10, 20, 20);
            SizeF labelSize = graphics.MeasureString(carId.ToString(), DefaultFont);

            graphics.DrawString(carId.ToString(), DefaultFont, Brushes.White, pos.X - labelSize.Width / 2,
                pos.Y - labelSize.Height / 2);
        }

        private void button_ChangeTrack_Click(object sender, EventArgs e)
        {
            if (this.listBox_CycleSessions.SelectedIndex > -1)
            {
                int trackIndex = this.listBox_CycleSessions.SelectedIndex;
                ThreadPool.QueueUserWorkItem(o => this.trackCycler.ChangeTrack(trackIndex));
            }
        }

        private void button_RestartSession_Click(object sender, EventArgs e)
        {
            this.pluginManager.RestartSession();
        }

        private void button_NextSession_Click(object sender, EventArgs e)
        {
            this.pluginManager.NextSession();
        }

        private void dataGridView_connections_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && this.dataGridView_connections.SelectedRows.Count == 1 && e.RowIndex == this.dataGridView_connections.SelectedRows[0].Index)
            {
                this.contextMenuStrip_driver.Show(Cursor.Position);
            }
        }

        private void sendChatToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedTab == this.tabPage_PositionGraph)
            {
                UpdatePositionGraph();
            }
        }

        private void kickDriverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.dataGridView_connections.SelectedRows.Count == 1)
            {
                this.pluginManager.RequestKickDriverById(((DriverInfo)this.dataGridView_connections.SelectedRows[0].DataBoundItem).CarId);
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
                this.pluginManager.RestartSession();
            }
        }
    }
}