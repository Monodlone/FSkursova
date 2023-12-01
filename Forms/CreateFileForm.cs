namespace Kursova.Forms
{
    public partial class CreateFileForm : Form
    {
        private string FileName { get; set; }
        private string FileContents { get; set; }
        public CreateFileForm()
        {
            InitializeComponent();
        }

        private void CreateBtn_Click(object sender, EventArgs e)
        {
            FileName  = nameTextbox.Text;
            FileContents = contentsTextbox.Text;
        }

        public string[] GetFileInfo()
        {
            string[] arr = { FileName, FileContents };
            return arr;
        }
    }
}
