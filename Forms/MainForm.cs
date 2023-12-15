using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Windows.Forms;

namespace Kursova.Forms
{
    public partial class MainForm : Form
    {
        private static readonly TreeNode RootNode = new("Root");
        internal static TreeNode CWD { get; private set; } = RootNode;
        internal static TreeNode? FileToInteract { get; private set; }



        public MainForm() => InitializeComponent();

        public static void AddTreeviewNodes(string name, long offset, bool isFile)
        {
            //create node from file info
            var node = new TreeNode(name) { ForeColor = isFile ? Color.Green : Color.Red, Tag = offset };
            //add to CWD in treeview
            CWD.Nodes.Add(node);
        }

        public static void DeleteNode(TreeNode node) => node.Remove();

        //public static TreeNode GetSelectedNode() => treeView.SelectedNode;

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
            var fileInfo = FileSystem.ReadFile((long)FileToInteract.Tag, FileToInteract.Text);
            objActionsForm.SetFileContents(fileInfo);
            objActionsForm.ShowDialog();
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == RootNode) return;

            FileSystem.DeleteObject(treeView.SelectedNode);
        }

        private void EditBtn_Click(object sender, EventArgs e)
        {
            if (FileToInteract == null) return;

            var objActionsForm = new ObjActionsForm(true, true, false);
            var fileInfo = FileSystem.ReadFile((long)FileToInteract.Tag, FileToInteract.Text);
            objActionsForm.SetFileContents(fileInfo);
            objActionsForm.ShowDialog();
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
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

        private void ExportBtn_Click(object sender, EventArgs e)
        {
            if (FileToInteract == null) return;

            var svFileDialog = new SaveFileDialog();
            svFileDialog.Filter = "txt files (*.txt)|*.txt";
            if (svFileDialog.ShowDialog() == DialogResult.OK)
            {
                var info = FileSystem.ReadFile((long)FileToInteract.Tag, FileToInteract.Text);
                using var fileStream = new FileStream(svFileDialog.FileName, FileMode.Create);
                    using (var sw = new StreamWriter(fileStream))
                        sw.Write(info?[1]);
            }
        }

        private void ImportBtn_Click(object sender, EventArgs e)
        {

        }
    }
}