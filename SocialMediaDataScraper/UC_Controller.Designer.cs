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
            cb_commands = new ComboBox();
            btn_runCommand = new Button();
            btn_stopCommand = new Button();
            tabPage2 = new TabPage();
            gv_tasks = new DataGridView();
            tabPage1 = new TabPage();
            tabControl1 = new TabControl();
            tb_userPk = new TextBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gv_tasks).BeginInit();
            tabPage1.SuspendLayout();
            tabControl1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tb_username
            // 
            tb_username.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_username.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            tb_username.Location = new Point(3, 3);
            tb_username.Name = "tb_username";
            tb_username.ReadOnly = true;
            tb_username.Size = new Size(278, 25);
            tb_username.TabIndex = 0;
            tb_username.TabStop = false;
            // 
            // listBox
            // 
            listBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBox.Font = new Font("Consolas", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listBox.FormattingEnabled = true;
            listBox.Location = new Point(6, 6);
            listBox.Name = "listBox";
            listBox.Size = new Size(548, 148);
            listBox.TabIndex = 4;
            listBox.DoubleClick += listBox_DoubleClick;
            // 
            // cb_commands
            // 
            cb_commands.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cb_commands.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_commands.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cb_commands.FormattingEnabled = true;
            cb_commands.Location = new Point(3, 5);
            cb_commands.Name = "cb_commands";
            cb_commands.Size = new Size(452, 25);
            cb_commands.TabIndex = 2;
            // 
            // btn_runCommand
            // 
            btn_runCommand.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_runCommand.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_runCommand.Location = new Point(461, 4);
            btn_runCommand.Name = "btn_runCommand";
            btn_runCommand.Size = new Size(52, 26);
            btn_runCommand.TabIndex = 3;
            btn_runCommand.Text = "Start";
            btn_runCommand.UseVisualStyleBackColor = true;
            btn_runCommand.Click += btn_runCommand_Click;
            // 
            // btn_stopCommand
            // 
            btn_stopCommand.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_stopCommand.Enabled = false;
            btn_stopCommand.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_stopCommand.Location = new Point(519, 4);
            btn_stopCommand.Name = "btn_stopCommand";
            btn_stopCommand.Size = new Size(52, 26);
            btn_stopCommand.TabIndex = 3;
            btn_stopCommand.Text = "Stop";
            btn_stopCommand.UseVisualStyleBackColor = true;

            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(gv_tasks);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(560, 170);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Tasks";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // gv_tasks
            // 
            gv_tasks.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gv_tasks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gv_tasks.Location = new Point(5, 5);
            gv_tasks.Margin = new Padding(2);
            gv_tasks.Name = "gv_tasks";
            gv_tasks.RowTemplate.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gv_tasks.RowTemplate.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
            gv_tasks.Size = new Size(550, 160);
            gv_tasks.TabIndex = 3;
            gv_tasks.DoubleClick += gv_tasks_DoubleClick;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(listBox);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(560, 170);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Logs";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            tabControl1.Location = new Point(3, 75);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(568, 200);
            tabControl1.TabIndex = 5;
            // 
            // tb_userPk
            // 
            tb_userPk.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_userPk.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            tb_userPk.Location = new Point(287, 3);
            tb_userPk.Name = "tb_userPk";
            tb_userPk.ReadOnly = true;
            tb_userPk.Size = new Size(278, 25);
            tb_userPk.TabIndex = 0;
            tb_userPk.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(tb_username, 0, 0);
            tableLayoutPanel1.Controls.Add(tb_userPk, 1, 0);
            tableLayoutPanel1.Location = new Point(3, 36);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(568, 33);
            tableLayoutPanel1.TabIndex = 6;
            // 
            // UC_Controller
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Controls.Add(tabControl1);
            Controls.Add(btn_stopCommand);
            Controls.Add(btn_runCommand);
            Controls.Add(cb_commands);
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "UC_Controller";
            Size = new Size(574, 278);
            Load += UC_Controller_Load;
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gv_tasks).EndInit();
            tabPage1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private TextBox tb_username;
        private ListBox listBox;
        private ComboBox cb_commands;
        private Button btn_runCommand;
        private Button btn_stopCommand;
        private TabPage tabPage2;
        private TabPage tabPage1;
        private TabControl tabControl1;
        private DataGridView gv_tasks;
        private TextBox tb_userPk;
        private TableLayoutPanel tableLayoutPanel1;
    }
}
