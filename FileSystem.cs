using System.Text;
using Kursova.Forms;
using Microsoft.VisualBasic;

namespace Kursova
{
    internal static class FileSystem
    {
        //TODO LIST:
        //TODO bitmap (sometimes) throwing error when writing long files
        //TODO can't delete file if it's larger than 16 sectors
        //TODO if there are dirs in CWD -> to delete CWD you need to delete the dirs inside first
        //TODO when viewing large files unknown char at end

        private static readonly FileStream Stream = File.Create("C:\\Users\\PiwKi\\Desktop\\fs_file");
        private static readonly BinaryWriter Bw = new(Stream, Encoding.UTF8, true);
        private static readonly BinaryReader Br = new(Stream, Encoding.UTF8, true);
        private static long BitmapSectors { get; set; } = 1;
        private static long RootOffset { get; set; }
        //private static int _rootFileAddressOffset;

        private const long SectorCount = 5120;
        private const long SectorSize = 512;
        private const long TotalSize = SectorCount * SectorSize;
        private const long BitmapSize = SectorCount / 8;

        internal static void Initiate()
        {
            Stream.SetLength(TotalSize);
            Stream.Position = 0;

            var tmp = BitmapSize;
            while (tmp > SectorSize)
            {
                BitmapSectors++;
                tmp /= SectorSize;
            }
            RootOffset = BitmapSectors * SectorSize + 1;
            Stream.Position = RootOffset;
            Bw.Write("Root");
            Stream.Position = RootOffset + (SectorSize - 8);
            Bw.Write((long)-1);
            UpdateBitmap();
        }

        internal static void CreateFile(string? fileName, string fileContents)
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

                UpdateDir(writeOffset, MainForm.CWD);
                UpdateBitmap();
                MainForm.AddTreeviewNodes(fileName, writeOffset, true);
            }
        }

        internal static void CreateDirectory(string? dirName)
        {
            if (dirName == null) return;
            //find free sector
            //change stream position to it
            //write true byte and dir name
            //update the bitmap
            var writeOffset = Bitmap.FindFreeSector(Br, (int)BitmapSectors, (int)SectorSize);
            Stream.Position = writeOffset;
            Bw.Write(false);
            Bw.Write(dirName.ToCharArray());

            UpdateDir(writeOffset, MainForm.CWD);
            Stream.Position = writeOffset + SectorSize - 8;
            Bw.Write((long)-1);

            UpdateBitmap();
            MainForm.AddTreeviewNodes(dirName, writeOffset, false);
        }

        internal static FileStream GetFileStream() => Stream;

        internal static long GetRootOffset() => RootOffset;

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
            long nextSector = 0;
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

        internal static void DeleteObject(TreeNode? obj)
        {
            if (obj == null) return;
            var objOffset = (long)obj.Tag;
            //File
            if (obj.ForeColor == Color.Green)
            {
                DeleteFile(objOffset, (long)obj.Parent.Tag, obj.Parent.Text.Length);
                MainForm.DeleteNode(obj);
            }
            //Directory
            else
            {
                //check if dir empty.
                var nameLength = obj.Text.Length;
                if (IsDirEmpty(objOffset, nameLength))
                {
                    //empty -> delete dir
                    Stream.Position = objOffset;
                    for (var i = 0; i < nameLength; i += 8)
                        Bw.Write((long)0);//long because 8bytes at a time -> fewer cycles
                    //clear end of sector value
                    Stream.Position = objOffset + SectorSize - 8;
                    Bw.Write((long)0);
                    CleanParentDir(objOffset, (long)obj.Parent.Tag, obj.Parent.Text.Length);
                }
                //not empty -> run DeleteFile with all offsets from directory
                else
                {
                    var currFileOffset = objOffset + nameLength + 1;
                    while (currFileOffset != 0)
                    {
                        Stream.Position = currFileOffset;
                        var bytes = Br.ReadBytes(8);
                        currFileOffset = BitConverter.ToInt64(bytes , 0);
                        DeleteFile(currFileOffset, (long)obj.Parent.Tag, obj.Parent.Text.Length);
                    }
                    //delete dir
                    Stream.Position = objOffset;
                    for (var i = 0; i < SectorSize; i += 8)
                        Bw.Write((long)0);
                    CleanParentDir(objOffset, (long)obj.Parent.Tag, obj.Parent.Text.Length);
                }
                MainForm.DeleteNode(obj);
            }
            UpdateBitmap();
        }

        private static void DeleteFile(long fileOffset, long parentOffset, int parentNameLength)
        {
            if (fileOffset < 1536)
                return;
            var offsets = new long[16]; 
            offsets[0] = fileOffset;
            Stream.Position = fileOffset + (SectorSize - 8);
            //read sectors and save all offsets of the file
            long nextSector = default;
            var indx = 1;
            while (nextSector != -1)
            {
                var bytes = Br.ReadBytes(8);
                nextSector = BitConverter.ToInt64(bytes , 0);
                if (nextSector == -1) break;
                offsets[indx++] = nextSector;
                Stream.Position = nextSector + (SectorSize - 8);
            }
            //replace sectors with 0 bytes
            foreach (var currOffset in offsets)
            {
                Stream.Position = currOffset;
                for (var i = 0; i < SectorSize; i += 8)
                    Bw.Write((long)0);//long because 8bytes at a time -> fewer cycles
            }
            //delete offset from parent directory
            CleanParentDir(fileOffset, parentOffset, parentNameLength);
        }

        private static bool IsDirEmpty(long offset, int nameLength)
        {
            Stream.Position = offset + nameLength + 1;
            //stream is between name and end of sector value
            while (Stream.Position < offset + (SectorSize - nameLength - 9))
            {
                var currBytes = Br.ReadBytes(8);
                var value = BitConverter.ToInt64(currBytes , 0);
                if (value != 0)
                    return false;
            }
            return true;
        }

        private static void CleanParentDir(long objOffset, long parentOffset, int parentNameLength)
        {
            //read dir and search for offset and delete it
            Stream.Position = parentOffset + parentNameLength + 1;
            while (Stream.Position < objOffset + (SectorSize - parentNameLength - 9))
            {
                var currBytes = Br.ReadBytes(8);
                var value = BitConverter.ToInt64(currBytes , 0);
                if (value != objOffset)
                    continue;
                else
                {
                    Stream.Position -= 8;
                    Bw.Write((long)0);
                }
            }
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
                    break;
                Bw.Write(strings[i].ToCharArray());
                if (i >= writeOffsets.Length - 1) 
                    continue;
                Bw.Write(writeOffsets[i + 1]);
                Stream.Position = writeOffsets[i + 1];
            }
            //special value for end of last sector
            Stream.Position = writeOffsets[^2] + SectorSize - 8;
            Bw.Write((long)-1);

            UpdateDir(writeOffsets[0], MainForm.CWD);
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

        private static void UpdateDir(long fileOffset, TreeNode CWD)
        {
            Stream.Position = (long)CWD.Tag + CWD.Text.Length + 1;
            var freeBytesOffset = -1;
            for (var i = 0; i < 63; i++)//512/8 = 64(8 bytes per offset) - 1 byte for end of file/dir value
            {
                var bytes = Br.ReadBytes(8);
                if (BitConverter.ToInt64(bytes , 0) != 0)
                    continue;
                break;
            }

            Stream.Position -= 8;
            Bw.Write(fileOffset);
        }
        
        private static void UpdateBitmap()
        {
            Stream.Position = 0;
            Bitmap.UpdateBitmap(new BinaryReader(Stream), (int)SectorSize, (int)BitmapSectors, (int)SectorCount);
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
