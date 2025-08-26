namespace SocialMediaDataScraper
{
    partial class BrowserForm
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
            splitContainer1 = new SplitContainer();
            uc_webView = new UC_WebView();
            btn_runCommand = new Button();
            btn_start = new Button();
            cb_commands = new ComboBox();
            btn_stop = new Button();
            listBox = new ListBox();
            tb_username = new TextBox();
            panel1 = new Panel();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(6, 6);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(uc_webView);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panel1);
            splitContainer1.Size = new Size(1115, 526);
            splitContainer1.SplitterDistance = 535;
            splitContainer1.TabIndex = 12;
            // 
            // uc_webView
            // 
            uc_webView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            uc_webView.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            uc_webView.Location = new Point(3, 3);
            uc_webView.Name = "uc_webView";
            uc_webView.Size = new Size(529, 520);
            uc_webView.TabIndex = 0;
            // 
            // btn_runCommand
            // 
            btn_runCommand.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_runCommand.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_runCommand.Location = new Point(480, 3);
            btn_runCommand.Name = "btn_runCommand";
            btn_runCommand.Size = new Size(87, 30);
            btn_runCommand.TabIndex = 5;
            btn_runCommand.Text = "Run Action";
            btn_runCommand.UseVisualStyleBackColor = true;
            btn_runCommand.Click += btn_runCommand_Click;
            // 
            // btn_start
            // 
            btn_start.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_start.Location = new Point(197, 3);
            btn_start.Name = "btn_start";
            btn_start.Size = new Size(52, 30);
            btn_start.TabIndex = 5;
            btn_start.Text = "Start";
            btn_start.UseVisualStyleBackColor = true;
            // 
            // cb_commands
            // 
            cb_commands.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cb_commands.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            cb_commands.FormattingEnabled = true;
            cb_commands.Location = new Point(313, 6);
            cb_commands.Name = "cb_commands";
            cb_commands.Size = new Size(161, 25);
            cb_commands.TabIndex = 8;
            // 
            // btn_stop
            // 
            btn_stop.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_stop.Location = new Point(255, 3);
            btn_stop.Name = "btn_stop";
            btn_stop.Size = new Size(52, 30);
            btn_stop.TabIndex = 4;
            btn_stop.Text = "Stop";
            btn_stop.UseVisualStyleBackColor = true;
            // 
            // listBox
            // 
            listBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBox.Font = new Font("Consolas", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listBox.FormattingEnabled = true;
            listBox.Location = new Point(3, 36);
            listBox.Name = "listBox";
            listBox.Size = new Size(564, 436);
            listBox.TabIndex = 3;
            listBox.DoubleClick += listBox_DoubleClick;
            // 
            // tb_username
            // 
            tb_username.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            tb_username.Location = new Point(3, 6);
            tb_username.Name = "tb_username";
            tb_username.ReadOnly = true;
            tb_username.Size = new Size(188, 25);
            tb_username.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(tb_username);
            panel1.Controls.Add(listBox);
            panel1.Controls.Add(btn_stop);
            panel1.Controls.Add(cb_commands);
            panel1.Controls.Add(btn_start);
            panel1.Controls.Add(btn_runCommand);
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(570, 520);
            panel1.TabIndex = 11;
            // 
            // BrowserForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1127, 544);
            Controls.Add(splitContainer1);
            Font = new Font("Segoe UI", 9.75F);
            Name = "BrowserForm";
            Padding = new Padding(3);
            Text = "BrowserForm";
            WindowState = FormWindowState.Maximized;
            Shown += BrowserForm_Shown;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private SplitContainer splitContainer1;
        private UC_WebView uc_webView;
        private Panel panel1;
        private TextBox tb_username;
        private ListBox listBox;
        private Button btn_stop;
        private ComboBox cb_commands;
        private Button btn_start;
        private Button btn_runCommand;
    }
}