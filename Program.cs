using System.Text;

namespace Kursova
{
    internal static class Program
    {
        private static readonly FileStream Stream = File.Create("C:\\Users\\PiwKi\\Desktop\\fs_file");
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());


            //var stream = File.Create("C:\\Users\\PiwKi\\Desktop\\fs_file");
            const long SectorCount = 5000;
            const long SectorSize = 512;
            const long TotalSize = SectorCount * SectorSize;
            Stream.SetLength(TotalSize);
            const long BitmapSize = SectorCount / 8;
            const long BitmapSectors = BitmapSize / SectorSize;
            const long RootOffset = BitmapSize + 1;
            const long RootSize = SectorSize;
            const long DataRegionOffset = RootOffset + SectorSize;

            using var bw = new BinaryWriter(Stream, Encoding.UTF8, true);

            Stream.Position = 0;
            for (var i = 0; i < BitmapSize; i++)//memory allocation for BitMap
                Stream.WriteByte(0);

            Stream.Position = RootOffset;
            bw.Write(false);
            bw.Write("Root");
            for (var i = 0; i < RootSize; i++)//memory allocation for Root
                Stream.WriteByte(0);

            Stream.Position = DataRegionOffset;
            var firstFileOffset = DataRegionOffset;
            bw.Write(true);
            bw.Write("name.txt");
            for (var i = 0; i < SectorSize; i++)//memory allocation for file
                Stream.WriteByte(0);
            Stream.Position = DataRegionOffset + 11;
            bw.Write("This is the contents of the first file");

            Stream.Position = RootOffset + 6;
            //greshen nachin za zapiswane na adresi
            //trqbwa da moga da zapiswam adresi > 255
            Stream.WriteByte((byte)firstFileOffset);

            Stream.Position = 0;
            Bitmap.UpdateBitmap(new BinaryReader(Stream), (int)BitmapSectors, (int)SectorCount);
        }

        public static FileStream GetFileStream()
        {
            return Stream;
        }
    }
}