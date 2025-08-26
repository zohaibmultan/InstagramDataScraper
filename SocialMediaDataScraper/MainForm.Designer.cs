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
            ((System.ComponentModel.ISupportInitialize)gv_browsers).BeginInit();
            SuspendLayout();
            // 
            // gv_browsers
            // 
            gv_browsers.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gv_browsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gv_browsers.Location = new Point(12, 54);
            gv_browsers.Name = "gv_browsers";
            gv_browsers.Size = new Size(872, 280);
            gv_browsers.TabIndex = 0;
            // 
            // btn_add
            // 
            btn_add.Location = new Point(12, 13);
            btn_add.Name = "btn_add";
            btn_add.Size = new Size(66, 35);
            btn_add.TabIndex = 1;
            btn_add.Text = "Add";
            btn_add.UseVisualStyleBackColor = true;
            btn_add.Click += btn_add_Click;
            // 
            // btn_delete
            // 
            btn_delete.Location = new Point(84, 13);
            btn_delete.Name = "btn_delete";
            btn_delete.Size = new Size(66, 35);
            btn_delete.TabIndex = 1;
            btn_delete.Text = "Delete";
            btn_delete.UseVisualStyleBackColor = true;
            btn_delete.Click += btn_delete_Click;
            // 
            // btn_startAll
            // 
            btn_startAll.Location = new Point(156, 12);
            btn_startAll.Name = "btn_startAll";
            btn_startAll.Size = new Size(75, 35);
            btn_startAll.TabIndex = 1;
            btn_startAll.Text = "Start All";
            btn_startAll.UseVisualStyleBackColor = true;
            // 
            // btn_stopAll
            // 
            btn_stopAll.Location = new Point(237, 12);
            btn_stopAll.Name = "btn_stopAll";
            btn_stopAll.Size = new Size(75, 35);
            btn_stopAll.TabIndex = 1;
            btn_stopAll.Text = "Stop All";
            btn_stopAll.UseVisualStyleBackColor = true;
            // 
            // btn_start
            // 
            btn_start.Location = new Point(318, 12);
            btn_start.Name = "btn_start";
            btn_start.Size = new Size(75, 35);
            btn_start.TabIndex = 1;
            btn_start.Text = "Start";
            btn_start.UseVisualStyleBackColor = true;
            btn_start.Click += btn_start_Click;
            // 
            // btn_stop
            // 
            btn_stop.Location = new Point(399, 12);
            btn_stop.Name = "btn_stop";
            btn_stop.Size = new Size(75, 35);
            btn_stop.TabIndex = 1;
            btn_stop.Text = "Stop";
            btn_stop.UseVisualStyleBackColor = true;
            btn_stop.Click += btn_stop_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(896, 495);
            Controls.Add(btn_delete);
            Controls.Add(btn_stop);
            Controls.Add(btn_stopAll);
            Controls.Add(btn_start);
            Controls.Add(btn_startAll);
            Controls.Add(btn_add);
            Controls.Add(gv_browsers);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            Name = "Form1";
            Text = "Form1";
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
    }
}
