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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridView_connections = new System.Windows.Forms.DataGridView();
            this.textBox_ConnectionCount = new System.Windows.Forms.TextBox();
            this.Column_CarId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_DriverName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_CarModel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.pictureBox_positionGraph = new System.Windows.Forms.PictureBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.listBox_CycleSessions = new System.Windows.Forms.ListBox();
            this.button_ChangeTrack = new System.Windows.Forms.Button();
            this.button_RestartSession = new System.Windows.Forms.Button();
            this.button_NextSession = new System.Windows.Forms.Button();
            this.contextMenuStrip_driver = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sendChatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kickDriverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button_SetLength = new System.Windows.Forms.Button();
            this.numericUpDown_Length = new System.Windows.Forms.NumericUpDown();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_connections)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_positionGraph)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.contextMenuStrip_driver.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Length)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonStart.Location = new System.Drawing.Point(218, 566);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxOutput.Location = new System.Drawing.Point(3, 3);
            this.textBoxOutput.MaxLength = 327670000;
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ReadOnly = true;
            this.textBoxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxOutput.Size = new System.Drawing.Size(473, 372);
            this.textBoxOutput.TabIndex = 1;
            this.textBoxOutput.WordWrap = false;
            // 
            // buttonNextTrack
            // 
            this.buttonNextTrack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNextTrack.Location = new System.Drawing.Point(427, 566);
            this.buttonNextTrack.Name = "buttonNextTrack";
            this.buttonNextTrack.Size = new System.Drawing.Size(75, 23);
            this.buttonNextTrack.TabIndex = 2;
            this.buttonNextTrack.Text = "Next Track";
            this.buttonNextTrack.UseVisualStyleBackColor = true;
            this.buttonNextTrack.Click += new System.EventHandler(this.buttonNextTrack_Click);
            // 
            // textBoxCurrentCycle
            // 
            this.textBoxCurrentCycle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.checkBoxAutoChangeTrack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxAutoChangeTrack.AutoSize = true;
            this.checkBoxAutoChangeTrack.Checked = true;
            this.checkBoxAutoChangeTrack.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoChangeTrack.Location = new System.Drawing.Point(12, 570);
            this.checkBoxAutoChangeTrack.Name = "checkBoxAutoChangeTrack";
            this.checkBoxAutoChangeTrack.Size = new System.Drawing.Size(138, 17);
            this.checkBoxAutoChangeTrack.TabIndex = 5;
            this.checkBoxAutoChangeTrack.Text = "Change track after race";
            this.checkBoxAutoChangeTrack.UseVisualStyleBackColor = true;
            this.checkBoxAutoChangeTrack.CheckedChanged += new System.EventHandler(this.checkBoxAutoChangeTrack_CheckedChanged);
            // 
            // checkBoxCreateLogs
            // 
            this.checkBoxCreateLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxCreateLogs.AutoSize = true;
            this.checkBoxCreateLogs.Checked = true;
            this.checkBoxCreateLogs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCreateLogs.Location = new System.Drawing.Point(12, 547);
            this.checkBoxCreateLogs.Name = "checkBoxCreateLogs";
            this.checkBoxCreateLogs.Size = new System.Drawing.Size(79, 17);
            this.checkBoxCreateLogs.TabIndex = 6;
            this.checkBoxCreateLogs.Text = "Create logs";
            this.checkBoxCreateLogs.UseVisualStyleBackColor = true;
            this.checkBoxCreateLogs.CheckedChanged += new System.EventHandler(this.checkBoxCreateLogs_CheckedChanged);
            // 
            // textBox_chat
            // 
            this.textBox_chat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_chat.Location = new System.Drawing.Point(47, 500);
            this.textBox_chat.Name = "textBox_chat";
            this.textBox_chat.Size = new System.Drawing.Size(455, 20);
            this.textBox_chat.TabIndex = 7;
            this.textBox_chat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_chat_KeyPress);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 503);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Chat";
            // 
            // textBox_sessionInfo
            // 
            this.textBox_sessionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_sessionInfo.Location = new System.Drawing.Point(107, 38);
            this.textBox_sessionInfo.Name = "textBox_sessionInfo";
            this.textBox_sessionInfo.ReadOnly = true;
            this.textBox_sessionInfo.Size = new System.Drawing.Size(395, 20);
            this.textBox_sessionInfo.TabIndex = 9;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(15, 90);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(487, 404);
            this.tabControl1.TabIndex = 10;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBoxOutput);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(479, 378);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Server Log";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridView_connections);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(479, 378);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Connected Drivers";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView_connections
            // 
            this.dataGridView_connections.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_connections.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column_CarId,
            this.Column_DriverName,
            this.Column_CarModel});
            this.dataGridView_connections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_connections.Location = new System.Drawing.Point(3, 3);
            this.dataGridView_connections.Name = "dataGridView_connections";
            this.dataGridView_connections.ReadOnly = true;
            this.dataGridView_connections.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_connections.Size = new System.Drawing.Size(473, 372);
            this.dataGridView_connections.TabIndex = 0;
            this.dataGridView_connections.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_connections_CellMouseDown);
            // 
            // textBox_ConnectionCount
            // 
            this.textBox_ConnectionCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_ConnectionCount.Location = new System.Drawing.Point(107, 64);
            this.textBox_ConnectionCount.Name = "textBox_ConnectionCount";
            this.textBox_ConnectionCount.ReadOnly = true;
            this.textBox_ConnectionCount.Size = new System.Drawing.Size(395, 20);
            this.textBox_ConnectionCount.TabIndex = 11;
            // 
            // Column_CarId
            // 
            this.Column_CarId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
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
            // Column_CarModel
            // 
            this.Column_CarModel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column_CarModel.DataPropertyName = "CarModel";
            this.Column_CarModel.HeaderText = "CarModel";
            this.Column_CarModel.Name = "Column_CarModel";
            this.Column_CarModel.ReadOnly = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.pictureBox_positionGraph);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(479, 378);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Position Graph";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // pictureBox_positionGraph
            // 
            this.pictureBox_positionGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_positionGraph.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_positionGraph.Name = "pictureBox_positionGraph";
            this.pictureBox_positionGraph.Size = new System.Drawing.Size(479, 378);
            this.pictureBox_positionGraph.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_positionGraph.TabIndex = 3;
            this.pictureBox_positionGraph.TabStop = false;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.numericUpDown_Length);
            this.tabPage4.Controls.Add(this.button_SetLength);
            this.tabPage4.Controls.Add(this.button_NextSession);
            this.tabPage4.Controls.Add(this.button_RestartSession);
            this.tabPage4.Controls.Add(this.button_ChangeTrack);
            this.tabPage4.Controls.Add(this.listBox_CycleSessions);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(479, 378);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Session Control";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // listBox_CycleSessions
            // 
            this.listBox_CycleSessions.FormattingEnabled = true;
            this.listBox_CycleSessions.Location = new System.Drawing.Point(11, 12);
            this.listBox_CycleSessions.Name = "listBox_CycleSessions";
            this.listBox_CycleSessions.Size = new System.Drawing.Size(177, 160);
            this.listBox_CycleSessions.TabIndex = 0;
            // 
            // button_ChangeTrack
            // 
            this.button_ChangeTrack.Location = new System.Drawing.Point(40, 178);
            this.button_ChangeTrack.Name = "button_ChangeTrack";
            this.button_ChangeTrack.Size = new System.Drawing.Size(110, 23);
            this.button_ChangeTrack.TabIndex = 1;
            this.button_ChangeTrack.Text = "Change Track";
            this.button_ChangeTrack.UseVisualStyleBackColor = true;
            this.button_ChangeTrack.Click += new System.EventHandler(this.button_ChangeTrack_Click);
            // 
            // button_RestartSession
            // 
            this.button_RestartSession.Location = new System.Drawing.Point(258, 69);
            this.button_RestartSession.Name = "button_RestartSession";
            this.button_RestartSession.Size = new System.Drawing.Size(142, 23);
            this.button_RestartSession.TabIndex = 2;
            this.button_RestartSession.Text = "Restart Session";
            this.button_RestartSession.UseVisualStyleBackColor = true;
            this.button_RestartSession.Click += new System.EventHandler(this.button_RestartSession_Click);
            // 
            // button_NextSession
            // 
            this.button_NextSession.Location = new System.Drawing.Point(258, 98);
            this.button_NextSession.Name = "button_NextSession";
            this.button_NextSession.Size = new System.Drawing.Size(142, 23);
            this.button_NextSession.TabIndex = 3;
            this.button_NextSession.Text = "Next Session";
            this.button_NextSession.UseVisualStyleBackColor = true;
            this.button_NextSession.Click += new System.EventHandler(this.button_NextSession_Click);
            // 
            // contextMenuStrip_driver
            // 
            this.contextMenuStrip_driver.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendChatToolStripMenuItem,
            this.kickDriverToolStripMenuItem});
            this.contextMenuStrip_driver.Name = "contextMenuStrip_driver";
            this.contextMenuStrip_driver.Size = new System.Drawing.Size(131, 48);
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
            // button_SetLength
            // 
            this.button_SetLength.Location = new System.Drawing.Point(242, 244);
            this.button_SetLength.Name = "button_SetLength";
            this.button_SetLength.Size = new System.Drawing.Size(172, 23);
            this.button_SetLength.TabIndex = 4;
            this.button_SetLength.Text = "Set Length of Current Session";
            this.button_SetLength.UseVisualStyleBackColor = true;
            this.button_SetLength.Click += new System.EventHandler(this.button_SetLength_Click);
            // 
            // numericUpDown_Length
            // 
            this.numericUpDown_Length.Location = new System.Drawing.Point(303, 218);
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
            this.numericUpDown_Length.TabIndex = 5;
            this.numericUpDown_Length.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // TrackCyclerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 601);
            this.Controls.Add(this.textBox_ConnectionCount);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.textBox_sessionInfo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_chat);
            this.Controls.Add(this.checkBoxCreateLogs);
            this.Controls.Add(this.checkBoxAutoChangeTrack);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxCurrentCycle);
            this.Controls.Add(this.buttonNextTrack);
            this.Controls.Add(this.buttonStart);
            this.MinimumSize = new System.Drawing.Size(480, 240);
            this.Name = "TrackCyclerForm";
            this.ShowIcon = false;
            this.Text = "AC Track Cycle";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_connections)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_positionGraph)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.contextMenuStrip_driver.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Length)).EndInit();
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
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView_connections;
        private System.Windows.Forms.TextBox textBox_ConnectionCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_CarId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_DriverName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_CarModel;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.PictureBox pictureBox_positionGraph;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button button_ChangeTrack;
        private System.Windows.Forms.ListBox listBox_CycleSessions;
        private System.Windows.Forms.Button button_NextSession;
        private System.Windows.Forms.Button button_RestartSession;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_driver;
        private System.Windows.Forms.ToolStripMenuItem sendChatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kickDriverToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown numericUpDown_Length;
        private System.Windows.Forms.Button button_SetLength;
    }
}

