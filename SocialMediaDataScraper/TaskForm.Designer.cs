namespace SocialMediaDataScraper
{
    partial class TaskForm
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
            tableLayoutPanel1 = new TableLayoutPanel();
            cb_commands = new ComboBox();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // propertyGrid
            // 
            propertyGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            propertyGrid.BackColor = SystemColors.Control;
            propertyGrid.Location = new Point(10, 93);
            propertyGrid.Name = "propertyGrid";
            propertyGrid.PropertySort = PropertySort.NoSort;
            propertyGrid.Size = new Size(511, 323);
            propertyGrid.TabIndex = 0;
            propertyGrid.PropertyValueChanged += propertyGrid_PropertyValueChanged;
            // 
            // btn_save
            // 
            btn_save.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_save.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_save.Location = new Point(3, 3);
            btn_save.Name = "btn_save";
            btn_save.Size = new Size(248, 30);
            btn_save.TabIndex = 1;
            btn_save.Text = "Save Task";
            btn_save.UseVisualStyleBackColor = true;
            btn_save.Click += btn_run_Click;
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
            tableLayoutPanel1.Controls.Add(btn_save, 0, 0);
            tableLayoutPanel1.Controls.Add(btn_cancel, 1, 0);
            tableLayoutPanel1.Location = new Point(12, 12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(508, 37);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // cb_commands
            // 
            cb_commands.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cb_commands.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_commands.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cb_commands.FormattingEnabled = true;
            cb_commands.Location = new Point(10, 62);
            cb_commands.Name = "cb_commands";
            cb_commands.Size = new Size(511, 25);
            cb_commands.TabIndex = 3;
            cb_commands.SelectedIndexChanged += cb_commands_SelectedIndexChanged;
            // 
            // TaskForm
            // 
            AcceptButton = btn_save;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(532, 427);
            Controls.Add(cb_commands);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(propertyGrid);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "TaskForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Add Task";
 
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private PropertyGrid propertyGrid;
        private Button btn_save;
        private Button btn_cancel;
        private TableLayoutPanel tableLayoutPanel1;
        private ComboBox cb_commands;
    }
}