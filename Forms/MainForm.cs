namespace Kursova.Forms
{
    public partial class MainForm : Form
    {
        private static readonly TreeNode RootNode = new ("Root");

        public MainForm()
        {
            InitializeComponent();
            treeView.Nodes.Add(RootNode);
            RootNode.ForeColor = Color.Red;
        }

        public static void AddTreeviewNodes(string name, bool isFile)
        {
            //create node from file name
            var node = new TreeNode(name)
            {
                ForeColor = isFile ? Color.Green : Color.Red
            };
            //find parent of file
            //add to correct parent in treeview
            RootNode.Nodes.Add(node);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

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
    }
}