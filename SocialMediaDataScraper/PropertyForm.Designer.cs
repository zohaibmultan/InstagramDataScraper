namespace SocialMediaDataScraper
{
    partial class PropertyForm
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
            propertyGrid = new PropertyGrid();
            btn_run = new Button();
            btn_cancel = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // propertyGrid
            // 
            propertyGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            propertyGrid.BackColor = SystemColors.Control;
            propertyGrid.Location = new Point(10, 55);
            propertyGrid.Name = "propertyGrid";
            propertyGrid.PropertySort = PropertySort.NoSort;
            propertyGrid.Size = new Size(511, 361);
            propertyGrid.TabIndex = 0;
            propertyGrid.PropertyValueChanged += propertyGrid_PropertyValueChanged;
            // 
            // btn_run
            // 
            btn_run.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_run.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_run.Location = new Point(3, 3);
            btn_run.Name = "btn_run";
            btn_run.Size = new Size(248, 30);
            btn_run.TabIndex = 1;
            btn_run.Text = "Run Task";
            btn_run.UseVisualStyleBackColor = true;
            btn_run.Click += btn_run_Click;
            // 
            // btn_cancel
            // 
            btn_cancel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_cancel.DialogResult = DialogResult.Cancel;
            btn_cancel.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_cancel.Location = new Point(257, 3);
            btn_cancel.Name = "btn_cancel";
            btn_cancel.Size = new Size(248, 30);
            btn_cancel.TabIndex = 1;
            btn_cancel.Text = "Cancel";
            btn_cancel.UseVisualStyleBackColor = true;
            btn_cancel.Click += btn_cancel_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(btn_run, 0, 0);
            tableLayoutPanel1.Controls.Add(btn_cancel, 1, 0);
            tableLayoutPanel1.Location = new Point(12, 12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(508, 37);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // PropertyForm
            // 
            AcceptButton = btn_run;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(532, 427);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(propertyGrid);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "PropertyForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PropertyForm";
            Shown += PropertyForm_Shown;
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private PropertyGrid propertyGrid;
        private Button btn_run;
        private Button btn_cancel;
        private TableLayoutPanel tableLayoutPanel1;
    }
}