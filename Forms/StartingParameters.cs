namespace Kursova.Forms
{
    public partial class StartingParameters : Form
    {
        private long SectorSize { get; set; }
        private long SectorCount { get; set; }

        public StartingParameters()
        {
            InitializeComponent();
        }

        public long GetSectorSize() => SectorSize;

        public long GetSectorCount() => SectorCount;

        private void CreateFsBtn_Click(object sender, EventArgs e)
        {
            switch (SectorCountBox.SelectedItem)
            {
                case "2560":
                    SectorCount = 2560;
                    break;
                case "5120":
                    SectorCount = 5120;
                    break;
                case "10240":
                    SectorCount = 10240;
                    break;
            }
            Close();
        }

        private void SectorSizeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SectorCountBox.Items.Clear();

            switch (SectorSizeBox.SelectedItem)
            {
                case "256":
                    SectorSize = 256;
                    SectorCountBox.Items.Add("2560");
                    SectorCountBox.Items.Add("5120");
                    SectorCountBox.Items.Add("10240");
                    break;
                case "512":
                    SectorSize = 512;
                    SectorCountBox.Items.Add("5120");
                    SectorCountBox.Items.Add("10240");
                    break;
                case "1024":
                    SectorSize = 1024;
                    SectorCountBox.Items.Add("10240");
                    break;
            }
        }
    }
}
