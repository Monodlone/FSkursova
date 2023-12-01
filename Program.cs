using System.Text;

namespace Kursova
{
    internal static class Program
    {
        private static readonly FileStream Stream = File.Create("C:\\Users\\PiwKi\\Desktop\\fs_file");
        private static readonly BinaryWriter Bw = new BinaryWriter(Stream, Encoding.UTF8, true);
        private static readonly BinaryReader Br = new BinaryReader(Stream, Encoding.UTF8, true);
        private static long BitmapSectors { get; set; } = 1;
        private static readonly long RootOffset = BitmapSectors + 1;
        private static readonly long DataRegionOffset = RootOffset + 1;

        private const long SectorCount = 5120;
        private const long SectorSize = 512;
        private const long TotalSize = SectorCount * SectorSize;
        private const long BitmapSize = SectorCount / 8;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());

            Stream.SetLength(TotalSize);
            Stream.Position = 0;

            long tmp = BitmapSize;
            while (tmp > SectorSize)
            {
                BitmapSectors++;
                tmp /= SectorSize;
            }

            Stream.Position = RootOffset * SectorSize + 1;
            Bw.Write(false);
            Bw.Write("Root");

            Stream.Position = DataRegionOffset * SectorSize;
            var firstFileOffset = DataRegionOffset;
            Bw.Write(true);
            Bw.Write("name.txt");

            Stream.Position = DataRegionOffset * SectorSize + 11;//TODO fix this its wrong
            Bw.Write("This is the contents of the first file");

            Stream.Position = RootOffset + 6;
            //greshen nachin za zapiswane na adresi
            //trqbwa da moga da zapiswam adresi > 255
            Stream.WriteByte((byte)firstFileOffset);

            Stream.Position = 0;
            Bitmap.UpdateBitmap(new BinaryReader(Stream), (int)SectorSize, (int)BitmapSectors, (int)SectorCount);
        }

        public static FileStream GetFileStream()
        {
            return Stream;
        }

        public static void CreateFile(string[] fileInfo)
        {
            //1 find free sector using bitmap
            //2 change stream position to the free sector
            //3 write file name and contents using Bw
            //4 save file offset to Root
            //5 update the bitmap
            int requiredSectors = (int)(fileInfo[1].Length % SectorSize);
            long writeOffset = Bitmap.FindFreeSector(Br, requiredSectors, (int)BitmapSectors, (int)SectorCount, (int)SectorSize);
            Stream.Position = requiredSectors;
            var firstFileOffset = requiredSectors;
            Bw.Write(true);
            Bw.Write(fileInfo[0]);

            Stream.Position = requiredSectors + 11;
            Bw.Write(fileInfo[1]);
            Stream.Position = RootOffset + 6;
            Stream.WriteByte((byte)firstFileOffset);
        }
    }
}