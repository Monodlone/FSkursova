namespace Kursova.Forms
{
    partial class StartingParameters
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
            CreateFsBtn = new Button();
            label1 = new Label();
            label2 = new Label();
            SectorSizeBox = new ListBox();
            SectorCountBox = new ListBox();
            SuspendLayout();
            // 
            // CreateFsBtn
            // 
            CreateFsBtn.Font = new Font("Segoe UI", 10F);
            CreateFsBtn.Location = new Point(198, 160);
            CreateFsBtn.Name = "CreateFsBtn";
            CreateFsBtn.Size = new Size(89, 35);
            CreateFsBtn.TabIndex = 2;
            CreateFsBtn.Text = "Create";
            CreateFsBtn.UseVisualStyleBackColor = true;
            CreateFsBtn.Click += CreateFsBtn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10F);
            label1.Location = new Point(9, 46);
            label1.Name = "label1";
            label1.Size = new Size(111, 19);
            label1.TabIndex = 3;
            label1.Text = "Select sector size";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10F);
            label2.Location = new Point(9, 105);
            label2.Name = "label2";
            label2.Size = new Size(124, 19);
            label2.TabIndex = 4;
            label2.Text = "Select sector count";
            // 
            // SectorSizeBox
            // 
            SectorSizeBox.Font = new Font("Segoe UI", 11F);
            SectorSizeBox.FormattingEnabled = true;
            SectorSizeBox.ItemHeight = 20;
            SectorSizeBox.Items.AddRange(new object[] { "256", "512", "1024" });
            SectorSizeBox.Location = new Point(139, 46);
            SectorSizeBox.Name = "SectorSizeBox";
            SectorSizeBox.ScrollAlwaysVisible = true;
            SectorSizeBox.Size = new Size(120, 24);
            SectorSizeBox.TabIndex = 5;
            SectorSizeBox.SelectedIndexChanged += SectorSizeBox_SelectedIndexChanged;
            // 
            // SectorCountBox
            // 
            SectorCountBox.Font = new Font("Segoe UI", 11F);
            SectorCountBox.FormattingEnabled = true;
            SectorCountBox.ItemHeight = 20;
            SectorCountBox.Location = new Point(140, 105);
            SectorCountBox.Name = "SectorCountBox";
            SectorCountBox.ScrollAlwaysVisible = true;
            SectorCountBox.Size = new Size(120, 24);
            SectorCountBox.TabIndex = 6;
            // 
            // StartingParameters
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(299, 209);
            Controls.Add(SectorCountBox);
            Controls.Add(SectorSizeBox);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(CreateFsBtn);
            Name = "StartingParameters";
            Text = "StartingParameters";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button CreateFsBtn;
        private Label label1;
        private Label label2;
        private ListBox SectorSizeBox;
        private ListBox SectorCountBox;
    }
}