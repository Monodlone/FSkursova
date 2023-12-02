namespace Kursova.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void CreateFileBtn_Click(object sender, EventArgs e)
        {
            var createFileForm = new CreateFileForm();
            createFileForm.ShowDialog();
        }

        private void CreateDirBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
