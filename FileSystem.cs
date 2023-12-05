using System.Text;

namespace Kursova
{

    internal static class FileSystem
    {
        //TODO LIST:
        //TODO when adding file or new dir to current dir update it just like root
        //TODO bitmap can't look for multiple sectors for big files (linked list / offsets at end of sector)
        //TODO can't use the built-in stuff: line 10 MainForm.cs

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

        public static void CreateFile(string? fileName, string fileContents)
        {
            if(fileName == null) return;
            //1 find free sector using bitmap
            //2 change stream position to the free sector
            //3 write file name and contents using Bw
            //4 save file offset to Root
            //5 update the bitmap
            long fileSize = fileName.Length + fileContents.Length;
            var requiredSectors = 1;
            while (fileSize > SectorSize)
            {
                requiredSectors++;
                fileSize -= SectorSize;
            }
            var writeOffset = Bitmap.FindFreeSector(Br, requiredSectors, (int)BitmapSectors, (int)SectorSize);

            Stream.Position = writeOffset;
            Bw.Write(true);
            Bw.Write(fileName + ".txt");

            Stream.Position = writeOffset + fileName.Length + 6;
            Bw.Write(fileContents != null? fileContents: "");

            UpdateRoot((writeOffset / SectorSize) + 1);
            UpdateBitmap();
            
        }

        public static void CreateDirectory(string? dirName)
        {
            if(dirName == null) return;
            //find free sector
            //change stream position to it
            //write true byte and dir name
            //update the bitmap
            var writeOffset = Bitmap.FindFreeSector(Br, 1, (int)BitmapSectors, (int)SectorSize);
            Stream.Position = writeOffset;
            Bw.Write(false);
            Bw.Write(dirName);

            UpdateRoot((writeOffset / SectorSize) + 1);
            UpdateBitmap();
        }

        public static FileStream GetFileStream() => Stream;

        private static void UpdateBitmap()
        {
            Stream.Position = 0;
            Bitmap.UpdateBitmap(new BinaryReader(Stream), (int)SectorSize, (int)BitmapSectors, (int)SectorCount);
        }

        private static void UpdateRoot(long fileOffset)
        {
            Stream.Position = (RootOffset * SectorSize) + _rootFileAddressOffset + 5;
            Bw.Write((byte)fileOffset);
            _rootFileAddressOffset++;
        }
    }
}
