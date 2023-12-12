using System.Text;
using Kursova.Forms;

namespace Kursova
{
    internal static class FileSystem
    {
        //TODO LIST:
        //TODO bitmap throwing some error I've never seen before when writing long files (sometimes)

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
                WriteLongFile(fileName, fileContents, requiredSectors + 1);
            else
            {
                var writeOffset = Bitmap.FindFreeSector(Br, (int)BitmapSectors, (int)SectorSize);

                Stream.Position = writeOffset;
                Bw.Write(true);
                Bw.Write((fileName + ".txt").ToCharArray());

                var contents = fileContents.ToCharArray();
                Bw.Write(contents != null? contents: null);

                //write special value at end of sector
                Stream.Position = writeOffset + SectorSize - 8;
                Bw.Write((long)-1);

                UpdateRoot((writeOffset / SectorSize) + 1);
                UpdateBitmap();
                MainForm.AddTreeviewNodes(fileName, writeOffset, true);
            }
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

            MainForm.AddTreeviewNodes(dirName, writeOffset, false);
        }

        public static FileStream GetFileStream() => Stream;

        public static long GetRootOffset() => RootOffset;

        internal static string[]? ReadFile(long offset, string fullName)
        {
            var info = new string[2];
            Stream.Position = offset;

            var check = Br.ReadByte();//true for files, false for directories
            if (check != 1) return null;

            var name = MyToString(Br.ReadChars(fullName.Length));
            if (name != fullName) return null;
            info[0] = name;

            var contents = "";
            long nextSector = default;
            var readLength = (int)SectorSize - fullName.Length - 9;
            while (nextSector != -1)
            {
                contents += MyToString(Br.ReadChars(readLength));
                var bytes = Br.ReadBytes(8);
                nextSector = BitConverter.ToInt64(bytes , 0);

                if (nextSector == -1) break;
                Stream.Position = nextSector;
                readLength = (int)SectorSize - 8;
            }

            info[1] = contents;
            return info;
        }

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
            Bw.Write((fileName + ".txt").ToCharArray());

            for (var i = 0; i < requiredSectors; i++)
            {
                if(strings[i] == null)
                    continue;
                Bw.Write(strings[i].ToCharArray());
                if (i >= writeOffsets.Length - 1) 
                    continue;
                Bw.Write(writeOffsets[i + 1]);
                Stream.Position = writeOffsets[i + 1];
            }
            //special value for end of last sector
            Stream.Position = writeOffsets[^1] + SectorSize - 8;//^1 points to last element
            Bw.Write((long)-1);

            UpdateRoot(writeOffsets[0]);
            UpdateBitmap();
            MainForm.AddTreeviewNodes(fileName, writeOffsets[0], true);
        }

        private static string[] SplitString(string str, int nameSize, int requiredSectors)
        {
            var strs = new string[requiredSectors];
            var firstPart = "";
            var indx = 0;
            int i;
            for (i = 0; i <= SectorSize - nameSize - 9; i++)
                firstPart += str[i];
            strs[indx++] = firstPart;

            var counter = 0;
            for (; i < str.Length; i++)
            {
                strs[indx] += str[i];
                counter++;
                if (counter == SectorSize - 8)
                {
                    indx++;
                    counter = 0;
                }
            }
            return strs;
        }

        private static void UpdateRoot(long fileOffset)
        {
            Stream.Position = (RootOffset * SectorSize) + _rootFileAddressOffset + 5;
            Bw.Write(fileOffset);
            _rootFileAddressOffset++;
        }

        private static string MyToString(char[] chars)
        {
            var str = "";
            foreach (var ch in chars)
                str += ch;

            return str;
        }
    }
}
