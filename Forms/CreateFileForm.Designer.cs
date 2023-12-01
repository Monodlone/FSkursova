namespace Kursova.Forms
{
    partial class CreateFileForm
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
            nameTextbox = new TextBox();
            nameLbl = new Label();
            contentsTextbox = new TextBox();
            contentsLbl = new Label();
            createBtn = new Button();
            SuspendLayout();
            // 
            // nameTextbox
            // 
            nameTextbox.Location = new Point(95, 23);
            nameTextbox.Name = "nameTextbox";
            nameTextbox.Size = new Size(110, 23);
            nameTextbox.TabIndex = 0;
            // 
            // nameLbl
            // 
            nameLbl.AutoSize = true;
            nameLbl.Location = new Point(28, 26);
            nameLbl.Name = "nameLbl";
            nameLbl.Size = new Size(39, 15);
            nameLbl.TabIndex = 1;
            nameLbl.Text = "Name";
            // 
            // contentsTextbox
            // 
            contentsTextbox.Location = new Point(95, 64);
            contentsTextbox.Multiline = true;
            contentsTextbox.Name = "contentsTextbox";
            contentsTextbox.Size = new Size(239, 220);
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
            // createBtn
            // 
            createBtn.Location = new Point(347, 415);
            createBtn.Name = "createBtn";
            createBtn.Size = new Size(75, 23);
            createBtn.TabIndex = 4;
            createBtn.Text = "Create";
            createBtn.UseVisualStyleBackColor = true;
            createBtn.Click += CreateBtn_Click;
            // 
            // CreateFileForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(434, 450);
            Controls.Add(createBtn);
            Controls.Add(contentsLbl);
            Controls.Add(contentsTextbox);
            Controls.Add(nameLbl);
            Controls.Add(nameTextbox);
            Name = "CreateFileForm";
            Text = "CreateFile";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox nameTextbox;
        private Label nameLbl;
        private TextBox contentsTextbox;
        private Label contentsLbl;
        private Button createBtn;
    }
}