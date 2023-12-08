using System.Text;
using Kursova.Forms;

namespace Kursova
{
    internal static class FileSystem
    {
        //TODO LIST:
        //TODO have a way to keep track of CWD
        //TODO make the treeview stuff for other directories
        //TODO make sure the math isn't wrong when writing at the offsets for files

        private static readonly FileStream Stream = File.Create("C:\\Users\\PiwKi\\Desktop\\fs_file");
        private static readonly BinaryWriter Bw = new(Stream, Encoding.UTF8, true);
        private static readonly BinaryReader Br = new(Stream, Encoding.UTF8, true);
        private static long BitmapSectors { get; set; } = 1;
        private static long RootOffset { get; set; }
        private static int _rootFileAddressOffset;

        private const long SectorCount = 5120;
        private const long SectorSize = 512;
        private const long TotalSize = SectorCount * SectorSize;
        private const long BitmapSize = SectorCount / 8;

        private static long _currObjOffset;

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
            if (fileName == null) return;
            //1 find free sector using bitmap
            //2 change stream position to the free sector
            //3 write file name and contents using Bw
            //3.1 at end of sector write address of next sector of contents or special value if no next
            //4 save file offset to Root
            //5 update the bitmap
            long fileSize = fileName.Length + fileContents.Length;
            var requiredSectors = 1;
            while (fileSize > SectorSize)
            {
                requiredSectors++;
                fileSize -= SectorSize;
            }

            if (requiredSectors > 1)
                WriteLongFile(fileName, fileContents, requiredSectors);
            else
            {
                var writeOffset = Bitmap.FindFreeSector(Br, (int)BitmapSectors, (int)SectorSize);

                Stream.Position = writeOffset;
                Bw.Write(true);
                Bw.Write(fileName + ".txt");

                Bw.Write(fileContents != null? fileContents: "");
                
                //write special value at end of sector
                Stream.Position = writeOffset + SectorSize - 2;
                Bw.Write((sbyte)-128);

                UpdateRoot((writeOffset / SectorSize) + 1);
                UpdateBitmap();
            }

            MainForm.AddTreeviewNodes(fileName, true);
        }

        public static void CreateDirectory(string? dirName)
        {
            if (dirName == null) return;
            //find free sector
            //change stream position to it
            //write true byte and dir name
            //update the bitmap
            var writeOffset = Bitmap.FindFreeSector(Br, (int)BitmapSectors, (int)SectorSize);
            Stream.Position = writeOffset;
            Bw.Write(false);
            Bw.Write(dirName);

            UpdateRoot((writeOffset / SectorSize) + 1);
            UpdateBitmap();

            MainForm.AddTreeviewNodes(dirName, false);
        }

        public static FileStream GetFileStream() => Stream;

        private static void UpdateBitmap()
        {
            Stream.Position = 0;
            Bitmap.UpdateBitmap(new BinaryReader(Stream), (int)SectorSize, (int)BitmapSectors, (int)SectorCount);
        }

        private static void WriteLongFile(string? fileName, string fileContents, int requiredSectors)
        {
            if (fileName == null) return;

            var writeOffsets = Bitmap.FindFreeSectors(Br, requiredSectors, (int)BitmapSectors, (int)SectorSize);
            var strings = SplitString(fileContents, fileName.Length + 5, requiredSectors);
            Stream.Position = writeOffsets[0];
            Bw.Write(true);
            Bw.Write(fileName + ".txt");

            for (var i = 0; i < requiredSectors; i++)
            {
                Bw.Write(strings[i]);
                if (i >= writeOffsets.Length - 1) 
                    continue;
                Bw.Write(writeOffsets[i + 1]);
                Stream.Position = writeOffsets[i + 1];
            }
            //special value for end of last sector
            Stream.Position = writeOffsets[^1] + SectorSize - 2;//^1 points to last element
            Bw.Write((long)-1);

            UpdateRoot(writeOffsets[0]);
            UpdateBitmap();
        }

        private static string[] SplitString(string str, int nameSize, int requiredSectors)
        {
            var strs = new string[requiredSectors];
            var firstPart = "";
            var indx = 0;
            int i;
            for (i = 0; i < SectorSize - nameSize - 2; i++)
                firstPart += str[i];
            strs[indx++] = firstPart;

            var counter = 0;
            for (; i < str.Length; i++)
            {
                strs[indx] += str[i];
                counter++;
                if (counter == 511)
                    indx++;
            }
            return strs;
        }

        private static void UpdateRoot(long fileOffset)
        {
            Stream.Position = (RootOffset * SectorSize) + _rootFileAddressOffset + 5;
            Bw.Write(fileOffset);
            _rootFileAddressOffset++;
        }
    }
}
