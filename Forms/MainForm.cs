namespace Kursova.Forms
{
    public partial class MainForm : Form
    {
        private static readonly TreeNode RootNode = new("Root");
        private static TreeNode CWD { get; set; } = RootNode;
        private TreeNode? FileToInteract { get; set; }


        public MainForm() => InitializeComponent();

        public static void AddTreeviewNodes(string name, long offset, bool isFile)
        {
            //create node from file info
            var node = new TreeNode(name) { ForeColor = isFile ? Color.Green : Color.Red, Tag = offset };
            //add to CWD in treeview
            CWD.Nodes.Add(node);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            treeView.Nodes.Add(RootNode);
            RootNode.ForeColor = Color.Red;
            RootNode.Tag = FileSystem.GetRootOffset();
        }

        private void CreateFileBtn_Click(object sender, EventArgs e)
        {
            var objActionsForm = new ObjActionsForm(true, false, false);
            objActionsForm.ShowDialog();
        }

        private void CreateDirBtn_Click(object sender, EventArgs e)
        {
            var objActionsForm = new ObjActionsForm(false, false, false);
            objActionsForm.ShowDialog();
        }

        private void ViewBtn_Click(object sender, EventArgs e)
        {
            if (FileToInteract == null) return;

            var objActionsForm = new ObjActionsForm(true, false, true);
            var fileInfo = FileSystem.ReadFile((long)FileToInteract.Tag, FileToInteract.Text + ".txt");
            objActionsForm.SetFileContents(fileInfo);
            objActionsForm.ShowDialog();
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (FileToInteract == null)
                FileSystem.DeleteObject((long)CWD.Tag);
            else
                FileSystem.DeleteObject((long)FileToInteract.Tag);
        }

        private void EditBtn_Click(object sender, EventArgs e)
        {
            if (FileToInteract == null) return;

            var objActionsForm = new ObjActionsForm(true, true, false);
            objActionsForm.ShowDialog();
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var currNode = treeView.SelectedNode;
            if (currNode.ForeColor == Color.Red)//Red == Directory
            {
                CWD = currNode;
                FileToInteract = null;
            }
            else //if (currNode.ForeColor == Color.Green)//Green == File
                FileToInteract = currNode;
        }
    }
}