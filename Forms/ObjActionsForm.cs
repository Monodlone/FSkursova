namespace Kursova.Forms
{
    public partial class ObjActionsForm : Form
    {
        private static string? FileName { get; set; }
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
            if (info[0] == null || info[1] == null) return;

            nameTxtBox.Text = info[0];
            contentsTextbox.Text = info[1];
        }

        private void CreateBtn_Click(object sender, EventArgs e)
        {
            EditBtn.Visible = false;
            FileName = nameTxtBox.Text;
            FileContents = contentsTextbox.Text;
            Close();
            if (IsFile)
                FileSystem.CreateFile(FileName, FileContents);
            else
                FileSystem.CreateDirectory(FileName);
        }

        private void EditBtn_Click(object sender, EventArgs e)
        {
            FileSystem.DeleteObject(MainForm.FileToInteract);
            FileName = nameTxtBox.Text;
            FileContents = contentsTextbox.Text;
            Close();
            FileSystem.CreateFile(FileName, FileContents);
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

        private void nameTxtBox_KeyPress(object sender, KeyPressEventArgs e) => e.Handled = !IsValidCharacter(e.KeyChar);

        private bool IsValidCharacter(char ch)
        {
            var lower = ch >= 'a' && ch <= 'z';
            var upper = ch >= 'A' && ch <= 'Z';
            return lower || upper || ch == 8;//backspace
        }
    }
}
