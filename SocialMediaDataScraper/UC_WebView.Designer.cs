namespace SocialMediaDataScraper
{
    partial class UC_WebView
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
            tb_addressBar = new TextBox();
            btn_refresh = new Button();
            btn_go = new Button();
            btn_devTool = new Button();
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            btn_zoomOut = new Button();
            btn_zoomIn = new Button();
            tb_status = new TextBox();
            ((System.ComponentModel.ISupportInitialize)webView).BeginInit();
            SuspendLayout();
            // 
            // tb_addressBar
            // 
            tb_addressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_addressBar.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            tb_addressBar.Location = new Point(3, 6);
            tb_addressBar.Name = "tb_addressBar";
            tb_addressBar.Size = new Size(337, 25);
            tb_addressBar.TabIndex = 0;
            // 
            // btn_refresh
            // 
            btn_refresh.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_refresh.Location = new Point(44, 37);
            btn_refresh.Name = "btn_refresh";
            btn_refresh.Size = new Size(35, 30);
            btn_refresh.TabIndex = 2;
            btn_refresh.Text = "R";
            btn_refresh.UseVisualStyleBackColor = true;
            btn_refresh.Click += btn_refresh_Click;
            // 
            // btn_go
            // 
            btn_go.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_go.Location = new Point(3, 37);
            btn_go.Name = "btn_go";
            btn_go.Size = new Size(35, 30);
            btn_go.TabIndex = 1;
            btn_go.Text = "G";
            btn_go.UseVisualStyleBackColor = true;
            btn_go.Click += btn_go_Click;
            // 
            // btn_devTool
            // 
            btn_devTool.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_devTool.Location = new Point(85, 37);
            btn_devTool.Name = "btn_devTool";
            btn_devTool.Size = new Size(35, 30);
            btn_devTool.TabIndex = 3;
            btn_devTool.Text = "T";
            btn_devTool.UseVisualStyleBackColor = true;
            btn_devTool.Click += btn_devTool_Click;
            // 
            // webView
            // 
            webView.AllowExternalDrop = true;
            webView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = Color.White;
            webView.Location = new Point(3, 73);
            webView.Name = "webView";
            webView.Size = new Size(337, 263);
            webView.TabIndex = 11;
            webView.ZoomFactor = 1D;
            // 
            // btn_zoomOut
            // 
            btn_zoomOut.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_zoomOut.Location = new Point(167, 37);
            btn_zoomOut.Name = "btn_zoomOut";
            btn_zoomOut.Size = new Size(35, 30);
            btn_zoomOut.TabIndex = 5;
            btn_zoomOut.Text = "-";
            btn_zoomOut.UseVisualStyleBackColor = true;
            btn_zoomOut.Click += btn_zoomOut_Click;
            // 
            // btn_zoomIn
            // 
            btn_zoomIn.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_zoomIn.Location = new Point(126, 37);
            btn_zoomIn.Name = "btn_zoomIn";
            btn_zoomIn.Size = new Size(35, 30);
            btn_zoomIn.TabIndex = 4;
            btn_zoomIn.Text = "+";
            btn_zoomIn.UseVisualStyleBackColor = true;
            btn_zoomIn.Click += btn_zoomIn_Click;
            // 
            // tb_status
            // 
            tb_status.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            tb_status.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            tb_status.Location = new Point(242, 40);
            tb_status.Name = "tb_status";
            tb_status.ReadOnly = true;
            tb_status.Size = new Size(98, 25);
            tb_status.TabIndex = 10;
            tb_status.TextAlign = HorizontalAlignment.Center;
            // 
            // UC_WebView
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tb_status);
            Controls.Add(btn_zoomIn);
            Controls.Add(btn_zoomOut);
            Controls.Add(webView);
            Controls.Add(tb_addressBar);
            Controls.Add(btn_refresh);
            Controls.Add(btn_go);
            Controls.Add(btn_devTool);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "UC_WebView";
            Size = new Size(343, 339);
            Load += UC_WebView_Load;
            ((System.ComponentModel.ISupportInitialize)webView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tb_addressBar;
        private Button btn_refresh;
        private Button btn_go;
        private Button btn_devTool;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private Button btn_zoomOut;
        private Button btn_zoomIn;
        private TextBox tb_status;
    }
}
