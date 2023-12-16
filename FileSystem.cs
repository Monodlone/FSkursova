using System.Text;
using Kursova.Forms;

namespace Kursova
{
    internal static class FileSystem
    {
        //TODO Problem LIST: 
        //bitmap (sometimes) throws error when writing long files
        //can't delete file if it's larger than 16 sectors
        //can't edit directories (maybe make it so the new name can't be longer than the old one)
        //if there are dirs in CWD -> to delete CWD you need to delete the dirs inside first
        //Make restrictions for the file and dir names

        //TODO ForImplementing LIST:
        //Resiliency

        private static readonly FileStream Stream = File.Create("C:\\Users\\PiwKi\\Desktop\\fs_file");
        private static readonly BinaryWriter Bw = new(Stream, Encoding.UTF8, true);
        private static readonly BinaryReader Br = new(Stream, Encoding.UTF8, true);
        private static long BitmapSectors { get; set; } = 1;
        private static long RootOffset { get; set; }

        private static long _sectorCount;
        private static long _sectorSize;
        private static long _totalSize;
        private static long _bitmapSize;

        internal static void Initiate(long sectorSize, long sectorCount)
        {
            _sectorCount = sectorCount;
            _sectorSize = sectorSize;
            _totalSize = _sectorCount * _sectorSize;
            _bitmapSize = _sectorCount / 8;
            Stream.SetLength(_totalSize);
            Stream.Position = 0;
            var tmp = _bitmapSize;
            while (tmp > _sectorSize)
            {
                BitmapSectors++;
                tmp -= _sectorSize;
            }
            RootOffset = BitmapSectors * _sectorSize + 1;
            Stream.Position = RootOffset;
            Bw.Write("Root");
            Stream.Position = RootOffset + (_sectorSize - 8);
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
            while (fileSize > _sectorSize)
            {
                requiredSectors++;
                fileSize -= _sectorSize;
            }
            if (requiredSectors > 1)
                WriteLongFile(fileName, fileContents, requiredSectors + 1);
            else
            {
                var writeOffset = Bitmap.FindFreeSector(Br, (int)BitmapSectors, (int)_sectorSize);

                Stream.Position = writeOffset;
                Bw.Write(true);
                Bw.Write(fileName.ToCharArray());

                if (fileContents != null)
                    Bw.Write(fileContents.ToCharArray());

                //write special value at end of sector
                Stream.Position = writeOffset + _sectorSize - 8;
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
            var writeOffset = Bitmap.FindFreeSector(Br, (int)BitmapSectors, (int)_sectorSize);
            Stream.Position = writeOffset;
            Bw.Write(false);
            Bw.Write(dirName.ToCharArray());

            UpdateDir(writeOffset, MainForm.CWD);
            Stream.Position = writeOffset + _sectorSize - 8;
            Bw.Write((long)-1);

            UpdateBitmap();
            MainForm.AddTreeviewNodes(dirName, writeOffset, false);
        }

        internal static FileStream GetFileStream() => Stream;

        internal static long GetRootOffset() => RootOffset;

        internal static string[] ReadFile(long offset, string fileName)
        {
            var info = new string[2];
            Stream.Position = offset + 1;

            info[0] = MyToString(Br.ReadChars(fileName.Length));

            var contents = "";
            long nextSector = 0;
            var readLength = (int)_sectorSize - fileName.Length - 9;
            while (nextSector != -1)
            {
                contents += MyToString(Br.ReadChars(readLength));
                var bytes = Br.ReadBytes(8);
                nextSector = BitConverter.ToInt64(bytes , 0);

                if (nextSector == -1) break;
                Stream.Position = nextSector;
                readLength = (int)_sectorSize - 8;
            }
            info[1] = CutString(contents);
            //info[1] = contents;
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
                    Stream.Position = objOffset + _sectorSize - 8;
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
                    for (var i = 0; i < _sectorSize; i += 8)
                        Bw.Write((long)0);
                    CleanParentDir(objOffset, (long)obj.Parent.Tag, obj.Parent.Text.Length);
                }
                MainForm.DeleteNode(obj);
            }
            UpdateBitmap();
        }

        private static void DeleteFile(long fileOffset, long parentOffset, int parentNameLength)
        {
            if (fileOffset < RootOffset)
                return;
            var offsets = new long[16]; 
            offsets[0] = fileOffset;
            Stream.Position = fileOffset + (_sectorSize - 8);
            //read sectors and save all offsets of the file
            long nextSector = default;
            var indx = 1;
            while (nextSector != -1)
            {
                var bytes = Br.ReadBytes(8);
                nextSector = BitConverter.ToInt64(bytes , 0);
                if (nextSector == -1) break;
                offsets[indx++] = nextSector;
                Stream.Position = nextSector + (_sectorSize - 8);
            }
            //replace sectors with 0 bytes
            foreach (var currOffset in offsets)
            {
                Stream.Position = currOffset;
                for (var i = 0; i < _sectorSize; i += 8)
                    Bw.Write((long)0);//long because 8bytes at a time -> fewer cycles
            }
            //delete offset from parent directory
            CleanParentDir(fileOffset, parentOffset, parentNameLength);
        }

        private static bool IsDirEmpty(long offset, int nameLength)
        {
            Stream.Position = offset + nameLength + 1;
            //stream is between name and end of sector value
            while (Stream.Position < offset + (_sectorSize - nameLength - 9))
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
            while (Stream.Position < objOffset + (_sectorSize - parentNameLength - 9))
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

            var writeOffsets = Bitmap.FindFreeSectors(Br, requiredSectors, (int)BitmapSectors, (int)_sectorSize);
            var strings = SplitString(fileContents, fileName.Length, requiredSectors);
            Stream.Position = writeOffsets[0];
            Bw.Write(true);
            Bw.Write(fileName.ToCharArray());

            for (var i = 0; i < requiredSectors; i++)
            {
                if(strings[i] == null) break;

                Bw.Write(strings[i].ToCharArray());

                if (i >= writeOffsets.Length - 2) break;

                Bw.Write(writeOffsets[i + 1]);
                Stream.Position = writeOffsets[i + 1];
            }
            //special value for end of last sector
            Stream.Position = writeOffsets[^2] + _sectorSize - 8;
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
            for (i = 0; i < _sectorSize - nameSize - 9; i++)
                firstPart += str[i];
            strs[indx++] = firstPart;

            var counter = 0;
            for (; i < str.Length; i++)
            {
                strs[indx] += str[i];
                counter++;
                if (counter == _sectorSize - 8)
                {
                    indx++;
                    counter = 0;
                }
            }
            return strs;
        }

        private static void UpdateDir(long fileOffset, TreeNode cwd)
        {
            Stream.Position = (long)cwd.Tag + cwd.Text.Length + 1;
            var freeBytesOffset = -1;
            for (var i = 0; i < _sectorSize/8 - 1; i++)//8 bytes per offset - 1 byte for end of file/dir value
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
            Bitmap.UpdateBitmap(new BinaryReader(Stream), (int)_sectorSize, (int)BitmapSectors, (int)_sectorCount);
        }

        private static string MyToString(char[] chars)
        {
            var str = "";
            foreach (var ch in chars)
                str += ch;
            return str;
        }

        private static string CutString(string str)
        {
            //remove trailing empty bytes
            var cutStr = "";
            foreach (var t in str)
            {
                if (t == 0) break;
                cutStr += t;
            }
            return cutStr;
        }
    }
}
