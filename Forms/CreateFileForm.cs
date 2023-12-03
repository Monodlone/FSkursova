namespace Kursova.Forms
{
    public partial class CreateFileForm : Form
    {
        private static string? FileName { get; set; }
        private static string? FileContents { get; set; }

        public CreateFileForm()
        {
            InitializeComponent();
        }

        private void CreateBtn_Click(object sender, EventArgs e)
        {
            FileName = nameTextbox.Text;
            FileContents = contentsTextbox.Text;
            this.Close();
            FileSystem.CreateFile(FileName, FileContents);
        }
    }
}
