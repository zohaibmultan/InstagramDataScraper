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
            btn_save = new Button();
            btn_cancel = new Button();
            SuspendLayout();
            // 
            // propertyGrid
            // 
            propertyGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            propertyGrid.BackColor = SystemColors.Control;
            propertyGrid.Location = new Point(10, 48);
            propertyGrid.Name = "propertyGrid";
            propertyGrid.PropertySort = PropertySort.NoSort;
            propertyGrid.Size = new Size(511, 218);
            propertyGrid.TabIndex = 0;
            propertyGrid.PropertyValueChanged += propertyGrid_PropertyValueChanged;
            // 
            // btn_save
            // 
            btn_save.DialogResult = DialogResult.OK;
            btn_save.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_save.Location = new Point(10, 11);
            btn_save.Name = "btn_save";
            btn_save.Size = new Size(55, 30);
            btn_save.TabIndex = 1;
            btn_save.Text = "Save";
            btn_save.UseVisualStyleBackColor = true;
            btn_save.Click += btn_save_Click;
            // 
            // btn_cancel
            // 
            btn_cancel.DialogResult = DialogResult.Cancel;
            btn_cancel.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_cancel.Location = new Point(71, 11);
            btn_cancel.Name = "btn_cancel";
            btn_cancel.Size = new Size(55, 30);
            btn_cancel.TabIndex = 1;
            btn_cancel.Text = "Cancel";
            btn_cancel.UseVisualStyleBackColor = true;
            btn_cancel.Click += btn_cancel_Click;
            // 
            // PropertyForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(532, 277);
            Controls.Add(btn_cancel);
            Controls.Add(btn_save);
            Controls.Add(propertyGrid);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "PropertyForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PropertyForm";
            Shown += PropertyForm_Shown;
            ResumeLayout(false);
        }

        #endregion

        private PropertyGrid propertyGrid;
        private Button btn_save;
        private Button btn_cancel;
    }
}