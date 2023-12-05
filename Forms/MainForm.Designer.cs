namespace Kursova.Forms
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
            TreeNode treeNode1 = new TreeNode("Root");
            createFileBtn = new Button();
            createDirBtn = new Button();
            treeView = new TreeView();
            editBtn = new Button();
            deleteBtn = new Button();
            SuspendLayout();
            // 
            // createFileBtn
            // 
            createFileBtn.Font = new Font("Segoe UI", 10F);
            createFileBtn.Location = new Point(283, 21);
            createFileBtn.Name = "createFileBtn";
            createFileBtn.Size = new Size(91, 28);
            createFileBtn.TabIndex = 1;
            createFileBtn.Text = "Create file";
            createFileBtn.UseVisualStyleBackColor = true;
            createFileBtn.Click += CreateFileBtn_Click;
            // 
            // createDirBtn
            // 
            createDirBtn.Font = new Font("Segoe UI", 10F);
            createDirBtn.Location = new Point(283, 55);
            createDirBtn.Name = "createDirBtn";
            createDirBtn.Size = new Size(91, 28);
            createDirBtn.TabIndex = 3;
            createDirBtn.Text = "Create dir";
            createDirBtn.UseVisualStyleBackColor = true;
            createDirBtn.Click += CreateDirBtn_Click;
            // 
            // treeView
            // 
            treeView.Font = new Font("Segoe UI", 11F);
            treeView.Location = new Point(12, 12);
            treeView.Name = "treeView";
            treeNode1.Name = "rootNode";
            treeNode1.Text = "Root";
            treeView.Nodes.AddRange(new TreeNode[] { treeNode1 });
            treeView.Size = new Size(265, 426);
            treeView.TabIndex = 4;
            // 
            // editBtn
            // 
            editBtn.Font = new Font("Segoe UI", 10F);
            editBtn.Location = new Point(283, 89);
            editBtn.Name = "editBtn";
            editBtn.Size = new Size(91, 28);
            editBtn.TabIndex = 5;
            editBtn.Text = "Edit";
            editBtn.UseVisualStyleBackColor = true;
            // 
            // deleteBtn
            // 
            deleteBtn.Font = new Font("Segoe UI", 10F);
            deleteBtn.Location = new Point(283, 123);
            deleteBtn.Name = "deleteBtn";
            deleteBtn.Size = new Size(91, 28);
            deleteBtn.TabIndex = 6;
            deleteBtn.Text = "Delete";
            deleteBtn.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(414, 450);
            Controls.Add(deleteBtn);
            Controls.Add(editBtn);
            Controls.Add(treeView);
            Controls.Add(createDirBtn);
            Controls.Add(createFileBtn);
            Name = "MainForm";
            Text = "FileSystem";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
        private Button createFileBtn;
        private Button createDirBtn;
        private TreeView treeView;
        private Button editBtn;
        private Button deleteBtn;
    }
}
