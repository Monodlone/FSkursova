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
            CreateFileBtn = new Button();
            CreateDirBtn = new Button();
            treeView = new TreeView();
            EditBtn = new Button();
            DeleteBtn = new Button();
            ImportBtn = new Button();
            ExportBtn = new Button();
            ViewBtn = new Button();
            SuspendLayout();
            // 
            // CreateFileBtn
            // 
            CreateFileBtn.Font = new Font("Segoe UI", 10F);
            CreateFileBtn.Location = new Point(283, 21);
            CreateFileBtn.Name = "CreateFileBtn";
            CreateFileBtn.Size = new Size(91, 28);
            CreateFileBtn.TabIndex = 1;
            CreateFileBtn.Text = "Create file";
            CreateFileBtn.UseVisualStyleBackColor = true;
            CreateFileBtn.Click += CreateFileBtn_Click;
            // 
            // CreateDirBtn
            // 
            CreateDirBtn.Font = new Font("Segoe UI", 10F);
            CreateDirBtn.Location = new Point(283, 55);
            CreateDirBtn.Name = "CreateDirBtn";
            CreateDirBtn.Size = new Size(91, 28);
            CreateDirBtn.TabIndex = 3;
            CreateDirBtn.Text = "Create dir";
            CreateDirBtn.UseVisualStyleBackColor = true;
            CreateDirBtn.Click += CreateDirBtn_Click;
            // 
            // treeView
            // 
            treeView.Font = new Font("Segoe UI", 11F);
            treeView.Location = new Point(12, 12);
            treeView.Name = "treeView";
            treeView.Size = new Size(265, 426);
            treeView.TabIndex = 4;
            treeView.AfterSelect += TreeView_AfterSelect;
            // 
            // EditBtn
            // 
            EditBtn.Font = new Font("Segoe UI", 10F);
            EditBtn.Location = new Point(283, 123);
            EditBtn.Name = "EditBtn";
            EditBtn.Size = new Size(91, 28);
            EditBtn.TabIndex = 5;
            EditBtn.Text = "Edit";
            EditBtn.UseVisualStyleBackColor = true;
            EditBtn.Click += EditBtn_Click;
            // 
            // DeleteBtn
            // 
            DeleteBtn.Font = new Font("Segoe UI", 10F);
            DeleteBtn.Location = new Point(283, 225);
            DeleteBtn.Name = "DeleteBtn";
            DeleteBtn.Size = new Size(91, 28);
            DeleteBtn.TabIndex = 6;
            DeleteBtn.Text = "Delete";
            DeleteBtn.UseVisualStyleBackColor = true;
            DeleteBtn.Click += DeleteBtn_Click;
            // 
            // ImportBtn
            // 
            ImportBtn.Font = new Font("Segoe UI", 10F);
            ImportBtn.Location = new Point(283, 157);
            ImportBtn.Name = "ImportBtn";
            ImportBtn.Size = new Size(91, 28);
            ImportBtn.TabIndex = 7;
            ImportBtn.Text = "Import";
            ImportBtn.UseVisualStyleBackColor = true;
            ImportBtn.Click += ImportBtn_Click;
            // 
            // ExportBtn
            // 
            ExportBtn.Font = new Font("Segoe UI", 10F);
            ExportBtn.Location = new Point(283, 191);
            ExportBtn.Name = "ExportBtn";
            ExportBtn.Size = new Size(91, 28);
            ExportBtn.TabIndex = 8;
            ExportBtn.Text = "Export";
            ExportBtn.UseVisualStyleBackColor = true;
            ExportBtn.Click += ExportBtn_Click;
            // 
            // ViewBtn
            // 
            ViewBtn.Font = new Font("Segoe UI", 10F);
            ViewBtn.Location = new Point(283, 89);
            ViewBtn.Name = "ViewBtn";
            ViewBtn.Size = new Size(91, 28);
            ViewBtn.TabIndex = 9;
            ViewBtn.Text = "View";
            ViewBtn.UseVisualStyleBackColor = true;
            ViewBtn.Click += ViewBtn_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(414, 450);
            Controls.Add(ViewBtn);
            Controls.Add(ExportBtn);
            Controls.Add(ImportBtn);
            Controls.Add(DeleteBtn);
            Controls.Add(EditBtn);
            Controls.Add(treeView);
            Controls.Add(CreateDirBtn);
            Controls.Add(CreateFileBtn);
            Name = "MainForm";
            Text = "FileSystem";
            Load += MainForm_Load;
            ResumeLayout(false);
        }

        #endregion
        private Button CreateFileBtn;
        private Button CreateDirBtn;
        private TreeView treeView;
        private Button EditBtn;
        private Button DeleteBtn;
        private Button ImportBtn;
        private Button ExportBtn;
        private Button ViewBtn;
    }
}
