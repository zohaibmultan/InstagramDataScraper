namespace SocialMediaDataScraper
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            gv_browsers = new DataGridView();
            btn_add = new Button();
            btn_delete = new Button();
            btn_startAll = new Button();
            btn_stopAll = new Button();
            btn_start = new Button();
            btn_stop = new Button();
            btn_edit = new Button();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            btn_taskAdd = new Button();
            btn_taskDelete = new Button();
            gv_tasks = new DataGridView();
            btn_taskStop = new Button();
            btn_taskEdit = new Button();
            btn_taskStart = new Button();
            tb_downlaodInterval = new NumericUpDown();
            tb_ipAddress = new TextBox();
            tb_downloadStatus = new TextBox();
            ((System.ComponentModel.ISupportInitialize)gv_browsers).BeginInit();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gv_tasks).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tb_downlaodInterval).BeginInit();
            SuspendLayout();
            // 
            // gv_browsers
            // 
            gv_browsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gv_browsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gv_browsers.Location = new Point(5, 38);
            gv_browsers.Margin = new Padding(2);
            gv_browsers.Name = "gv_browsers";
            gv_browsers.RowTemplate.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gv_browsers.RowTemplate.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
            gv_browsers.Size = new Size(802, 342);
            gv_browsers.TabIndex = 0;
            gv_browsers.DoubleClick += gv_browsers_DoubleClick;
            // 
            // btn_add
            // 
            btn_add.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_add.Location = new Point(5, 5);
            btn_add.Margin = new Padding(2);
            btn_add.Name = "btn_add";
            btn_add.Size = new Size(75, 28);
            btn_add.TabIndex = 1;
            btn_add.Text = "Add";
            btn_add.UseVisualStyleBackColor = true;
            btn_add.Click += btn_add_Click;
            // 
            // btn_delete
            // 
            btn_delete.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_delete.Location = new Point(163, 5);
            btn_delete.Margin = new Padding(2);
            btn_delete.Name = "btn_delete";
            btn_delete.Size = new Size(75, 28);
            btn_delete.TabIndex = 1;
            btn_delete.Text = "Delete";
            btn_delete.UseVisualStyleBackColor = true;
            btn_delete.Click += btn_delete_Click;
            // 
            // btn_startAll
            // 
            btn_startAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_startAll.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_startAll.Location = new Point(639, 5);
            btn_startAll.Margin = new Padding(2);
            btn_startAll.Name = "btn_startAll";
            btn_startAll.Size = new Size(82, 28);
            btn_startAll.TabIndex = 1;
            btn_startAll.Text = "Start All";
            btn_startAll.UseVisualStyleBackColor = true;
            btn_startAll.Click += btn_startAll_Click;
            // 
            // btn_stopAll
            // 
            btn_stopAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_stopAll.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_stopAll.Location = new Point(725, 5);
            btn_stopAll.Margin = new Padding(2);
            btn_stopAll.Name = "btn_stopAll";
            btn_stopAll.Size = new Size(82, 28);
            btn_stopAll.TabIndex = 1;
            btn_stopAll.Text = "Stop All";
            btn_stopAll.UseVisualStyleBackColor = true;
            btn_stopAll.Click += btn_stopAll_Click;
            // 
            // btn_start
            // 
            btn_start.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_start.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_start.Location = new Point(409, 5);
            btn_start.Margin = new Padding(2);
            btn_start.Name = "btn_start";
            btn_start.Size = new Size(111, 28);
            btn_start.TabIndex = 1;
            btn_start.Text = "Start Selected";
            btn_start.UseVisualStyleBackColor = true;
            btn_start.Click += btn_start_Click;
            // 
            // btn_stop
            // 
            btn_stop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_stop.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_stop.Location = new Point(524, 5);
            btn_stop.Margin = new Padding(2);
            btn_stop.Name = "btn_stop";
            btn_stop.Size = new Size(111, 28);
            btn_stop.TabIndex = 1;
            btn_stop.Text = "Stop Selected";
            btn_stop.UseVisualStyleBackColor = true;
            btn_stop.Click += btn_stop_Click;
            // 
            // btn_edit
            // 
            btn_edit.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_edit.Location = new Point(84, 5);
            btn_edit.Margin = new Padding(2);
            btn_edit.Name = "btn_edit";
            btn_edit.Size = new Size(75, 28);
            btn_edit.TabIndex = 1;
            btn_edit.Text = "Edit";
            btn_edit.UseVisualStyleBackColor = true;
            btn_edit.Click += btn_edit_Click;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(820, 415);
            tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btn_add);
            tabPage1.Controls.Add(btn_delete);
            tabPage1.Controls.Add(gv_browsers);
            tabPage1.Controls.Add(btn_stop);
            tabPage1.Controls.Add(btn_edit);
            tabPage1.Controls.Add(btn_stopAll);
            tabPage1.Controls.Add(btn_startAll);
            tabPage1.Controls.Add(btn_start);
            tabPage1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(812, 385);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Accounts";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(btn_taskAdd);
            tabPage2.Controls.Add(btn_taskDelete);
            tabPage2.Controls.Add(gv_tasks);
            tabPage2.Controls.Add(btn_taskStop);
            tabPage2.Controls.Add(btn_taskEdit);
            tabPage2.Controls.Add(btn_taskStart);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(729, 385);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Tasks";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // btn_taskAdd
            // 
            btn_taskAdd.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_taskAdd.Location = new Point(5, 5);
            btn_taskAdd.Margin = new Padding(2);
            btn_taskAdd.Name = "btn_taskAdd";
            btn_taskAdd.Size = new Size(75, 28);
            btn_taskAdd.TabIndex = 3;
            btn_taskAdd.Text = "Add";
            btn_taskAdd.UseVisualStyleBackColor = true;
            btn_taskAdd.Click += btn_taskAdd_Click;
            // 
            // btn_taskDelete
            // 
            btn_taskDelete.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_taskDelete.Location = new Point(163, 5);
            btn_taskDelete.Margin = new Padding(2);
            btn_taskDelete.Name = "btn_taskDelete";
            btn_taskDelete.Size = new Size(75, 28);
            btn_taskDelete.TabIndex = 4;
            btn_taskDelete.Text = "Delete";
            btn_taskDelete.UseVisualStyleBackColor = true;
            btn_taskDelete.Click += btn_taskDelete_Click;
            // 
            // gv_tasks
            // 
            gv_tasks.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gv_tasks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gv_tasks.Location = new Point(5, 38);
            gv_tasks.Margin = new Padding(2);
            gv_tasks.Name = "gv_tasks";
            gv_tasks.RowTemplate.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gv_tasks.RowTemplate.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
            gv_tasks.Size = new Size(786, 315);
            gv_tasks.TabIndex = 2;
            gv_tasks.DoubleClick += gv_tasks_DoubleClick;
            // 
            // btn_taskStop
            // 
            btn_taskStop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_taskStop.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_taskStop.Location = new Point(680, 6);
            btn_taskStop.Margin = new Padding(2);
            btn_taskStop.Name = "btn_taskStop";
            btn_taskStop.Size = new Size(111, 28);
            btn_taskStop.TabIndex = 5;
            btn_taskStop.Text = "Stop Selected";
            btn_taskStop.UseVisualStyleBackColor = true;
            btn_taskStop.Click += btn_taskStop_Click;
            // 
            // btn_taskEdit
            // 
            btn_taskEdit.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_taskEdit.Location = new Point(84, 5);
            btn_taskEdit.Margin = new Padding(2);
            btn_taskEdit.Name = "btn_taskEdit";
            btn_taskEdit.Size = new Size(75, 28);
            btn_taskEdit.TabIndex = 6;
            btn_taskEdit.Text = "Edit";
            btn_taskEdit.UseVisualStyleBackColor = true;
            btn_taskEdit.Click += btn_taskEdit_Click;
            // 
            // btn_taskStart
            // 
            btn_taskStart.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_taskStart.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_taskStart.Location = new Point(565, 6);
            btn_taskStart.Margin = new Padding(2);
            btn_taskStart.Name = "btn_taskStart";
            btn_taskStart.Size = new Size(111, 28);
            btn_taskStart.TabIndex = 9;
            btn_taskStart.Text = "Start Selected";
            btn_taskStart.UseVisualStyleBackColor = true;
            btn_taskStart.Click += btn_taskStart_Click;
            // 
            // tb_downlaodInterval
            // 
            tb_downlaodInterval.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            tb_downlaodInterval.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb_downlaodInterval.Location = new Point(363, 433);
            tb_downlaodInterval.Name = "tb_downlaodInterval";
            tb_downlaodInterval.Size = new Size(108, 25);
            tb_downlaodInterval.TabIndex = 11;
            tb_downlaodInterval.TextAlign = HorizontalAlignment.Center;
            tb_downlaodInterval.ValueChanged += tb_downlaodInterval_ValueChanged;
            // 
            // tb_ipAddress
            // 
            tb_ipAddress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tb_ipAddress.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            tb_ipAddress.Location = new Point(12, 433);
            tb_ipAddress.Name = "tb_ipAddress";
            tb_ipAddress.Size = new Size(345, 25);
            tb_ipAddress.TabIndex = 10;
            tb_ipAddress.TabStop = false;
            tb_ipAddress.TextChanged += tb_ipAddress_TextChanged;
            // 
            // tb_downloadStatus
            // 
            tb_downloadStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            tb_downloadStatus.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            tb_downloadStatus.Location = new Point(477, 433);
            tb_downloadStatus.Name = "tb_downloadStatus";
            tb_downloadStatus.ReadOnly = true;
            tb_downloadStatus.Size = new Size(351, 25);
            tb_downloadStatus.TabIndex = 10;
            tb_downloadStatus.TabStop = false;
            tb_downloadStatus.TextChanged += tb_ipAddress_TextChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(844, 470);
            Controls.Add(tb_downlaodInterval);
            Controls.Add(tabControl1);
            Controls.Add(tb_downloadStatus);
            Controls.Add(tb_ipAddress);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Instagram Accounts";
            FormClosing += MainForm_FormClosing;
            Shown += MainForm_Shown;
            ((System.ComponentModel.ISupportInitialize)gv_browsers).EndInit();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gv_tasks).EndInit();
            ((System.ComponentModel.ISupportInitialize)tb_downlaodInterval).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView gv_browsers;
        private Button btn_add;
        private Button btn_delete;
        private Button btn_startAll;
        private Button btn_stopAll;
        private Button btn_start;
        private Button btn_stop;
        private Button btn_edit;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button btn_taskAdd;
        private Button btn_taskDelete;
        private DataGridView gv_tasks;
        private Button btn_taskStop;
        private Button btn_taskEdit;
        private Button btn_taskStart;
        private TextBox tb_ipAddress;
        private NumericUpDown tb_downlaodInterval;
        private TextBox tb_downloadStatus;
    }
}
