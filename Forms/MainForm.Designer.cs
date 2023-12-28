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
            StartBtn = new Button();
            MoveBtn = new Button();
            SuspendLayout();
            // 
            // CreateFileBtn
            // 
            CreateFileBtn.Font = new Font("Segoe UI", 10F);
            CreateFileBtn.Location = new Point(485, 42);
            CreateFileBtn.Margin = new Padding(5, 6, 5, 6);
            CreateFileBtn.Name = "CreateFileBtn";
            CreateFileBtn.Size = new Size(156, 56);
            CreateFileBtn.TabIndex = 1;
            CreateFileBtn.Text = "Create file";
            CreateFileBtn.UseVisualStyleBackColor = true;
            CreateFileBtn.Click += CreateFileBtn_Click;
            // 
            // CreateDirBtn
            // 
            CreateDirBtn.Font = new Font("Segoe UI", 10F);
            CreateDirBtn.Location = new Point(485, 110);
            CreateDirBtn.Margin = new Padding(5, 6, 5, 6);
            CreateDirBtn.Name = "CreateDirBtn";
            CreateDirBtn.Size = new Size(156, 56);
            CreateDirBtn.TabIndex = 3;
            CreateDirBtn.Text = "Create dir";
            CreateDirBtn.UseVisualStyleBackColor = true;
            CreateDirBtn.Click += CreateDirBtn_Click;
            // 
            // treeView
            // 
            treeView.Font = new Font("Segoe UI", 11F);
            treeView.Location = new Point(21, 24);
            treeView.Margin = new Padding(5, 6, 5, 6);
            treeView.Name = "treeView";
            treeView.Size = new Size(451, 848);
            treeView.TabIndex = 4;
            treeView.AfterSelect += TreeView_AfterSelect;
            treeView.MouseMove += treeView_MouseMove;
            // 
            // EditBtn
            // 
            EditBtn.Font = new Font("Segoe UI", 10F);
            EditBtn.Location = new Point(485, 246);
            EditBtn.Margin = new Padding(5, 6, 5, 6);
            EditBtn.Name = "EditBtn";
            EditBtn.Size = new Size(156, 56);
            EditBtn.TabIndex = 5;
            EditBtn.Text = "Edit";
            EditBtn.UseVisualStyleBackColor = true;
            EditBtn.Click += EditBtn_Click;
            // 
            // DeleteBtn
            // 
            DeleteBtn.Font = new Font("Segoe UI", 10F);
            DeleteBtn.Location = new Point(485, 450);
            DeleteBtn.Margin = new Padding(5, 6, 5, 6);
            DeleteBtn.Name = "DeleteBtn";
            DeleteBtn.Size = new Size(156, 56);
            DeleteBtn.TabIndex = 6;
            DeleteBtn.Text = "Delete";
            DeleteBtn.UseVisualStyleBackColor = true;
            DeleteBtn.Click += DeleteBtn_Click;
            // 
            // ImportBtn
            // 
            ImportBtn.Font = new Font("Segoe UI", 10F);
            ImportBtn.Location = new Point(485, 314);
            ImportBtn.Margin = new Padding(5, 6, 5, 6);
            ImportBtn.Name = "ImportBtn";
            ImportBtn.Size = new Size(156, 56);
            ImportBtn.TabIndex = 7;
            ImportBtn.Text = "Import";
            ImportBtn.UseVisualStyleBackColor = true;
            ImportBtn.Click += ImportBtn_Click;
            // 
            // ExportBtn
            // 
            ExportBtn.Font = new Font("Segoe UI", 10F);
            ExportBtn.Location = new Point(485, 382);
            ExportBtn.Margin = new Padding(5, 6, 5, 6);
            ExportBtn.Name = "ExportBtn";
            ExportBtn.Size = new Size(156, 56);
            ExportBtn.TabIndex = 8;
            ExportBtn.Text = "Export";
            ExportBtn.UseVisualStyleBackColor = true;
            ExportBtn.Click += ExportBtn_Click;
            // 
            // ViewBtn
            // 
            ViewBtn.Font = new Font("Segoe UI", 10F);
            ViewBtn.Location = new Point(485, 178);
            ViewBtn.Margin = new Padding(5, 6, 5, 6);
            ViewBtn.Name = "ViewBtn";
            ViewBtn.Size = new Size(156, 56);
            ViewBtn.TabIndex = 9;
            ViewBtn.Text = "View";
            ViewBtn.UseVisualStyleBackColor = true;
            ViewBtn.Click += ViewBtn_Click;
            // 
            // StartBtn
            // 
            StartBtn.Font = new Font("Segoe UI", 12F);
            StartBtn.Location = new Point(430, 518);
            StartBtn.Margin = new Padding(5, 6, 5, 6);
            StartBtn.Name = "StartBtn";
            StartBtn.Size = new Size(211, 114);
            StartBtn.TabIndex = 10;
            StartBtn.Text = "Start";
            StartBtn.UseVisualStyleBackColor = true;
            StartBtn.Click += StartBtn_Click;
            // 
            // MoveBtn
            // 
            MoveBtn.Font = new Font("Segoe UI", 10F);
            MoveBtn.Location = new Point(485, 518);
            MoveBtn.Margin = new Padding(5, 6, 5, 6);
            MoveBtn.Name = "MoveBtn";
            MoveBtn.Size = new Size(156, 56);
            MoveBtn.TabIndex = 13;
            MoveBtn.Text = "Move";
            MoveBtn.UseVisualStyleBackColor = true;
            MoveBtn.Click += MoveBtn_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(710, 900);
            Controls.Add(MoveBtn);
            Controls.Add(StartBtn);
            Controls.Add(ViewBtn);
            Controls.Add(ExportBtn);
            Controls.Add(ImportBtn);
            Controls.Add(DeleteBtn);
            Controls.Add(EditBtn);
            Controls.Add(treeView);
            Controls.Add(CreateDirBtn);
            Controls.Add(CreateFileBtn);
            Margin = new Padding(5, 6, 5, 6);
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
        private Button StartBtn;
        private Button MoveBtn;
    }
}
