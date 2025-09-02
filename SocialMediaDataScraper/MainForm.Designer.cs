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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            gv_browsers = new DataGridView();
            btn_add = new Button();
            btn_delete = new Button();
            btn_startAll = new Button();
            btn_stopAll = new Button();
            btn_start = new Button();
            btn_stop = new Button();
            btn_edit = new Button();
            ((System.ComponentModel.ISupportInitialize)gv_browsers).BeginInit();
            SuspendLayout();
            // 
            // gv_browsers
            // 
            gv_browsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gv_browsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            gv_browsers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            gv_browsers.Location = new Point(11, 44);
            gv_browsers.Margin = new Padding(2);
            gv_browsers.Name = "gv_browsers";
            gv_browsers.RowTemplate.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gv_browsers.RowTemplate.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
            gv_browsers.Size = new Size(1126, 503);
            gv_browsers.TabIndex = 0;
            gv_browsers.DoubleClick += gv_browsers_DoubleClick;
            // 
            // btn_add
            // 
            btn_add.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_add.Location = new Point(11, 11);
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
            btn_delete.Location = new Point(169, 11);
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
            btn_startAll.Location = new Point(969, 11);
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
            btn_stopAll.Location = new Point(1055, 11);
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
            btn_start.Location = new Point(739, 11);
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
            btn_stop.Location = new Point(854, 11);
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
            btn_edit.Location = new Point(90, 11);
            btn_edit.Margin = new Padding(2);
            btn_edit.Name = "btn_edit";
            btn_edit.Size = new Size(75, 28);
            btn_edit.TabIndex = 1;
            btn_edit.Text = "Edit";
            btn_edit.UseVisualStyleBackColor = true;
            btn_edit.Click += btn_edit_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1148, 558);
            Controls.Add(btn_delete);
            Controls.Add(btn_stop);
            Controls.Add(btn_stopAll);
            Controls.Add(btn_start);
            Controls.Add(btn_startAll);
            Controls.Add(btn_edit);
            Controls.Add(btn_add);
            Controls.Add(gv_browsers);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Instagram Accounts";
            Load += Form1_Load;
            Shown += Form1_Shown;
            ((System.ComponentModel.ISupportInitialize)gv_browsers).EndInit();
            ResumeLayout(false);
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
    }
}
