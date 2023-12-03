namespace Kursova.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public void UpdateTreeView(TreeNode node, TreeNode parent)
        {
            if (parent == null || parent.Name == "Root")
            {
                var rootNode = treeView.Nodes.Cast<TreeNode>().ToList().Find(n => n.Text.Equals("Root"))!;
                rootNode.Nodes.Add(node);
            }
            else
                parent.Nodes.Add(node);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void CreateFileBtn_Click(object sender, EventArgs e)
        {
            var createFileForm = new CreateFileForm();
            createFileForm.ShowDialog();
        }

    }
}
