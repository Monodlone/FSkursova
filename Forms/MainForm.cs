namespace Kursova.Forms
{
    public partial class MainForm : Form
    {
        private static readonly TreeNode RootNode = new("Root");

        public MainForm() => InitializeComponent();

        public static void AddTreeviewNodes(string name, long offset, bool isFile)
        {
            //create node from file name
            var node = new TreeNode(name) { ForeColor = isFile ? Color.Green : Color.Red, Tag = offset };
            //node.Tag = offset;
            //find parent of file
            //add to correct parent in treeview
            RootNode.Nodes.Add(node);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            treeView.Nodes.Add(RootNode);
            RootNode.ForeColor = Color.Red;
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

        }
    }
}