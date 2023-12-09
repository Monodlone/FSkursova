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
            var createFileForm = new CreateFileForm(true);
            createFileForm.ShowDialog();
        }

        private void CreateDirBtn_Click(object sender, EventArgs e)
        {
            var createFileForm = new CreateFileForm(false);
            createFileForm.ShowDialog();
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //find the selected node
            var currNode = treeView.SelectedNode;
            //if it's a dir update CWD
            if (currNode.ForeColor == Color.Red)
                CWD = currNode;
            //if it's a file update fileToInteract
            else if (currNode.ForeColor == Color.Green)
                FileToInteract = currNode;
        }
    }
}