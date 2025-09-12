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
            btn_start = new Button();
            btn_stop = new Button();
            btn_edit = new Button();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            tableLayoutPanel2 = new TableLayoutPanel();
            btn_taskSearch = new Button();
            label6 = new Label();
            filter_account = new CheckedListBox();
            label7 = new Label();
            label5 = new Label();
            filter_status = new CheckedListBox();
            filter_query = new CheckedListBox();
            btn_stopAll = new Button();
            btn_startAll = new Button();
            btn_taskAdd = new Button();
            btn_taskDelete = new Button();
            gv_tasks = new DataGridView();
            btn_taskEdit = new Button();
            btn_taskReload = new Button();
            btn_taskStart = new Button();
            tabPage3 = new TabPage();
            btn_saveSettings = new Button();
            tb_downloadStatus = new TextBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            label1 = new Label();
            tb_downlaodInterval = new NumericUpDown();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            tb_ipAddress = new TextBox();
            tb_instagrapiSession = new TextBox();
            tb_instagrapiApiUrl = new TextBox();
            ((System.ComponentModel.ISupportInitialize)gv_browsers).BeginInit();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gv_tasks).BeginInit();
            tabPage3.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
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
            gv_browsers.Size = new Size(1041, 401);
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
            // btn_start
            // 
            btn_start.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_start.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_start.Location = new Point(820, 5);
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
            btn_stop.Location = new Point(935, 5);
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
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1059, 474);
            tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btn_add);
            tabPage1.Controls.Add(btn_delete);
            tabPage1.Controls.Add(gv_browsers);
            tabPage1.Controls.Add(btn_stop);
            tabPage1.Controls.Add(btn_edit);
            tabPage1.Controls.Add(btn_start);
            tabPage1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1051, 444);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Accounts";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(tableLayoutPanel2);
            tabPage2.Controls.Add(btn_stopAll);
            tabPage2.Controls.Add(btn_startAll);
            tabPage2.Controls.Add(btn_taskAdd);
            tabPage2.Controls.Add(btn_taskDelete);
            tabPage2.Controls.Add(gv_tasks);
            tabPage2.Controls.Add(btn_taskEdit);
            tabPage2.Controls.Add(btn_taskReload);
            tabPage2.Controls.Add(btn_taskStart);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1051, 444);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Tasks";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(btn_taskSearch, 0, 6);
            tableLayoutPanel2.Controls.Add(label6, 0, 4);
            tableLayoutPanel2.Controls.Add(filter_account, 0, 5);
            tableLayoutPanel2.Controls.Add(label7, 0, 2);
            tableLayoutPanel2.Controls.Add(label5, 0, 0);
            tableLayoutPanel2.Controls.Add(filter_status, 0, 1);
            tableLayoutPanel2.Controls.Add(filter_query, 0, 3);
            tableLayoutPanel2.Location = new Point(843, 39);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 7;
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.Size = new Size(202, 399);
            tableLayoutPanel2.TabIndex = 15;
            // 
            // btn_taskSearch
            // 
            btn_taskSearch.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btn_taskSearch.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_taskSearch.Location = new Point(2, 369);
            btn_taskSearch.Margin = new Padding(2);
            btn_taskSearch.Name = "btn_taskSearch";
            btn_taskSearch.Size = new Size(198, 28);
            btn_taskSearch.TabIndex = 15;
            btn_taskSearch.Text = "Search";
            btn_taskSearch.UseVisualStyleBackColor = true;
            btn_taskSearch.Click += btn_taskSearch_Click;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(3, 271);
            label6.Name = "label6";
            label6.Size = new Size(111, 17);
            label6.TabIndex = 14;
            label6.Text = "Filter by Account";
            // 
            // filter_account
            // 
            filter_account.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            filter_account.FormattingEnabled = true;
            filter_account.Location = new Point(3, 291);
            filter_account.Name = "filter_account";
            filter_account.Size = new Size(196, 64);
            filter_account.TabIndex = 12;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.Location = new Point(3, 96);
            label7.Name = "label7";
            label7.Size = new Size(99, 17);
            label7.TabIndex = 14;
            label7.Text = "Filter by Query";
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(3, 0);
            label5.Name = "label5";
            label5.Size = new Size(99, 17);
            label5.TabIndex = 14;
            label5.Text = "Filter by Status";
            // 
            // filter_status
            // 
            filter_status.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            filter_status.FormattingEnabled = true;
            filter_status.Location = new Point(3, 20);
            filter_status.Name = "filter_status";
            filter_status.Size = new Size(196, 64);
            filter_status.TabIndex = 12;
            // 
            // filter_query
            // 
            filter_query.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            filter_query.FormattingEnabled = true;
            filter_query.Location = new Point(3, 116);
            filter_query.Name = "filter_query";
            filter_query.Size = new Size(196, 144);
            filter_query.TabIndex = 12;
            // 
            // btn_stopAll
            // 
            btn_stopAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_stopAll.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_stopAll.Location = new Point(964, 5);
            btn_stopAll.Margin = new Padding(2);
            btn_stopAll.Name = "btn_stopAll";
            btn_stopAll.Size = new Size(82, 28);
            btn_stopAll.TabIndex = 10;
            btn_stopAll.Text = "Stop All";
            btn_stopAll.UseVisualStyleBackColor = true;
            btn_stopAll.Click += btn_stopAll_Click;
            // 
            // btn_startAll
            // 
            btn_startAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_startAll.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_startAll.Location = new Point(878, 5);
            btn_startAll.Margin = new Padding(2);
            btn_startAll.Name = "btn_startAll";
            btn_startAll.Size = new Size(82, 28);
            btn_startAll.TabIndex = 11;
            btn_startAll.Text = "Start All";
            btn_startAll.UseVisualStyleBackColor = true;
            btn_startAll.Click += btn_startAll_Click;
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
            gv_tasks.Location = new Point(6, 38);
            gv_tasks.Margin = new Padding(2);
            gv_tasks.Name = "gv_tasks";
            gv_tasks.RowTemplate.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gv_tasks.RowTemplate.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
            gv_tasks.Size = new Size(832, 400);
            gv_tasks.TabIndex = 2;
            gv_tasks.DoubleClick += gv_tasks_DoubleClick;
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
            // btn_taskReload
            // 
            btn_taskReload.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_taskReload.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_taskReload.Location = new Point(705, 6);
            btn_taskReload.Margin = new Padding(2);
            btn_taskReload.Name = "btn_taskReload";
            btn_taskReload.Size = new Size(64, 28);
            btn_taskReload.TabIndex = 9;
            btn_taskReload.Text = "Reload";
            btn_taskReload.UseVisualStyleBackColor = true;
            btn_taskReload.Click += btn_taskReload_Click;
            // 
            // btn_taskStart
            // 
            btn_taskStart.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_taskStart.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_taskStart.Location = new Point(773, 6);
            btn_taskStart.Margin = new Padding(2);
            btn_taskStart.Name = "btn_taskStart";
            btn_taskStart.Size = new Size(101, 28);
            btn_taskStart.TabIndex = 9;
            btn_taskStart.Text = "Start Selected";
            btn_taskStart.UseVisualStyleBackColor = true;
            btn_taskStart.Click += btn_taskStart_Click;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(btn_saveSettings);
            tabPage3.Controls.Add(tb_downloadStatus);
            tabPage3.Controls.Add(tableLayoutPanel1);
            tabPage3.Location = new Point(4, 26);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(1051, 444);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Settings";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // btn_saveSettings
            // 
            btn_saveSettings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_saveSettings.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_saveSettings.Location = new Point(637, 76);
            btn_saveSettings.Margin = new Padding(2);
            btn_saveSettings.Name = "btn_saveSettings";
            btn_saveSettings.Size = new Size(82, 28);
            btn_saveSettings.TabIndex = 13;
            btn_saveSettings.Text = "Save";
            btn_saveSettings.UseVisualStyleBackColor = true;
            btn_saveSettings.Click += btn_saveSettings_Click;
            // 
            // tb_downloadStatus
            // 
            tb_downloadStatus.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            tb_downloadStatus.Location = new Point(3, 79);
            tb_downloadStatus.Name = "tb_downloadStatus";
            tb_downloadStatus.ReadOnly = true;
            tb_downloadStatus.Size = new Size(233, 25);
            tb_downloadStatus.TabIndex = 10;
            tb_downloadStatus.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(tb_downlaodInterval, 3, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(label3, 2, 0);
            tableLayoutPanel1.Controls.Add(label4, 2, 1);
            tableLayoutPanel1.Controls.Add(tb_ipAddress, 1, 0);
            tableLayoutPanel1.Controls.Add(tb_instagrapiSession, 1, 1);
            tableLayoutPanel1.Controls.Add(tb_instagrapiApiUrl, 3, 1);
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(719, 68);
            tableLayoutPanel1.TabIndex = 12;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(23, 7);
            label1.Name = "label1";
            label1.Size = new Size(115, 17);
            label1.TabIndex = 13;
            label1.Text = "Download API Url";
            // 
            // tb_downlaodInterval
            // 
            tb_downlaodInterval.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tb_downlaodInterval.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb_downlaodInterval.Location = new Point(496, 3);
            tb_downlaodInterval.Name = "tb_downlaodInterval";
            tb_downlaodInterval.Size = new Size(220, 25);
            tb_downlaodInterval.TabIndex = 11;
            tb_downlaodInterval.TextAlign = HorizontalAlignment.Center;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(3, 38);
            label2.Name = "label2";
            label2.Size = new Size(135, 17);
            label2.TabIndex = 13;
            label2.Text = "Instagrapi Session ID";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(370, 7);
            label3.Name = "label3";
            label3.Size = new Size(120, 17);
            label3.TabIndex = 13;
            label3.Text = "Download Interval";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(375, 38);
            label4.Name = "label4";
            label4.Size = new Size(115, 17);
            label4.TabIndex = 13;
            label4.Text = "Instagrapi API Url";
            // 
            // tb_ipAddress
            // 
            tb_ipAddress.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tb_ipAddress.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            tb_ipAddress.Location = new Point(144, 3);
            tb_ipAddress.Name = "tb_ipAddress";
            tb_ipAddress.Size = new Size(220, 25);
            tb_ipAddress.TabIndex = 10;
            tb_ipAddress.TabStop = false;
            // 
            // tb_instagrapiSession
            // 
            tb_instagrapiSession.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tb_instagrapiSession.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            tb_instagrapiSession.Location = new Point(144, 34);
            tb_instagrapiSession.Name = "tb_instagrapiSession";
            tb_instagrapiSession.Size = new Size(220, 25);
            tb_instagrapiSession.TabIndex = 10;
            tb_instagrapiSession.TabStop = false;
            // 
            // tb_instagrapiApiUrl
            // 
            tb_instagrapiApiUrl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tb_instagrapiApiUrl.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            tb_instagrapiApiUrl.Location = new Point(496, 34);
            tb_instagrapiApiUrl.Name = "tb_instagrapiApiUrl";
            tb_instagrapiApiUrl.Size = new Size(220, 25);
            tb_instagrapiApiUrl.TabIndex = 10;
            tb_instagrapiApiUrl.TabStop = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1083, 498);
            Controls.Add(tabControl1);
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
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gv_tasks).EndInit();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tb_downlaodInterval).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView gv_browsers;
        private Button btn_add;
        private Button btn_delete;
        private Button btn_start;
        private Button btn_stop;
        private Button btn_edit;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button btn_taskAdd;
        private Button btn_taskDelete;
        private DataGridView gv_tasks;
        private Button btn_taskEdit;
        private Button btn_taskStart;
        private TextBox tb_ipAddress;
        private NumericUpDown tb_downlaodInterval;
        private TextBox tb_downloadStatus;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox tb_instagrapiSession;
        private TextBox tb_instagrapiApiUrl;
        private TabPage tabPage3;
        private Button btn_saveSettings;
        private Button btn_taskReload;
        private Button btn_stopAll;
        private Button btn_startAll;
        private CheckedListBox filter_status;
        private CheckedListBox filter_query;
        private TableLayoutPanel tableLayoutPanel2;
        private Label label6;
        private CheckedListBox filter_account;
        private Label label7;
        private Label label5;
        private Button btn_taskSearch;
    }
}
