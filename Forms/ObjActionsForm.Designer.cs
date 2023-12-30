namespace Kursova.Forms
{
    partial class ObjActionsForm
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
            nameLbl = new Label();
            contentsLbl = new Label();
            CreateBtn = new Button();
            extensionLbl = new Label();
            EditBtn = new Button();
            nameTxtBox = new TextBox();
            contentsTextbox = new TextBox();
            SuspendLayout();
            // 
            // nameLbl
            // 
            nameLbl.AutoSize = true;
            nameLbl.Location = new Point(48, 52);
            nameLbl.Margin = new Padding(5, 0, 5, 0);
            nameLbl.Name = "nameLbl";
            nameLbl.Size = new Size(100, 30);
            nameLbl.TabIndex = 1;
            nameLbl.Text = "FileName";
            // 
            // contentsLbl
            // 
            contentsLbl.AutoSize = true;
            contentsLbl.Location = new Point(48, 128);
            contentsLbl.Margin = new Padding(5, 0, 5, 0);
            contentsLbl.Name = "contentsLbl";
            contentsLbl.Size = new Size(96, 30);
            contentsLbl.TabIndex = 3;
            contentsLbl.Text = "Contents";
            // 
            // CreateBtn
            // 
            CreateBtn.Font = new Font("Segoe UI", 11F);
            CreateBtn.Location = new Point(593, 700);
            CreateBtn.Margin = new Padding(5, 6, 5, 6);
            CreateBtn.Name = "CreateBtn";
            CreateBtn.Size = new Size(158, 70);
            CreateBtn.TabIndex = 4;
            CreateBtn.Text = "Create";
            CreateBtn.UseVisualStyleBackColor = true;
            CreateBtn.Click += CreateBtn_Click;
            // 
            // extensionLbl
            // 
            extensionLbl.AutoSize = true;
            extensionLbl.Font = new Font("Segoe UI", 10F);
            extensionLbl.Location = new Point(381, 52);
            extensionLbl.Margin = new Padding(5, 0, 5, 0);
            extensionLbl.Name = "extensionLbl";
            extensionLbl.Size = new Size(46, 32);
            extensionLbl.TabIndex = 5;
            extensionLbl.Text = ".txt";
            // 
            // EditBtn
            // 
            EditBtn.Font = new Font("Segoe UI", 11F);
            EditBtn.Location = new Point(593, 700);
            EditBtn.Margin = new Padding(5, 6, 5, 6);
            EditBtn.Name = "EditBtn";
            EditBtn.Size = new Size(158, 70);
            EditBtn.TabIndex = 6;
            EditBtn.Text = "Edit";
            EditBtn.UseVisualStyleBackColor = true;
            EditBtn.Click += EditBtn_Click;
            // 
            // nameTxtBox
            // 
            nameTxtBox.Location = new Point(178, 46);
            nameTxtBox.Margin = new Padding(5, 6, 5, 6);
            nameTxtBox.Name = "nameTxtBox";
            nameTxtBox.Size = new Size(205, 35);
            nameTxtBox.TabIndex = 8;
            nameTxtBox.KeyPress += nameTxtBox_KeyPress;
            // 
            // contentsTextbox
            // 
            contentsTextbox.Font = new Font("Segoe UI", 10F);
            contentsTextbox.Location = new Point(178, 132);
            contentsTextbox.Margin = new Padding(5, 6, 5, 6);
            contentsTextbox.Multiline = true;
            contentsTextbox.Name = "contentsTextbox";
            contentsTextbox.ScrollBars = ScrollBars.Vertical;
            contentsTextbox.Size = new Size(585, 556);
            contentsTextbox.TabIndex = 9;
            // 
            // ObjActionsForm
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(775, 794);
            Controls.Add(contentsTextbox);
            Controls.Add(nameTxtBox);
            Controls.Add(EditBtn);
            Controls.Add(extensionLbl);
            Controls.Add(CreateBtn);
            Controls.Add(contentsLbl);
            Controls.Add(nameLbl);
            Margin = new Padding(5, 6, 5, 6);
            Name = "ObjActionsForm";
            Text = "Create";
            Load += ObjActionsForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label nameLbl;
        private Label contentsLbl;
        private Button CreateBtn;
        private Label extensionLbl;
        private Button EditBtn;
        private TextBox nameTxtBox;
        private TextBox contentsTextbox;
    }
}