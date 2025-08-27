namespace SocialMediaDataScraper
{
    partial class UC_Controller
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tb_username = new TextBox();
            listBox = new ListBox();
            btn_stop = new Button();
            cb_commands = new ComboBox();
            btn_start = new Button();
            btn_runCommand = new Button();
            btn_stopCommand = new Button();
            SuspendLayout();
            // 
            // tb_username
            // 
            tb_username.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            tb_username.Location = new Point(3, 5);
            tb_username.Name = "tb_username";
            tb_username.ReadOnly = true;
            tb_username.Size = new Size(162, 25);
            tb_username.TabIndex = 0;
            tb_username.TabStop = false;
            // 
            // listBox
            // 
            listBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBox.Font = new Font("Consolas", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listBox.FormattingEnabled = true;
            listBox.Location = new Point(3, 35);
            listBox.Name = "listBox";
            listBox.Size = new Size(784, 346);
            listBox.TabIndex = 4;
            listBox.DoubleClick += listBox_DoubleClick;
            // 
            // btn_stop
            // 
            btn_stop.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_stop.Location = new Point(237, 4);
            btn_stop.Name = "btn_stop";
            btn_stop.Size = new Size(60, 26);
            btn_stop.TabIndex = 1;
            btn_stop.Text = "Stop";
            btn_stop.UseVisualStyleBackColor = true;
            // 
            // cb_commands
            // 
            cb_commands.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cb_commands.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cb_commands.FormattingEnabled = true;
            cb_commands.Location = new Point(303, 5);
            cb_commands.Name = "cb_commands";
            cb_commands.Size = new Size(298, 25);
            cb_commands.TabIndex = 2;
            // 
            // btn_start
            // 
            btn_start.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_start.Location = new Point(171, 4);
            btn_start.Name = "btn_start";
            btn_start.Size = new Size(60, 26);
            btn_start.TabIndex = 0;
            btn_start.Text = "Start";
            btn_start.UseVisualStyleBackColor = true;
            btn_start.Click += btn_start_Click;
            // 
            // btn_runCommand
            // 
            btn_runCommand.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_runCommand.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_runCommand.Location = new Point(607, 4);
            btn_runCommand.Name = "btn_runCommand";
            btn_runCommand.Size = new Size(84, 26);
            btn_runCommand.TabIndex = 3;
            btn_runCommand.Text = "Run Action";
            btn_runCommand.UseVisualStyleBackColor = true;
            btn_runCommand.Click += btn_runCommand_Click;
            // 
            // btn_stopCommand
            // 
            btn_stopCommand.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_stopCommand.Enabled = false;
            btn_stopCommand.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_stopCommand.Location = new Point(697, 4);
            btn_stopCommand.Name = "btn_stopCommand";
            btn_stopCommand.Size = new Size(90, 26);
            btn_stopCommand.TabIndex = 3;
            btn_stopCommand.Text = "Stop Action";
            btn_stopCommand.UseVisualStyleBackColor = true;
            // 
            // UC_Controller
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(listBox);
            Controls.Add(tb_username);
            Controls.Add(btn_stop);
            Controls.Add(btn_stopCommand);
            Controls.Add(btn_runCommand);
            Controls.Add(btn_start);
            Controls.Add(cb_commands);
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "UC_Controller";
            Size = new Size(790, 382);
            Load += UC_Controller_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox tb_username;
        private ListBox listBox;
        private Button btn_stop;
        private ComboBox cb_commands;
        private Button btn_start;
        private Button btn_runCommand;
        private Button btn_stopCommand;
    }
}
