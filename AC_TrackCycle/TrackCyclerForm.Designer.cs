namespace AC_TrackCycle
{
    partial class TrackCyclerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonStart = new System.Windows.Forms.Button();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.buttonNextTrack = new System.Windows.Forms.Button();
            this.textBoxCurrentCycle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxAutoChangeTrack = new System.Windows.Forms.CheckBox();
            this.checkBoxCreateLogs = new System.Windows.Forms.CheckBox();
            this.textBox_chat = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_sessionInfo = new System.Windows.Forms.TextBox();
            this.tabPage_WhiteList = new System.Windows.Forms.TabControl();
            this.tabPage_ServerLog = new System.Windows.Forms.TabPage();
            this.tabPage_SessionControl = new System.Windows.Forms.TabPage();
            this.checkBox_BroadcastFastestLap = new System.Windows.Forms.CheckBox();
            this.checkBox_BroadcastIncidents = new System.Windows.Forms.CheckBox();
            this.checkBox_BroadcastResults = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDown_Length = new System.Windows.Forms.NumericUpDown();
            this.button_SetLength = new System.Windows.Forms.Button();
            this.button_NextSession = new System.Windows.Forms.Button();
            this.button_RestartSession = new System.Windows.Forms.Button();
            this.button_ChangeTrack = new System.Windows.Forms.Button();
            this.listBox_CycleSessions = new System.Windows.Forms.ListBox();
            this.tabPage_ConnectedDrivers = new System.Windows.Forms.TabPage();
            this.dataGridView_connections = new System.Windows.Forms.DataGridView();
            this.Column_CarId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_DriverName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_DriverGuid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_CarModel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_BestLap = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_LapCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage_chatlog = new System.Windows.Forms.TabPage();
            this.textBox_chatlog = new System.Windows.Forms.TextBox();
            this.tabPage_PositionGraph = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.listBox_whitelist = new System.Windows.Forms.ListBox();
            this.checkBox_enableWhiteList = new System.Windows.Forms.CheckBox();
            this.textBox_ConnectionCount = new System.Windows.Forms.TextBox();
            this.contextMenuStrip_driver = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sendChatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kickDriverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.banDriverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox_logo = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBox_elapedTime = new System.Windows.Forms.TextBox();
            this.tabPage_WhiteList.SuspendLayout();
            this.tabPage_ServerLog.SuspendLayout();
            this.tabPage_SessionControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Length)).BeginInit();
            this.tabPage_ConnectedDrivers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_connections)).BeginInit();
            this.tabPage_chatlog.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.contextMenuStrip_driver.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_logo)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonStart.Location = new System.Drawing.Point(219, 587);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxOutput.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxOutput.Location = new System.Drawing.Point(3, 3);
            this.textBoxOutput.MaxLength = 327670000;
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ReadOnly = true;
            this.textBoxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxOutput.Size = new System.Drawing.Size(473, 410);
            this.textBoxOutput.TabIndex = 1;
            this.textBoxOutput.WordWrap = false;
            // 
            // buttonNextTrack
            // 
            this.buttonNextTrack.Location = new System.Drawing.Point(11, 220);
            this.buttonNextTrack.Name = "buttonNextTrack";
            this.buttonNextTrack.Size = new System.Drawing.Size(177, 23);
            this.buttonNextTrack.TabIndex = 2;
            this.buttonNextTrack.Text = "Next Track";
            this.buttonNextTrack.UseVisualStyleBackColor = true;
            this.buttonNextTrack.Click += new System.EventHandler(this.buttonNextTrack_Click);
            // 
            // textBoxCurrentCycle
            // 
            this.textBoxCurrentCycle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCurrentCycle.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxCurrentCycle.Location = new System.Drawing.Point(107, 12);
            this.textBoxCurrentCycle.Name = "textBoxCurrentCycle";
            this.textBoxCurrentCycle.ReadOnly = true;
            this.textBoxCurrentCycle.Size = new System.Drawing.Size(395, 20);
            this.textBoxCurrentCycle.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Currently running:";
            // 
            // checkBoxAutoChangeTrack
            // 
            this.checkBoxAutoChangeTrack.AutoSize = true;
            this.checkBoxAutoChangeTrack.Checked = true;
            this.checkBoxAutoChangeTrack.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoChangeTrack.Location = new System.Drawing.Point(11, 249);
            this.checkBoxAutoChangeTrack.Name = "checkBoxAutoChangeTrack";
            this.checkBoxAutoChangeTrack.Size = new System.Drawing.Size(148, 17);
            this.checkBoxAutoChangeTrack.TabIndex = 5;
            this.checkBoxAutoChangeTrack.Text = "Change Track After Race";
            this.checkBoxAutoChangeTrack.UseVisualStyleBackColor = true;
            this.checkBoxAutoChangeTrack.CheckedChanged += new System.EventHandler(this.checkBoxAutoChangeTrack_CheckedChanged);
            // 
            // checkBoxCreateLogs
            // 
            this.checkBoxCreateLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxCreateLogs.AutoSize = true;
            this.checkBoxCreateLogs.Checked = true;
            this.checkBoxCreateLogs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCreateLogs.Location = new System.Drawing.Point(6, 417);
            this.checkBoxCreateLogs.Name = "checkBoxCreateLogs";
            this.checkBoxCreateLogs.Size = new System.Drawing.Size(95, 17);
            this.checkBoxCreateLogs.TabIndex = 6;
            this.checkBoxCreateLogs.Text = "Create log files";
            this.checkBoxCreateLogs.UseVisualStyleBackColor = true;
            this.checkBoxCreateLogs.CheckedChanged += new System.EventHandler(this.checkBoxCreateLogs_CheckedChanged);
            // 
            // textBox_chat
            // 
            this.textBox_chat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_chat.Location = new System.Drawing.Point(47, 560);
            this.textBox_chat.Name = "textBox_chat";
            this.textBox_chat.Size = new System.Drawing.Size(455, 20);
            this.textBox_chat.TabIndex = 0;
            this.textBox_chat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_chat_KeyPress);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 563);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Chat";
            // 
            // textBox_sessionInfo
            // 
            this.textBox_sessionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_sessionInfo.BackColor = System.Drawing.SystemColors.Window;
            this.textBox_sessionInfo.Location = new System.Drawing.Point(107, 38);
            this.textBox_sessionInfo.Name = "textBox_sessionInfo";
            this.textBox_sessionInfo.ReadOnly = true;
            this.textBox_sessionInfo.Size = new System.Drawing.Size(325, 20);
            this.textBox_sessionInfo.TabIndex = 9;
            // 
            // tabPage_WhiteList
            // 
            this.tabPage_WhiteList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabPage_WhiteList.Controls.Add(this.tabPage_ServerLog);
            this.tabPage_WhiteList.Controls.Add(this.tabPage_SessionControl);
            this.tabPage_WhiteList.Controls.Add(this.tabPage_ConnectedDrivers);
            this.tabPage_WhiteList.Controls.Add(this.tabPage_chatlog);
            this.tabPage_WhiteList.Controls.Add(this.tabPage_PositionGraph);
            this.tabPage_WhiteList.Controls.Add(this.tabPage1);
            this.tabPage_WhiteList.Location = new System.Drawing.Point(15, 90);
            this.tabPage_WhiteList.Name = "tabPage_WhiteList";
            this.tabPage_WhiteList.SelectedIndex = 0;
            this.tabPage_WhiteList.Size = new System.Drawing.Size(487, 463);
            this.tabPage_WhiteList.TabIndex = 10;
            this.tabPage_WhiteList.Tag = "";
            // 
            // tabPage_ServerLog
            // 
            this.tabPage_ServerLog.Controls.Add(this.textBoxOutput);
            this.tabPage_ServerLog.Controls.Add(this.checkBoxCreateLogs);
            this.tabPage_ServerLog.Location = new System.Drawing.Point(4, 22);
            this.tabPage_ServerLog.Name = "tabPage_ServerLog";
            this.tabPage_ServerLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_ServerLog.Size = new System.Drawing.Size(479, 437);
            this.tabPage_ServerLog.TabIndex = 0;
            this.tabPage_ServerLog.Text = "Server Log";
            this.tabPage_ServerLog.UseVisualStyleBackColor = true;
            // 
            // tabPage_SessionControl
            // 
            this.tabPage_SessionControl.Controls.Add(this.checkBox_BroadcastFastestLap);
            this.tabPage_SessionControl.Controls.Add(this.checkBox_BroadcastIncidents);
            this.tabPage_SessionControl.Controls.Add(this.checkBox_BroadcastResults);
            this.tabPage_SessionControl.Controls.Add(this.label3);
            this.tabPage_SessionControl.Controls.Add(this.numericUpDown_Length);
            this.tabPage_SessionControl.Controls.Add(this.button_SetLength);
            this.tabPage_SessionControl.Controls.Add(this.button_NextSession);
            this.tabPage_SessionControl.Controls.Add(this.button_RestartSession);
            this.tabPage_SessionControl.Controls.Add(this.button_ChangeTrack);
            this.tabPage_SessionControl.Controls.Add(this.listBox_CycleSessions);
            this.tabPage_SessionControl.Controls.Add(this.buttonNextTrack);
            this.tabPage_SessionControl.Controls.Add(this.checkBoxAutoChangeTrack);
            this.tabPage_SessionControl.Location = new System.Drawing.Point(4, 22);
            this.tabPage_SessionControl.Name = "tabPage_SessionControl";
            this.tabPage_SessionControl.Size = new System.Drawing.Size(479, 437);
            this.tabPage_SessionControl.TabIndex = 3;
            this.tabPage_SessionControl.Text = "Session Control";
            this.tabPage_SessionControl.UseVisualStyleBackColor = true;
            // 
            // checkBox_BroadcastFastestLap
            // 
            this.checkBox_BroadcastFastestLap.AutoSize = true;
            this.checkBox_BroadcastFastestLap.Location = new System.Drawing.Point(247, 226);
            this.checkBox_BroadcastFastestLap.Name = "checkBox_BroadcastFastestLap";
            this.checkBox_BroadcastFastestLap.Size = new System.Drawing.Size(132, 17);
            this.checkBox_BroadcastFastestLap.TabIndex = 16;
            this.checkBox_BroadcastFastestLap.Text = "Broadcast Fastest Lap";
            this.checkBox_BroadcastFastestLap.ThreeState = true;
            this.checkBox_BroadcastFastestLap.UseVisualStyleBackColor = true;
            this.checkBox_BroadcastFastestLap.CheckedChanged += new System.EventHandler(this.checkBox_BroadcastFastestLap_CheckedChanged);
            // 
            // checkBox_BroadcastIncidents
            // 
            this.checkBox_BroadcastIncidents.AutoSize = true;
            this.checkBox_BroadcastIncidents.Location = new System.Drawing.Point(247, 249);
            this.checkBox_BroadcastIncidents.Name = "checkBox_BroadcastIncidents";
            this.checkBox_BroadcastIncidents.Size = new System.Drawing.Size(120, 17);
            this.checkBox_BroadcastIncidents.TabIndex = 15;
            this.checkBox_BroadcastIncidents.Text = "Broadcast Incidents";
            this.checkBox_BroadcastIncidents.ThreeState = true;
            this.checkBox_BroadcastIncidents.UseVisualStyleBackColor = true;
            this.checkBox_BroadcastIncidents.CheckStateChanged += new System.EventHandler(this.checkBox_BroadcastIncidents_CheckStateChanged);
            // 
            // checkBox_BroadcastResults
            // 
            this.checkBox_BroadcastResults.AutoSize = true;
            this.checkBox_BroadcastResults.Location = new System.Drawing.Point(247, 203);
            this.checkBox_BroadcastResults.Name = "checkBox_BroadcastResults";
            this.checkBox_BroadcastResults.Size = new System.Drawing.Size(152, 17);
            this.checkBox_BroadcastResults.TabIndex = 14;
            this.checkBox_BroadcastResults.Text = "Broadcast Session Results";
            this.checkBox_BroadcastResults.UseVisualStyleBackColor = true;
            this.checkBox_BroadcastResults.CheckedChanged += new System.EventHandler(this.checkBox_BroadcastResults_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(299, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 26);
            this.label3.TabIndex = 12;
            this.label3.Text = "Minutes for Pract/Quali\r\nLaps for Race";
            // 
            // numericUpDown_Length
            // 
            this.numericUpDown_Length.Location = new System.Drawing.Point(247, 103);
            this.numericUpDown_Length.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_Length.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Length.Name = "numericUpDown_Length";
            this.numericUpDown_Length.Size = new System.Drawing.Size(46, 20);
            this.numericUpDown_Length.TabIndex = 13;
            this.numericUpDown_Length.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // button_SetLength
            // 
            this.button_SetLength.Location = new System.Drawing.Point(247, 129);
            this.button_SetLength.Name = "button_SetLength";
            this.button_SetLength.Size = new System.Drawing.Size(177, 35);
            this.button_SetLength.TabIndex = 4;
            this.button_SetLength.Text = "Set Length of Current Session \r\nand Restart";
            this.button_SetLength.UseVisualStyleBackColor = true;
            this.button_SetLength.Click += new System.EventHandler(this.button_SetLength_Click);
            // 
            // button_NextSession
            // 
            this.button_NextSession.Location = new System.Drawing.Point(247, 41);
            this.button_NextSession.Name = "button_NextSession";
            this.button_NextSession.Size = new System.Drawing.Size(177, 23);
            this.button_NextSession.TabIndex = 3;
            this.button_NextSession.Text = "Next Session";
            this.button_NextSession.UseVisualStyleBackColor = true;
            this.button_NextSession.Click += new System.EventHandler(this.button_NextSession_Click);
            // 
            // button_RestartSession
            // 
            this.button_RestartSession.Location = new System.Drawing.Point(247, 12);
            this.button_RestartSession.Name = "button_RestartSession";
            this.button_RestartSession.Size = new System.Drawing.Size(177, 23);
            this.button_RestartSession.TabIndex = 2;
            this.button_RestartSession.Text = "Restart Session";
            this.button_RestartSession.UseVisualStyleBackColor = true;
            this.button_RestartSession.Click += new System.EventHandler(this.button_RestartSession_Click);
            // 
            // button_ChangeTrack
            // 
            this.button_ChangeTrack.Location = new System.Drawing.Point(11, 191);
            this.button_ChangeTrack.Name = "button_ChangeTrack";
            this.button_ChangeTrack.Size = new System.Drawing.Size(177, 23);
            this.button_ChangeTrack.TabIndex = 1;
            this.button_ChangeTrack.Text = "Change to Selected Track";
            this.button_ChangeTrack.UseVisualStyleBackColor = true;
            this.button_ChangeTrack.Click += new System.EventHandler(this.button_ChangeTrack_Click);
            // 
            // listBox_CycleSessions
            // 
            this.listBox_CycleSessions.FormattingEnabled = true;
            this.listBox_CycleSessions.Location = new System.Drawing.Point(11, 12);
            this.listBox_CycleSessions.Name = "listBox_CycleSessions";
            this.listBox_CycleSessions.Size = new System.Drawing.Size(177, 173);
            this.listBox_CycleSessions.TabIndex = 0;
            // 
            // tabPage_ConnectedDrivers
            // 
            this.tabPage_ConnectedDrivers.Controls.Add(this.dataGridView_connections);
            this.tabPage_ConnectedDrivers.Location = new System.Drawing.Point(4, 22);
            this.tabPage_ConnectedDrivers.Name = "tabPage_ConnectedDrivers";
            this.tabPage_ConnectedDrivers.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_ConnectedDrivers.Size = new System.Drawing.Size(479, 437);
            this.tabPage_ConnectedDrivers.TabIndex = 1;
            this.tabPage_ConnectedDrivers.Text = "Connected Drivers";
            this.tabPage_ConnectedDrivers.UseVisualStyleBackColor = true;
            // 
            // dataGridView_connections
            // 
            this.dataGridView_connections.AllowUserToAddRows = false;
            this.dataGridView_connections.AllowUserToDeleteRows = false;
            this.dataGridView_connections.AllowUserToResizeRows = false;
            this.dataGridView_connections.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView_connections.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_connections.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column_CarId,
            this.Column_DriverName,
            this.Column_DriverGuid,
            this.Column_CarModel,
            this.Column_BestLap,
            this.Column_LapCount});
            this.dataGridView_connections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_connections.Location = new System.Drawing.Point(3, 3);
            this.dataGridView_connections.Name = "dataGridView_connections";
            this.dataGridView_connections.ReadOnly = true;
            this.dataGridView_connections.RowHeadersWidth = 24;
            this.dataGridView_connections.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView_connections.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_connections.Size = new System.Drawing.Size(473, 431);
            this.dataGridView_connections.TabIndex = 0;
            this.dataGridView_connections.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_connections_CellMouseDown);
            this.dataGridView_connections.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView_connections_CellPainting);
            // 
            // Column_CarId
            // 
            this.Column_CarId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Column_CarId.DataPropertyName = "CarId";
            this.Column_CarId.HeaderText = "CarId";
            this.Column_CarId.Name = "Column_CarId";
            this.Column_CarId.ReadOnly = true;
            this.Column_CarId.Width = 57;
            // 
            // Column_DriverName
            // 
            this.Column_DriverName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column_DriverName.DataPropertyName = "DriverName";
            this.Column_DriverName.HeaderText = "DriverName";
            this.Column_DriverName.Name = "Column_DriverName";
            this.Column_DriverName.ReadOnly = true;
            // 
            // Column_DriverGuid
            // 
            this.Column_DriverGuid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column_DriverGuid.DataPropertyName = "DriverGuid";
            this.Column_DriverGuid.HeaderText = "DriverGuid";
            this.Column_DriverGuid.Name = "Column_DriverGuid";
            this.Column_DriverGuid.ReadOnly = true;
            // 
            // Column_CarModel
            // 
            this.Column_CarModel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column_CarModel.DataPropertyName = "CarModel";
            this.Column_CarModel.HeaderText = "CarModel";
            this.Column_CarModel.Name = "Column_CarModel";
            this.Column_CarModel.ReadOnly = true;
            // 
            // Column_BestLap
            // 
            this.Column_BestLap.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column_BestLap.DataPropertyName = "BestLapTxt";
            this.Column_BestLap.HeaderText = "BestLap";
            this.Column_BestLap.Name = "Column_BestLap";
            this.Column_BestLap.ReadOnly = true;
            // 
            // Column_LapCount
            // 
            this.Column_LapCount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Column_LapCount.DataPropertyName = "LapCount";
            this.Column_LapCount.HeaderText = "Laps";
            this.Column_LapCount.Name = "Column_LapCount";
            this.Column_LapCount.ReadOnly = true;
            this.Column_LapCount.Width = 55;
            // 
            // tabPage_chatlog
            // 
            this.tabPage_chatlog.Controls.Add(this.textBox_chatlog);
            this.tabPage_chatlog.Location = new System.Drawing.Point(4, 22);
            this.tabPage_chatlog.Name = "tabPage_chatlog";
            this.tabPage_chatlog.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_chatlog.Size = new System.Drawing.Size(479, 437);
            this.tabPage_chatlog.TabIndex = 4;
            this.tabPage_chatlog.Text = "Chat Messages";
            this.tabPage_chatlog.UseVisualStyleBackColor = true;
            // 
            // textBox_chatlog
            // 
            this.textBox_chatlog.BackColor = System.Drawing.SystemColors.Window;
            this.textBox_chatlog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_chatlog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_chatlog.Location = new System.Drawing.Point(3, 3);
            this.textBox_chatlog.MaxLength = 327670000;
            this.textBox_chatlog.Multiline = true;
            this.textBox_chatlog.Name = "textBox_chatlog";
            this.textBox_chatlog.ReadOnly = true;
            this.textBox_chatlog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_chatlog.Size = new System.Drawing.Size(473, 431);
            this.textBox_chatlog.TabIndex = 2;
            this.textBox_chatlog.WordWrap = false;
            // 
            // tabPage_PositionGraph
            // 
            this.tabPage_PositionGraph.Location = new System.Drawing.Point(4, 22);
            this.tabPage_PositionGraph.Name = "tabPage_PositionGraph";
            this.tabPage_PositionGraph.Size = new System.Drawing.Size(479, 437);
            this.tabPage_PositionGraph.TabIndex = 2;
            this.tabPage_PositionGraph.Text = "Track Graph";
            this.tabPage_PositionGraph.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.listBox_whitelist);
            this.tabPage1.Controls.Add(this.checkBox_enableWhiteList);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(479, 437);
            this.tabPage1.TabIndex = 5;
            this.tabPage1.Text = "White list";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(398, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 20);
            this.button1.TabIndex = 3;
            this.button1.Text = "Add";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(114, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(284, 20);
            this.textBox1.TabIndex = 2;
            // 
            // listBox_whitelist
            // 
            this.listBox_whitelist.FormattingEnabled = true;
            this.listBox_whitelist.Location = new System.Drawing.Point(7, 31);
            this.listBox_whitelist.Name = "listBox_whitelist";
            this.listBox_whitelist.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBox_whitelist.Size = new System.Drawing.Size(466, 394);
            this.listBox_whitelist.TabIndex = 1;
            // 
            // checkBox_enableWhiteList
            // 
            this.checkBox_enableWhiteList.AutoSize = true;
            this.checkBox_enableWhiteList.Location = new System.Drawing.Point(7, 7);
            this.checkBox_enableWhiteList.Name = "checkBox_enableWhiteList";
            this.checkBox_enableWhiteList.Size = new System.Drawing.Size(101, 17);
            this.checkBox_enableWhiteList.TabIndex = 0;
            this.checkBox_enableWhiteList.Text = "enable white list";
            this.checkBox_enableWhiteList.UseVisualStyleBackColor = true;
            // 
            // textBox_ConnectionCount
            // 
            this.textBox_ConnectionCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_ConnectionCount.BackColor = System.Drawing.SystemColors.Window;
            this.textBox_ConnectionCount.Location = new System.Drawing.Point(107, 64);
            this.textBox_ConnectionCount.Name = "textBox_ConnectionCount";
            this.textBox_ConnectionCount.ReadOnly = true;
            this.textBox_ConnectionCount.Size = new System.Drawing.Size(395, 20);
            this.textBox_ConnectionCount.TabIndex = 11;
            // 
            // contextMenuStrip_driver
            // 
            this.contextMenuStrip_driver.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendChatToolStripMenuItem,
            this.kickDriverToolStripMenuItem,
            this.banDriverToolStripMenuItem});
            this.contextMenuStrip_driver.Name = "contextMenuStrip_driver";
            this.contextMenuStrip_driver.Size = new System.Drawing.Size(131, 70);
            // 
            // sendChatToolStripMenuItem
            // 
            this.sendChatToolStripMenuItem.Name = "sendChatToolStripMenuItem";
            this.sendChatToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.sendChatToolStripMenuItem.Text = "Send Chat";
            this.sendChatToolStripMenuItem.Click += new System.EventHandler(this.sendChatToolStripMenuItem_Click);
            // 
            // kickDriverToolStripMenuItem
            // 
            this.kickDriverToolStripMenuItem.Name = "kickDriverToolStripMenuItem";
            this.kickDriverToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.kickDriverToolStripMenuItem.Text = "Kick Driver";
            this.kickDriverToolStripMenuItem.Click += new System.EventHandler(this.kickDriverToolStripMenuItem_Click);
            // 
            // banDriverToolStripMenuItem
            // 
            this.banDriverToolStripMenuItem.Name = "banDriverToolStripMenuItem";
            this.banDriverToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.banDriverToolStripMenuItem.Text = "Ban Driver";
            this.banDriverToolStripMenuItem.Click += new System.EventHandler(this.banDriverToolStripMenuItem_Click);
            // 
            // pictureBox_logo
            // 
            this.pictureBox_logo.Image = global::AC_TrackCycle.Properties.Resources.AC_logo_small_grey;
            this.pictureBox_logo.Location = new System.Drawing.Point(25, 38);
            this.pictureBox_logo.Name = "pictureBox_logo";
            this.pictureBox_logo.Size = new System.Drawing.Size(50, 41);
            this.pictureBox_logo.TabIndex = 1;
            this.pictureBox_logo.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBox_elapedTime
            // 
            this.textBox_elapedTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_elapedTime.BackColor = System.Drawing.SystemColors.Window;
            this.textBox_elapedTime.Location = new System.Drawing.Point(438, 38);
            this.textBox_elapedTime.Name = "textBox_elapedTime";
            this.textBox_elapedTime.ReadOnly = true;
            this.textBox_elapedTime.Size = new System.Drawing.Size(64, 20);
            this.textBox_elapedTime.TabIndex = 12;
            this.textBox_elapedTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // TrackCyclerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 618);
            this.Controls.Add(this.textBox_elapedTime);
            this.Controls.Add(this.pictureBox_logo);
            this.Controls.Add(this.textBox_ConnectionCount);
            this.Controls.Add(this.tabPage_WhiteList);
            this.Controls.Add(this.textBox_sessionInfo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_chat);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxCurrentCycle);
            this.Controls.Add(this.buttonStart);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(490, 495);
            this.Name = "TrackCyclerForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "AC Track Cycle";
            this.tabPage_WhiteList.ResumeLayout(false);
            this.tabPage_ServerLog.ResumeLayout(false);
            this.tabPage_ServerLog.PerformLayout();
            this.tabPage_SessionControl.ResumeLayout(false);
            this.tabPage_SessionControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Length)).EndInit();
            this.tabPage_ConnectedDrivers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_connections)).EndInit();
            this.tabPage_chatlog.ResumeLayout(false);
            this.tabPage_chatlog.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.contextMenuStrip_driver.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_logo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.Button buttonNextTrack;
        private System.Windows.Forms.TextBox textBoxCurrentCycle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxAutoChangeTrack;
        private System.Windows.Forms.CheckBox checkBoxCreateLogs;
        private System.Windows.Forms.TextBox textBox_chat;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_sessionInfo;
        private System.Windows.Forms.TabControl tabPage_WhiteList;
        private System.Windows.Forms.TabPage tabPage_ServerLog;
        private System.Windows.Forms.TabPage tabPage_ConnectedDrivers;
        private System.Windows.Forms.DataGridView dataGridView_connections;
        private System.Windows.Forms.TextBox textBox_ConnectionCount;
        private System.Windows.Forms.TabPage tabPage_PositionGraph;
        private System.Windows.Forms.TabPage tabPage_SessionControl;
        private System.Windows.Forms.Button button_ChangeTrack;
        private System.Windows.Forms.ListBox listBox_CycleSessions;
        private System.Windows.Forms.Button button_NextSession;
        private System.Windows.Forms.Button button_RestartSession;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_driver;
        private System.Windows.Forms.ToolStripMenuItem sendChatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kickDriverToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown numericUpDown_Length;
        private System.Windows.Forms.Button button_SetLength;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBox_logo;
        private System.Windows.Forms.CheckBox checkBox_BroadcastIncidents;
        private System.Windows.Forms.CheckBox checkBox_BroadcastResults;
        private System.Windows.Forms.CheckBox checkBox_BroadcastFastestLap;
        private System.Windows.Forms.ToolStripMenuItem banDriverToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage_chatlog;
        private System.Windows.Forms.TextBox textBox_chatlog;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_CarId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_DriverName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_DriverGuid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_CarModel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_BestLap;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_LapCount;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox textBox_elapedTime;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ListBox listBox_whitelist;
        private System.Windows.Forms.CheckBox checkBox_enableWhiteList;
    }
}

