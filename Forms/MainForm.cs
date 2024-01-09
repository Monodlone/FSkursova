using System.Text;

namespace Kursova.Forms
{
    public partial class MainForm : Form
    {
        internal static TreeNode CWD { get; private set; }
        internal static TreeNode? FileToInteract { get; private set; }
        private static TreeNode RootNode { get; set;  }
        private static TreeNode? LastSelectedFile { get; set; }

        internal static bool Restore { get; set; }

        internal static readonly Color FileColor = Color.Green;
        internal static readonly Color BadObjColor = Color.Red;
        private static readonly Color DirColor = Color.Blue;

        public MainForm() => InitializeComponent();

        private void MainForm_Load(object sender, EventArgs e)
        {
            CreateFileBtn.Visible = false;
            CreateDirBtn.Visible = false;
            DeleteBtn.Visible = false;
            ViewBtn.Visible = false;
            EditBtn.Visible = false;
            ExportBtn.Visible = false;
            ImportBtn.Visible = false;
            treeView.Visible = false;
            MoveBtn.Visible = false;
        }

        internal static void InitRoot(TreeNode root)
        {
            RootNode = root;
            RootNode.ForeColor = DirColor;
            treeView.Nodes.Add(RootNode);
            CWD = RootNode;
        }

        public static void AddTreeviewNodes(string name, long offset, bool isFile)
        {
            //create node from file info
            var node = new TreeNode(name) { ForeColor = isFile ? FileColor : DirColor, Tag = offset };
            CWD.Nodes.Add(node);
        }

        public static void DeleteNode(TreeNode node) => node.Remove();

        internal static void ChangeToRootWhenCwdBad() => CWD = RootNode;

        internal static void ResetParentDir(TreeNode node) => node.Nodes.Clear();

        internal static void ChangeCWDForFileEditing(TreeNode fileParent) => CWD = fileParent;

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var currNode = treeView.SelectedNode;
            if (currNode.ForeColor == BadObjColor)
                return;
            if (currNode.ForeColor == DirColor)//Red == Directory
            {
                CWD = currNode;
                FileSystem.CWDOffset = (long)CWD.Tag;
                LastSelectedFile = FileToInteract;
                FileToInteract = null;
            }
            else //if (currNode.ForeColor == fileColor)//Green == File
                FileToInteract = currNode;
        }

        private static string MyGetNameWithoutExtension(string path)
        {
            var name = "";
            var lastIndex = -1;
            for (var i = 0; i < path.Length; i++)
                if (path[i] == '\\')
                    lastIndex = i + 1;

            for (var i = lastIndex; i < path.Length; i++)
            {
                if (path[i] == '.')
                    break;
                name += path[i];
            }
            return name;
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
            if (CWD.IsExpanded == false)
            {
                FileSystem.ReadDirectory(FileSystem.CWDOffset);
                CWD.Expand();
                return;
            }

            if (FileToInteract == null)
                return;

            if (FileToInteract.ForeColor == Color.Red)
            {
                MessageBox.Show("Error: Object is corrupted");
                return;
            }

            var objActionsForm = new ObjActionsForm(true, false, true);
            var fileInfo = FileSystem.ReadFile((long)FileToInteract.Tag);
            if (fileInfo == null)
            {
                FileToInteract.ForeColor = BadObjColor;
                return;
            }
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
            if (FileToInteract.ForeColor == Color.Red)
            {
                MessageBox.Show("Error: Object is corrupted");
                return;
            }

            var objActionsForm = new ObjActionsForm(true, true, false);
            var fileInfo = FileSystem.ReadFile((long)FileToInteract.Tag);
            if (fileInfo == null)
            {
                FileToInteract.ForeColor = BadObjColor;
                return;
            }
            objActionsForm.SetFileContents(fileInfo);
            objActionsForm.ShowDialog();
        }


        private void ExportBtn_Click(object sender, EventArgs e)
        {
            if (FileToInteract == null) return;
            if (FileToInteract.ForeColor == Color.Red)
            {
                MessageBox.Show("Error: Object is corrupted");
                return;
            }

            var svFileDialog = new SaveFileDialog();
            svFileDialog.Filter = "txt files (*.txt)|*.txt";
            var info = FileSystem.ReadFile((long)FileToInteract.Tag);
            svFileDialog.FileName = info[0];
            if (svFileDialog.ShowDialog() == DialogResult.OK)
            {
                using var fileStream = new FileStream(svFileDialog.FileName, FileMode.Create);
                try
                {
                    using var sw = new StreamWriter(fileStream);
                    sw.Write(info![1]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not write file to disk. Original error: " + ex.Message);
                }
            }
        }

        private void ImportBtn_Click(object sender, EventArgs e)
        {
            var opnFileDialog = new OpenFileDialog();
            opnFileDialog.Filter = "TXT files|*.txt";
            if (opnFileDialog.ShowDialog() == DialogResult.OK)
            {
                var path = opnFileDialog.FileName;
                var contents = "";
                try
                {
                    using var sr = new StreamReader(path);
                    contents = sr.ReadToEnd();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                    return;
                }
                FileSystem.CreateFile(MyGetNameWithoutExtension(path), contents);
            }
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            var initForm = new StartingParameters();
            initForm.ShowDialog();
            if (initForm.GetSectorSize() == 0 || initForm.GetSectorCount() == 0)
                return;
            FileSystem.Initiate(initForm.GetSectorSize(), initForm.GetSectorCount(), Restore);
            CreateFileBtn.Visible = true;
            CreateDirBtn.Visible = true;
            DeleteBtn.Visible = true;
            ViewBtn.Visible = true;
            EditBtn.Visible = true;
            ExportBtn.Visible = true;
            ImportBtn.Visible = true;
            treeView.Visible = true;
            MoveBtn.Visible = true;
            StartBtn.Visible = false;
        }

        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            //keep last selected node highlighted
            if (treeView.SelectedNode != null)
                treeView.SelectedNode.Checked = true;
            //keep bad directory collapsed
            if (CWD.ForeColor == BadObjColor)
                CWD.Collapse();

            if (RootNode.ForeColor == BadObjColor)
            {
                MessageBox.Show("Fatal error: Root is corrupted");
                throw new ArgumentException("Fatal error: Root is corrupted");
            }
        }

        private void MoveBtn_Click(object sender, EventArgs e)
        {
            if (LastSelectedFile == null)
                return;

            var fileOffset = (long)LastSelectedFile.Tag;
            var parentOffset = FileSystem.GetParentOffset(fileOffset);

            if (parentOffset == FileSystem.CWDOffset)
                return;

            //replace old parent offset with new one in file
            var stream = FileSystem.GetStream();
            stream.Position = fileOffset + 1;
            var bw = new BinaryWriter(stream, Encoding.UTF8);
            bw.Write(FileSystem.CWDOffset);
            //bw.Close();
            //stream.Close();
            //remove file offset from old parent
            FileSystem.RemoveOffsetFromParent(fileOffset, parentOffset);

            //write file offset to new parent dir
            FileSystem.UpdateDir(fileOffset, CWD);

            //move node to new parent dir in tree view
            LastSelectedFile.Remove();
            CWD.Nodes.Add(LastSelectedFile);
        }
    }
}