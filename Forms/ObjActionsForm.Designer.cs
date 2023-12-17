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
            contentsTextbox = new TextBox();
            contentsLbl = new Label();
            CreateBtn = new Button();
            extensionLbl = new Label();
            EditBtn = new Button();
            nameTxtBox = new TextBox();
            SuspendLayout();
            // 
            // nameLbl
            // 
            nameLbl.AutoSize = true;
            nameLbl.Location = new Point(28, 26);
            nameLbl.Name = "nameLbl";
            nameLbl.Size = new Size(57, 15);
            nameLbl.TabIndex = 1;
            nameLbl.Text = "FileName";
            // 
            // contentsTextbox
            // 
            contentsTextbox.Font = new Font("Segoe UI", 10F);
            contentsTextbox.Location = new Point(95, 64);
            contentsTextbox.Multiline = true;
            contentsTextbox.Name = "contentsTextbox";
            contentsTextbox.ScrollBars = ScrollBars.Vertical;
            contentsTextbox.Size = new Size(343, 280);
            contentsTextbox.TabIndex = 2;
            // 
            // contentsLbl
            // 
            contentsLbl.AutoSize = true;
            contentsLbl.Location = new Point(28, 64);
            contentsLbl.Name = "contentsLbl";
            contentsLbl.Size = new Size(55, 15);
            contentsLbl.TabIndex = 3;
            contentsLbl.Text = "Contents";
            // 
            // CreateBtn
            // 
            CreateBtn.Font = new Font("Segoe UI", 11F);
            CreateBtn.Location = new Point(346, 350);
            CreateBtn.Name = "CreateBtn";
            CreateBtn.Size = new Size(92, 35);
            CreateBtn.TabIndex = 4;
            CreateBtn.Text = "Create";
            CreateBtn.UseVisualStyleBackColor = true;
            CreateBtn.Click += CreateBtn_Click;
            // 
            // extensionLbl
            // 
            extensionLbl.AutoSize = true;
            extensionLbl.Font = new Font("Segoe UI", 10F);
            extensionLbl.Location = new Point(222, 26);
            extensionLbl.Name = "extensionLbl";
            extensionLbl.Size = new Size(28, 19);
            extensionLbl.TabIndex = 5;
            extensionLbl.Text = ".txt";
            // 
            // EditBtn
            // 
            EditBtn.Font = new Font("Segoe UI", 11F);
            EditBtn.Location = new Point(346, 350);
            EditBtn.Name = "EditBtn";
            EditBtn.Size = new Size(92, 35);
            EditBtn.TabIndex = 6;
            EditBtn.Text = "Edit";
            EditBtn.UseVisualStyleBackColor = true;
            EditBtn.Click += EditBtn_Click;
            // 
            // nameTxtBox
            // 
            nameTxtBox.Location = new Point(104, 23);
            nameTxtBox.Name = "nameTxtBox";
            nameTxtBox.Size = new Size(121, 23);
            nameTxtBox.TabIndex = 8;
            nameTxtBox.KeyPress += nameTxtBox_KeyPress;
            // 
            // ObjActionsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(450, 397);
            Controls.Add(nameTxtBox);
            Controls.Add(EditBtn);
            Controls.Add(extensionLbl);
            Controls.Add(CreateBtn);
            Controls.Add(contentsLbl);
            Controls.Add(contentsTextbox);
            Controls.Add(nameLbl);
            Name = "ObjActionsForm";
            Text = "Create";
            Load += ObjActionsForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label nameLbl;
        private TextBox contentsTextbox;
        private Label contentsLbl;
        private Button CreateBtn;
        private Label extensionLbl;
        private Button EditBtn;
        private TextBox nameTxtBox;
    }
}