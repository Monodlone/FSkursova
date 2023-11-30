using System.Text;

namespace Kursova
{
    internal static class Program
    {
        private static readonly FileStream stream = File.Create("C:\\Users\\PiwKi\\Desktop\\fs_file");
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
            stream.SetLength(TotalSize);
            const long BitmapSize = SectorCount / 8;
            const long BitmapSectors = BitmapSize / SectorSize;
            const long RootOffset = BitmapSize + 1;
            const long RootSize = SectorSize;
            const long DataRegionOffset = RootOffset + SectorSize;

            const long NameSizeLimit = 31;

            using var bw = new BinaryWriter(stream, Encoding.UTF8, true);

            stream.Position = 0;
            for (var i = 0; i < BitmapSize; i++)//memory allocation for BitMap
                stream.WriteByte(0);

            stream.Position = RootOffset;
            bw.Write(false);
            bw.Write("Root");
            for (var i = 0; i < RootSize; i++)//memory allocation for Root
                stream.WriteByte(0);

            stream.Position = DataRegionOffset;
            var firstFileOffset = DataRegionOffset;
            bw.Write(true);
            bw.Write("name.txt");
            for (var i = 0; i < SectorSize; i++)//memory allocation for file
                stream.WriteByte(0);
            stream.Position = DataRegionOffset + 11;
            bw.Write("This is the contents of the first file");

            stream.Position = RootOffset + 6;
            stream.WriteByte((byte)firstFileOffset);
            stream.Position = 0;


            Bitmap.UpdateBitmap(new BinaryReader(stream), (int)BitmapSectors, (int)SectorCount);
        }

        public static FileStream GetFileStream()
        {
            return stream;
        }
    }
}