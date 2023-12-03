using Kursova.Forms;
using System.Text;

namespace Kursova
{

    internal static class FileSystem
    {
        //TODO LIST:
        //TODO bitmap can't look for multiple sectors for big files

        private static readonly FileStream Stream = File.Create("C:\\Users\\PiwKi\\Desktop\\fs_file");
        private static readonly BinaryWriter Bw = new(Stream, Encoding.UTF8, true);
        private static readonly BinaryReader Br = new(Stream, Encoding.UTF8, true);
        private static long BitmapSectors { get; set; } = 1;
        private static long RootOffset { get; set; }
        private static int _rootFileAddressOffset = 0;

        private const long SectorCount = 5120;
        private const long SectorSize = 512;
        private const long TotalSize = SectorCount * SectorSize;
        private const long BitmapSize = SectorCount / 8;

        public static void Initiate()
        {
            Stream.SetLength(TotalSize);
            Stream.Position = 0;

            var tmp = BitmapSize;
            while (tmp > SectorSize)
            {
                BitmapSectors++;
                tmp /= SectorSize;
            }
            RootOffset = BitmapSectors;
            Stream.Position = RootOffset * SectorSize;
            Bw.Write("Root");
            UpdateBitmap();
        }
        public static void CreateFile(string fileName, string fileContents)
        {
            if(fileName == null)
                return;
            //1 find free sector using bitmap
            //2 change stream position to the free sector
            //3 write file name and contents using Bw
            //4 save file offset to Root
            //5 update the bitmap
            long fileSize = fileContents.Length;
            var requiredSectors = 1;
            while (fileSize > SectorSize)
            {
                requiredSectors++;
                fileSize -= SectorSize;
            }
            var writeOffset = Bitmap.FindFreeSector(Br, (int)BitmapSectors, (int)SectorSize);

            Stream.Position = writeOffset;
            var firstFileOffset = requiredSectors;
            Bw.Write(true);
            Bw.Write(fileName + ".txt");

            Stream.Position = writeOffset + fileName.Length + 6;
            Bw.Write(fileContents != null? fileContents: "");

            UpdateRoot(writeOffset);
            UpdateBitmap();
        }

        private static void UpdateBitmap()
        {
            Stream.Position = 0;
            Bitmap.UpdateBitmap(new BinaryReader(Stream), (int)SectorSize, (int)BitmapSectors, (int)SectorCount);
        }

        private static void UpdateRoot(long fileOffset)
        {
            Stream.Position = RootOffset + 6 + _rootFileAddressOffset;
            Stream.WriteByte((byte)fileOffset);
            _rootFileAddressOffset++;
        }

        public static FileStream GetFileStream()
        {
            return Stream;
        }
    }
}
