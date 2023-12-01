namespace Kursova
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
            createFileBtn = new Button();
            createDirBtn = new Button();
            treeView1 = new TreeView();
            editBtn = new Button();
            SuspendLayout();
            // 
            // createFileBtn
            // 
            createFileBtn.Location = new Point(283, 21);
            createFileBtn.Name = "createFileBtn";
            createFileBtn.Size = new Size(75, 23);
            createFileBtn.TabIndex = 1;
            createFileBtn.Text = "Create file";
            createFileBtn.UseVisualStyleBackColor = true;
            createFileBtn.Click += CreateFileBtn_Click;
            // 
            // createDirBtn
            // 
            createDirBtn.Location = new Point(283, 50);
            createDirBtn.Name = "createDirBtn";
            createDirBtn.Size = new Size(75, 23);
            createDirBtn.TabIndex = 3;
            createDirBtn.Text = "Create dir";
            createDirBtn.UseVisualStyleBackColor = true;
            createDirBtn.Click += CreateDirBtn_Click;
            // 
            // treeView1
            // 
            treeView1.Location = new Point(12, 12);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(265, 426);
            treeView1.TabIndex = 4;
            // 
            // editBtn
            // 
            editBtn.Location = new Point(283, 79);
            editBtn.Name = "editBtn";
            editBtn.Size = new Size(75, 23);
            editBtn.TabIndex = 5;
            editBtn.Text = "Edit";
            editBtn.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(538, 450);
            Controls.Add(editBtn);
            Controls.Add(treeView1);
            Controls.Add(createDirBtn);
            Controls.Add(createFileBtn);
            Name = "MainForm";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
        private Button createFileBtn;
        private Button createDirBtn;
        private TreeView treeView1;
        private Button editBtn;
    }
}
