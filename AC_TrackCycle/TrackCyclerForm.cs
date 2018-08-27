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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Configuration;

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
        private TrackMapControl trackMapControl;
        private List<string> whiteList;
        private string whiteListStoragePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "whitelist.txt");

        private int origBroadcastResultCount;

        public TrackCyclerForm()
        {
            this.InitializeComponent();
            this.trackMapControl = new TrackMapControl();
            this.trackMapControl.Dock = DockStyle.Fill;
            this.trackMapControl.Paint += trackMapControl_Paint;
            this.tabPage_PositionGraph.Controls.Add(this.trackMapControl);

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

            this.origBroadcastResultCount = this.trackCycler.BroadcastResults;
            if (origBroadcastResultCount <= 0)
            {
                origBroadcastResultCount = 10;
            }
            else
            {
                this.checkBox_BroadcastResults.Checked = true;
            }

            if (this.trackCycler.BroadcastIncidents < 0 || this.trackCycler.BroadcastIncidents > 2)
            {
                this.trackCycler.BroadcastIncidents = 0;
            }

            this.checkBox_BroadcastIncidents.CheckState = (CheckState)this.trackCycler.BroadcastIncidents;

            this.checkBox_BroadcastFastestLap.CheckState = (CheckState)this.trackCycler.BroadcastFastestLap;

            this.dataGridView_connections.AutoGenerateColumns = false;

            if (this.trackCycler.CreateServerWindow || this.trackCycler.HasAdditionalExes)
            {
                this.TopMost = true;
            }

            if(File.Exists(whiteListStoragePath))
            {
                string[] data = File.ReadAllLines(whiteListStoragePath);
                whiteList = data.ToList();
                listBox_whitelist.DataSource = whiteList;
               
            }
            else
            {
                FileStream f = File.Create(whiteListStoragePath);
                f.Close();
                whiteList = new List<string>();
            }
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            checkBox_enableWhiteList.Checked = (config.AppSettings.Settings["enable_white_list"].Value == "1" ? true : false);
         }

        private DateTime? sessionStart;

        public void SetSessionInfo(MsgSessionInfo msg)
        {
            if (msg != null)
            {
                this.currentSessionInfo = msg;

                sessionStart = DateTime.Now.AddMilliseconds(-msg.ElapsedMS);

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
                    Path.Combine(this.trackCycler.ServerDirectory,"content", "tracks", msg.Track, msg.TrackConfig),
                    Path.Combine(Path.GetDirectoryName(this.trackCycler.ServerDirectory.Substring(0, this.trackCycler.ServerDirectory.Length - 1)),"content", "tracks", msg.Track, msg.TrackConfig),
                };

                foreach (string possibleFolder in possibleFolders)
                {
                    if ((File.Exists(Path.Combine(possibleFolder, "map.png")) || File.Exists(Path.Combine(possibleFolder, "Data", "map.png"))) &&
                        File.Exists(Path.Combine(possibleFolder, "Data", "map.ini")))
                    {
                        if (File.Exists(Path.Combine(possibleFolder, "map.png")))
                        {
                            this.trackMap = Bitmap.FromFile(Path.Combine(possibleFolder, "map.png"));
                        }
                        else
                        {
                            this.trackMap = Bitmap.FromFile(Path.Combine(possibleFolder, "Data", "map.png"));
                        }

                        Bitmap copy = new Bitmap(this.trackMap.Width, this.trackMap.Height);
                        using (Graphics graphics = Graphics.FromImage(copy))
                        {
                            graphics.Clear(Color.White);
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                            ColorMatrix colorMatrix = new ColorMatrix(
                                   new float[][]
                                   {
                                      new float[] {-1, 0, 0, 0, 0},
                                      new float[] {0, -1, 0, 0, 0},
                                      new float[] {0, 0, -1, 0, 0},
                                      new float[] {0, 0, 0, 1, 0},
                                      new float[] {1, 1, 1, 0, 1}
                                   });

                            ImageAttributes attributes = new ImageAttributes();
                            attributes.SetColorMatrix(colorMatrix);

                            graphics.DrawImage(this.trackMap, new Rectangle(0, 0, this.trackMap.Width, this.trackMap.Height), 0, 0, this.trackMap.Width, this.trackMap.Height, GraphicsUnit.Pixel, attributes);
                        }
                        this.trackMap.Dispose();
                        this.trackMap = copy;

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

                this.textBox_chatlog.Text = string.Empty;
            }

            this.textBoxOutput.AppendText(message + Environment.NewLine);
            this.logLength++;

            if (message.StartsWith("CHAT ["))
            {
                this.textBox_chatlog.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "]: " + message.Substring(5) + Environment.NewLine);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.trackCycler.StopServer();
            this.logWriter.StopLoggingToFile();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.buttonStart.Enabled = false;
            this.pictureBox_logo.Image = Properties.Resources.AC_logo_small;
            this.trackCycler.StartServer();
        }

        private void buttonNextTrack_Click(object sender, EventArgs e)
        {
            this.buttonStart.Enabled = false;
            this.pictureBox_logo.Image = Properties.Resources.AC_logo_small;
            this.trackCycler.NextTrackAsync(true);
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
                e.Handled = true;
                this.pluginManager.BroadcastChatMessage(this.textBox_chat.Text);
                this.textBox_chatlog.AppendText("[Server]: " + this.textBox_chat.Text + Environment.NewLine);
                this.textBox_chat.Text = string.Empty;
            }
        }

        public void UpdateGui()
        {
            List<DriverInfo> connectedDrivers = this.pluginManager.GetDriverInfos().Where(d => d.IsConnected).ToList();
            if (checkBox_enableWhiteList.Checked)
            {
                foreach(var driver in connectedDrivers)
                {
                    if (!whiteList.Contains(driver.DriverGuid))
                    {
                        this.pluginManager.RequestKickDriverById(driver.CarId);
                    }
                }
            }
            this.textBox_ConnectionCount.Text = connectedDrivers.Count + " driver(s) currently connected";
            this.dataGridView_connections.DataSource = connectedDrivers;

        }
        public void OnClientLoadedG(MsgClientLoaded msg)
        {
            DriverInfo Driver = this.pluginManager.GetDriverInfos().Where(d => msg.CarId == d.CarId).First();
            listBox_LastConnectedGUID.Items.Add(Driver.DriverGuid + ": " + Driver.DriverName);
        }

        private SolidBrush getCarBrush(byte carId)
        {
            SolidBrush color;
            if (!this.carColors.TryGetValue(carId, out color))
            {
                color = new SolidBrush(getCarColor(carId));
                this.carColors.Add(carId, color);
            }
            return color;
        }

        public static Color getCarColor(byte carId)
        {
            AHSV hsv = new AHSV(255, (carId % 24) * 15, 0.9f - (carId / 24) * 0.25f, (carId % 3) * 0.09f + 0.45f);
            return hsv.ToColor();
        }

        public void UpdatePositionGraph()
        {
            this.trackMapControl.Invalidate();
        }

        private void DrawDriverPos(Point pos, byte carId, Graphics graphics)
        {
            graphics.FillEllipse(this.getCarBrush(carId), pos.X - 10, pos.Y - 10, 20, 20);
            SizeF labelSize = graphics.MeasureString(carId.ToString(), DefaultFont);

            graphics.DrawString(carId.ToString(), DefaultFont, Brushes.White, pos.X - labelSize.Width / 2,
                pos.Y - labelSize.Height / 2 + 1);
        }

        private void button_ChangeTrack_Click(object sender, EventArgs e)
        {
            if (this.listBox_CycleSessions.SelectedIndex > -1)
            {
                this.buttonStart.Enabled = false;
                this.pictureBox_logo.Image = Properties.Resources.AC_logo_small;
                int trackIndex = this.listBox_CycleSessions.SelectedIndex;
                ThreadPool.QueueUserWorkItem(o => this.trackCycler.ChangeTrack(trackIndex, true));
            }
        }

        public void TrackChanging()
        {
            this.buttonNextTrack.Enabled = false;
            this.button_ChangeTrack.Enabled = false;
        }

        public void TrackChanged()
        {
            this.buttonNextTrack.Enabled = this.trackCycler.HasCycle;
            this.button_ChangeTrack.Enabled = this.trackCycler.HasCycle;
        }

        private void button_RestartSession_Click(object sender, EventArgs e)
        {
            if (this.pluginManager.IsConnected)
            {
                this.pluginManager.AdminCommand("/restart_session");
                //this.pluginManager.RestartSession();
            }
        }

        private void button_NextSession_Click(object sender, EventArgs e)
        {
            if (this.pluginManager.IsConnected)
            {
                this.pluginManager.NextSession();
            }
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
                    this.textBox_chatlog.AppendText("[Server to " + ((DriverInfo)this.dataGridView_connections.SelectedRows[0].DataBoundItem).DriverName + "]: " + form.String + Environment.NewLine);
                }
            }
        }

        private void dataGridView_connections_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView dv = sender as DataGridView;

            if (e.ColumnIndex == -1 && e.RowIndex > -1)
            {
                DriverInfo driver = dv.Rows[e.RowIndex].DataBoundItem as DriverInfo;
                if (driver != null)
                {
                    e.Graphics.FillRectangle(getCarBrush(driver.CarId), e.CellBounds);
                    e.Handled = true;
                }
            }
        }

        private void checkBox_BroadcastResults_CheckedChanged(object sender, EventArgs e)
        {
            this.trackCycler.BroadcastResults = this.checkBox_BroadcastResults.Checked ? this.origBroadcastResultCount : 0;
        }

        private void checkBox_BroadcastIncidents_CheckStateChanged(object sender, EventArgs e)
        {
            this.trackCycler.BroadcastIncidents = (int)this.checkBox_BroadcastIncidents.CheckState;
        }

        private void checkBox_BroadcastFastestLap_CheckedChanged(object sender, EventArgs e)
        {
            this.trackCycler.BroadcastFastestLap = (int)this.checkBox_BroadcastFastestLap.CheckState;
        }

        private void trackMapControl_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            int width = e.ClipRectangle.Width;
            int height = e.ClipRectangle.Height;

            graphics.Clear(Color.White);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            if (this.trackMap == null)
            {
                float radius = Math.Min(width, height) * 0.9f / 2.0f;
                PointF center = new PointF(width / 2.0f, height / 2.0f);
                graphics.DrawEllipse(new Pen(Color.Black, 5f), center.X - radius, center.Y - radius, radius * 2, radius * 2);

                foreach (DriverInfo driver in this.pluginManager.GetDriverInfos().Where(d => d.IsConnected))
                {
                    Point pos = new Point((int)(center.X + Math.Sin(driver.LastSplinePosition * Math.PI * 2) * radius),
                        (int)(center.Y - Math.Cos(driver.LastSplinePosition * Math.PI * 2) * radius));

                    DrawDriverPos(pos, driver.CarId, graphics);
                }
            }
            else
            {
                float screenRatio = (float)width / height;
                float mapRatio = (float)trackMap.Width / trackMap.Height;
                float mapScreenScale;
                if (screenRatio > mapRatio)
                {
                    mapScreenScale = (float)height / trackMap.Height * 0.95f;
                }
                else
                {
                    mapScreenScale = (float)width / trackMap.Width * 0.95f;
                }

                float mapScreenOffsetX = (width - trackMap.Width * mapScreenScale) / 2;
                float mapScreenOffsetY = (height - trackMap.Height * mapScreenScale) / 2;

                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(this.trackMap, mapScreenOffsetX, mapScreenOffsetY, trackMap.Width * mapScreenScale, trackMap.Height * mapScreenScale);

                foreach (DriverInfo driver in this.pluginManager.GetDriverInfos().Where(d => d.IsConnected))
                {
                    Point pos = new Point(
                        (int)((driver.LastPosition.X + trackMapOffsetX) / trackMapScale * mapScreenScale + mapScreenOffsetX),
                        (int)((driver.LastPosition.Z + trackMapOffsetY) / trackMapScale * mapScreenScale + mapScreenOffsetY));
                    DrawDriverPos(pos, driver.CarId, graphics);
                }
            }
        }

        private void banDriverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.dataGridView_connections.SelectedRows.Count == 1)
            {
                this.pluginManager.AdminCommand("/ban_id " + ((DriverInfo)this.dataGridView_connections.SelectedRows[0].DataBoundItem).CarId);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.sessionStart != null)
            {
                TimeSpan time = DateTime.Now - this.sessionStart.Value;
                textBox_elapedTime.Text = ((time < TimeSpan.Zero) ? "-" : "") + time.ToString(@"hh\:mm\:ss");
            }
        }

        private void button_addToWhiteList_Click(object sender, EventArgs e)
        {
            if(textBox_driver_guid.Text != null && textBox_driver_guid.Text != "")
            {
                if (whiteList.Contains(textBox_driver_guid.Text))
                {
                    MessageBox.Show("Already in list");
                }
                else
                {
                    whiteList.Add(textBox_driver_guid.Text);
                    listBox_whitelist.DataSource = whiteList.ToList();
                    listBox_whitelist.Refresh();
                    if (File.Exists(whiteListStoragePath))
                    {
                        File.Delete(whiteListStoragePath);
                    }
                    FileStream f = File.Create(whiteListStoragePath);
                    var offset = 0;
                    foreach (var guid in whiteList)
                    {
                        byte[] toWrite = Encoding.ASCII.GetBytes(guid + Environment.NewLine);
                        f.Write(toWrite, 0, toWrite.Length);
                        offset += toWrite.Length;
                    }
                    f.Close();
                }
            }
        }

        private void listBox_whitelist_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox_driver_guid.Text = listBox_whitelist.SelectedItem.ToString();
        }

        private void button_removeFromWhiteList_Click(object sender, EventArgs e)
        {
            whiteList.Remove(listBox_whitelist.SelectedItem.ToString());
            listBox_whitelist.DataSource = whiteList.ToList();
            listBox_whitelist.Refresh();
            if (File.Exists(whiteListStoragePath))
            {
                File.Delete(whiteListStoragePath);
            }
            FileStream f = File.Create(whiteListStoragePath);
            var offset = 0;
            foreach (var guid in whiteList)
            {
                byte[] toWrite = Encoding.ASCII.GetBytes(guid + Environment.NewLine);
                f.Write(toWrite, 0, toWrite.Length);
                offset += toWrite.Length;
            }
            f.Close();
        }

        private void listBox_LastConnectedGUID_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox_driver_guid.Text = listBox_LastConnectedGUID.SelectedItem.ToString().Split(':').First().Replace(" ","");
        }

        private void checkBox_enableWhiteList_CheckedChanged(object sender, EventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["enable_white_list"].Value = (checkBox_enableWhiteList.Checked ? "1" : "0");
            config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
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
            if (this.currentSessionInfo != null && this.pluginManager.IsConnected)
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