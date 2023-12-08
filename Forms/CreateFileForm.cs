namespace Kursova.Forms
{
    public partial class CreateFileForm : Form
    {
        private static string? FileName { get; set; }
        private static string? FileContents { get; set; }
        private static bool IsFile { get; set; }

        public CreateFileForm(bool isfile)
        {
            IsFile = isfile;
            InitializeComponent();
        }

        private void CreateBtn_Click(object sender, EventArgs e)
        {
            FileName = nameTextbox.Text;
            FileContents = contentsTextbox.Text;
            this.Close();
            if(IsFile)
                FileSystem.CreateFile(FileName, FileContents);
            else
                FileSystem.CreateDirectory(FileName);
        }

        private void CreateFileForm_Load(object sender, EventArgs e)
        {
            contentsTextbox.Visible = IsFile;
            contentsLbl.Visible = IsFile;
            extensionLbl.Visible = IsFile;
        }
    }
}
