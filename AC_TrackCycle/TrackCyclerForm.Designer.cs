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
            this.textBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxOutput.Location = new System.Drawing.Point(12, 67);
            this.textBoxOutput.MaxLength = 327670000;
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ReadOnly = true;
            this.textBoxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxOutput.Size = new System.Drawing.Size(490, 411);
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
            // TrackCyclerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 601);
            this.Controls.Add(this.textBox_sessionInfo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_chat);
            this.Controls.Add(this.checkBoxCreateLogs);
            this.Controls.Add(this.checkBoxAutoChangeTrack);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxCurrentCycle);
            this.Controls.Add(this.buttonNextTrack);
            this.Controls.Add(this.textBoxOutput);
            this.Controls.Add(this.buttonStart);
            this.MinimumSize = new System.Drawing.Size(480, 240);
            this.Name = "TrackCyclerForm";
            this.ShowIcon = false;
            this.Text = "AC Track Cycle";
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
    }
}

