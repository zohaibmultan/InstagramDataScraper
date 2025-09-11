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
            tb_taskResult = new TableLayoutPanel();
            btn_taskResult = new Button();
            cb_commands = new ComboBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            label1 = new Label();
            label2 = new Label();
            tb_logs = new RichTextBox();
            label4 = new Label();
            label3 = new Label();
            cb_status = new ComboBox();
            cb_doneBy = new ComboBox();
            label5 = new Label();
            label6 = new Label();
            tb_DoneAt = new TextBox();
            tb_taskResult.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // propertyGrid
            // 
            propertyGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            propertyGrid.BackColor = SystemColors.Control;
            propertyGrid.Location = new Point(3, 140);
            propertyGrid.Name = "propertyGrid";
            propertyGrid.PropertySort = PropertySort.NoSort;
            propertyGrid.Size = new Size(414, 309);
            propertyGrid.TabIndex = 4;
            propertyGrid.PropertyValueChanged += propertyGrid_PropertyValueChanged;
            // 
            // btn_save
            // 
            btn_save.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_save.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_save.Location = new Point(3, 3);
            btn_save.Name = "btn_save";
            btn_save.Size = new Size(144, 30);
            btn_save.TabIndex = 1;
            btn_save.Text = "Save Task";
            btn_save.UseVisualStyleBackColor = true;
            btn_save.Click += btn_save_Click;
            // 
            // btn_cancel
            // 
            btn_cancel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_cancel.DialogResult = DialogResult.Cancel;
            btn_cancel.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_cancel.Location = new Point(153, 3);
            btn_cancel.Name = "btn_cancel";
            btn_cancel.Size = new Size(144, 30);
            btn_cancel.TabIndex = 6;
            btn_cancel.Text = "Cancel";
            btn_cancel.UseVisualStyleBackColor = true;
            btn_cancel.Click += btn_cancel_Click;
            // 
            // tb_taskResult
            // 
            tb_taskResult.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_taskResult.ColumnCount = 4;
            tb_taskResult.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tb_taskResult.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tb_taskResult.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tb_taskResult.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tb_taskResult.Controls.Add(btn_save, 0, 0);
            tb_taskResult.Controls.Add(btn_cancel, 1, 0);
            tb_taskResult.Controls.Add(btn_taskResult, 3, 0);
            tb_taskResult.Location = new Point(10, 12);
            tb_taskResult.Name = "tb_taskResult";
            tb_taskResult.RowCount = 1;
            tb_taskResult.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tb_taskResult.Size = new Size(841, 37);
            tb_taskResult.TabIndex = 2;
            // 
            // btn_taskResult
            // 
            btn_taskResult.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_taskResult.DialogResult = DialogResult.Cancel;
            btn_taskResult.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btn_taskResult.Location = new Point(694, 3);
            btn_taskResult.Name = "btn_taskResult";
            btn_taskResult.Size = new Size(144, 30);
            btn_taskResult.TabIndex = 7;
            btn_taskResult.Text = "View Task Result";
            btn_taskResult.UseVisualStyleBackColor = true;
            btn_taskResult.Click += btn_taskResult_Click;
            // 
            // cb_commands
            // 
            cb_commands.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cb_commands.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_commands.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cb_commands.FormattingEnabled = true;
            cb_commands.Location = new Point(3, 28);
            cb_commands.Name = "cb_commands";
            cb_commands.Size = new Size(414, 25);
            cb_commands.TabIndex = 0;
            cb_commands.SelectedIndexChanged += cb_commands_SelectedIndexChanged;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(label1, 1, 4);
            tableLayoutPanel2.Controls.Add(label2, 0, 0);
            tableLayoutPanel2.Controls.Add(tb_logs, 1, 5);
            tableLayoutPanel2.Controls.Add(cb_commands, 0, 1);
            tableLayoutPanel2.Controls.Add(label4, 0, 4);
            tableLayoutPanel2.Controls.Add(label3, 1, 0);
            tableLayoutPanel2.Controls.Add(propertyGrid, 0, 5);
            tableLayoutPanel2.Controls.Add(cb_status, 1, 1);
            tableLayoutPanel2.Controls.Add(cb_doneBy, 0, 3);
            tableLayoutPanel2.Controls.Add(label5, 1, 2);
            tableLayoutPanel2.Controls.Add(label6, 0, 2);
            tableLayoutPanel2.Controls.Add(tb_DoneAt, 1, 3);
            tableLayoutPanel2.Location = new Point(10, 55);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 6;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.Size = new Size(841, 452);
            tableLayoutPanel2.TabIndex = 4;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(423, 120);
            label1.Name = "label1";
            label1.Size = new Size(60, 17);
            label1.TabIndex = 5;
            label1.Text = "Task Log";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(3, 8);
            label2.Name = "label2";
            label2.Size = new Size(66, 17);
            label2.TabIndex = 5;
            label2.Text = "Task Type";
            // 
            // tb_logs
            // 
            tb_logs.AcceptsTab = true;
            tb_logs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tb_logs.Location = new Point(423, 140);
            tb_logs.Name = "tb_logs";
            tb_logs.ReadOnly = true;
            tb_logs.Size = new Size(415, 309);
            tb_logs.TabIndex = 5;
            tb_logs.Text = "";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(3, 120);
            label4.Name = "label4";
            label4.Size = new Size(76, 17);
            label4.TabIndex = 5;
            label4.Text = "Task Query";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(423, 8);
            label3.Name = "label3";
            label3.Size = new Size(76, 17);
            label3.TabIndex = 5;
            label3.Text = "Task Status";
            // 
            // cb_status
            // 
            cb_status.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cb_status.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_status.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cb_status.FormattingEnabled = true;
            cb_status.Location = new Point(423, 28);
            cb_status.Name = "cb_status";
            cb_status.Size = new Size(415, 25);
            cb_status.TabIndex = 1;
            cb_status.SelectedIndexChanged += cb_commands_SelectedIndexChanged;
            // 
            // cb_doneBy
            // 
            cb_doneBy.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cb_doneBy.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_doneBy.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cb_doneBy.FormattingEnabled = true;
            cb_doneBy.Location = new Point(3, 84);
            cb_doneBy.Name = "cb_doneBy";
            cb_doneBy.Size = new Size(414, 25);
            cb_doneBy.TabIndex = 2;
            cb_doneBy.SelectedIndexChanged += cb_commands_SelectedIndexChanged;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(423, 64);
            label5.Name = "label5";
            label5.Size = new Size(88, 17);
            label5.TabIndex = 5;
            label5.Text = "Task Done At";
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(3, 64);
            label6.Name = "label6";
            label6.Size = new Size(89, 17);
            label6.TabIndex = 5;
            label6.Text = "Task Done By";
            // 
            // tb_DoneAt
            // 
            tb_DoneAt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_DoneAt.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb_DoneAt.Location = new Point(423, 84);
            tb_DoneAt.Name = "tb_DoneAt";
            tb_DoneAt.ReadOnly = true;
            tb_DoneAt.Size = new Size(415, 25);
            tb_DoneAt.TabIndex = 3;
            // 
            // TaskForm
            // 
            AcceptButton = btn_save;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(862, 519);
            Controls.Add(tableLayoutPanel2);
            Controls.Add(tb_taskResult);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "TaskForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Add Task";
            tb_taskResult.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PropertyGrid propertyGrid;
        private Button btn_save;
        private Button btn_cancel;
        private TableLayoutPanel tb_taskResult;
        private ComboBox cb_commands;
        private TableLayoutPanel tableLayoutPanel2;
        private RichTextBox tb_logs;
        private Label label1;
        private Label label3;
        private Label label2;
        private ComboBox cb_status;
        private Label label5;
        private Label label4;
        private ComboBox cb_doneBy;
        private Label label6;
        private TextBox tb_DoneAt;
        private Button btn_taskResult;
    }
}