namespace Kursova.Forms
{
    public partial class ObjActionsForm : Form
    {
        private static string? ObjectName { get; set; }
        private static string? FileContents { get; set; }
        private static bool IsFile { get; set; }
        private bool IsEditing { get; set; }
        private bool IsViewing { get; set; }

        public ObjActionsForm(bool isfile, bool isEditing, bool isViewing)
        {
            IsFile = isfile;
            IsEditing = isEditing;
            IsViewing = isViewing;
            InitializeComponent();
        }

        internal void SetFileContents(string[] info)
        {
            if (info[0] == null || info[1] == null || info == null) return;

            nameTxtBox.Text = info[0];
            contentsTextbox.Text = info[1];
        }

        private void CreateBtn_Click(object sender, EventArgs e)
        {
            if (nameTxtBox.Text == "")
            {
                Close();
                return;
            }
            EditBtn.Visible = false;
            ObjectName = nameTxtBox.Text;
            FileContents = contentsTextbox.Text;
            Close();
            if (IsFile)
                FileSystem.CreateFile(ObjectName, FileContents);
            else
                FileSystem.CreateDirectory(ObjectName);
        }

        private void EditBtn_Click(object sender, EventArgs e)
        {
            var fileParent = MainForm.FileToInteract!.Parent;
            FileSystem.DeleteObject(MainForm.FileToInteract);
            ObjectName = nameTxtBox.Text;
            FileContents = contentsTextbox.Text;
            Close();

            MainForm.ChangeCWDForFileEditing(fileParent);
            FileSystem.CreateFile(ObjectName, FileContents);
        }

        private void ObjActionsForm_Load(object sender, EventArgs e)
        {
            contentsTextbox.Visible = IsFile;
            contentsLbl.Visible = IsFile;
            extensionLbl.Visible = IsFile;

            EditBtn.Visible = IsEditing;
            CreateBtn.Visible = !IsEditing;

            if (!IsViewing) return;

            EditBtn.Visible = !IsViewing;
            CreateBtn.Visible = !IsViewing;
            contentsTextbox.ReadOnly = IsViewing;
            nameTxtBox.ReadOnly = IsViewing;
        }

        private void nameTxtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !IsValidCharacter(e.KeyChar);
        }

        private bool IsValidCharacter(char ch)
        {
            var lower = ch >= 'a' && ch <= 'z';
            var upper = ch >= 'A' && ch <= 'Z';
            return (lower || upper || ch == 8) && (nameTxtBox.Text.Length <= FileSystem.NameLength || ch == 8);//ch == 8 -> backspace
        }
    }
}
