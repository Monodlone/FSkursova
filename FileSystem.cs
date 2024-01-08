using System.Text;
using Kursova.Forms;

namespace Kursova
{
    //TODO DON'T DEPEND ON TREEVIEW
    //read dir contents without using TreeView only sector offset
    //be able to resume the filesystem after closing and opening the project

    internal static class FileSystem
    {
        private static readonly FileStream Stream = File.Create("C:\\Users\\PiwKi\\Desktop\\fs_file");
        private static readonly BinaryWriter Bw = new(Stream, Encoding.UTF8, true);
        private static readonly BinaryReader Br = new(Stream, Encoding.UTF8, true);
        private static long BitmapSectors { get; set; } = 1;
        private static long RootOffset { get; set; }

        private static long _sectorCount;
        private static long _sectorSize;
        private static long _totalSize;
        private static long _bitmapSize;

        private const int NameLength = 15;

        internal static void Initiate(long sectorSize, long sectorCount)
        {
            _sectorCount = sectorCount;
            _sectorSize = sectorSize;
            _totalSize = _sectorCount * _sectorSize;
            Stream.SetLength(_totalSize);
            _bitmapSize = _sectorCount / sizeof(long);
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
            Stream.Position = RootOffset + (_sectorSize - sizeof(long));
            Bw.Write((long)-1);
            UpdateBitmap();
        }

        internal static void CreateFile(string? fileName, string fileContents)
        {
            if (fileName == null) return;
            //find free sector using bitmap
            //change stream position to the free sector
            //write file name and contents using Bw
            //at end of sector write address of next sector of contents or special value if no next
            //save file offset to Root
            //update the bitmap
            long fileSize = 1 + NameLength + fileContents.Length;
            var requiredSectors = 1;
            var counter = 0;
            while (fileSize > _sectorSize - 1 - sizeof(long))
            {
                requiredSectors++;
                fileSize -= _sectorSize - 1 - sizeof(long);
                counter++;
                if (counter < 0)
                {
                    counter++;
                    fileSize -= 1;
                }
            }
            if (requiredSectors > 1)
                CreateLongFile(fileName, fileContents, requiredSectors);
            else
            {
                var writeOffset = Bitmap.FindFreeSector(Br, (int)BitmapSectors, (int)_sectorSize);

                Stream.Position = writeOffset;
                Bw.Write(true);
                Bw.Write(fileName.ToCharArray());
                Stream.Position = writeOffset + 1 + NameLength;

                if (fileContents != null)
                    Bw.Write(fileContents.ToCharArray());

                //write special value at end of sector
                Stream.Position = writeOffset + _sectorSize - sizeof(long);
                Bw.Write((long)-1);

                ParityCheck.WriteParityBit(writeOffset, _sectorSize);
                UpdateDir(writeOffset, MainForm.CWD);
                UpdateBitmap();

                if (MainForm.CWD.ForeColor == MainForm.BadObjColor)
                    MainForm.ChangeToRootWhenCwdBad();

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
            Stream.Position = writeOffset + 1 + NameLength;

            UpdateDir(writeOffset, MainForm.CWD);
            Stream.Position = writeOffset + _sectorSize - sizeof(long);
            Bw.Write((long)-1);

            ParityCheck.WriteParityBit(writeOffset, _sectorSize);
            UpdateBitmap();

            MainForm.AddTreeviewNodes(dirName, writeOffset, false);
        }

        internal static FileStream GetStream() => Stream;

        internal static long GetRootOffset() => RootOffset;

        internal static string[]? ReadFile(long offset)
        {
            if (!ParityCheck.CheckSectorIntegrity(offset, _sectorSize))
                return null;

            var info = new string[2];
            Stream.Position = offset + 1;

            info[0] = MyToString(Br.ReadChars(NameLength));

            var contents = "";
            long nextSector = 0;
            var readLength = (int)_sectorSize - 1 - NameLength - 1 - sizeof(long);
            while (nextSector != -1)
            {
                contents += MyToString(Br.ReadChars(readLength));
                Stream.Position++;// skip parity bit
                var bytes = Br.ReadBytes(sizeof(long));
                nextSector = BitConverter.ToInt64(bytes , 0);

                if (nextSector == -1)
                    break;

                Stream.Position = nextSector;
                readLength = (int)_sectorSize - 1 - sizeof(long);
            }
            info[1] = MyCutString(contents);
            return info;
        }

        internal static void RemoveOffsetFromParent(long fileOffset, TreeNode parentNode, long parentOffset, int parentNameLength)
        {
            //scan all offsets until it finds fileOffset
            //replace with (long)0
            Stream.Position = parentOffset + parentNameLength + 1;
            for (var i = parentNameLength; i < _sectorSize - sizeof(long); i+=sizeof(long))
            {
                var currOffsetBytes = Br.ReadBytes(sizeof(long));
                var currOffset = BitConverter.ToInt64(currOffsetBytes, 0);

                if (currOffset != fileOffset)
                    continue;

                Stream.Position -= sizeof(long);
                Bw.Write((long)0);
                break;
            }

            ParityCheck.UpdateParityBitOfCWD(parentOffset, _sectorSize);
        }

        internal static void UpdateDir(long fileOffset, TreeNode cwd)
        {
            if (!ParityCheck.CheckSectorIntegrity((long)cwd.Tag, _sectorSize))
            {
                cwd.ForeColor = MainForm.BadObjColor;
                cwd = MainForm.RootNode;
                //check root too before continuing
                if (!ParityCheck.CheckSectorIntegrity((long)cwd.Tag, _sectorSize))
                {
                    MessageBox.Show("Fatal Error: Root is corrupted");
                    throw new ArgumentException("Fatal Error: Root is corrupted");
                }
            }
            Stream.Position = (long)cwd.Tag + cwd.Text.Length + 1;
            var isFull = false;
            for (var i = 0; i < _sectorSize/sizeof(long) - sizeof(long); i++)
            {
                var bytes = Br.ReadBytes(sizeof(long));
                if (i == _sectorSize/sizeof(long) - sizeof(long) - 1 && BitConverter.ToInt64(bytes , 0) != 0)
                    isFull = true;
                if (BitConverter.ToInt64(bytes , 0) != 0)
                    continue;
                break;
            }

            if (isFull)
            {
                MessageBox.Show("Error: Directory is full");
                return;
            }
            Stream.Position -= sizeof(long);
            Bw.Write(fileOffset);
            //delete old parity bit then calculate and write new one
            ParityCheck.UpdateParityBitOfCWD((long)cwd.Tag, _sectorSize);
        }

        internal static void DeleteObject(TreeNode? obj)
        {
            if (obj == null) return;
            var objOffset = (long)obj.Tag;
            Stream.Position = objOffset;
            //File
            if (obj.ForeColor == MainForm.FileColor ||
                //dirs can have BadObjColor too so read first byte to make sure it's a file
                (obj.ForeColor == MainForm.BadObjColor && Br.ReadByte() == 1))
            {
                DeleteFile(objOffset, (long)obj.Parent.Tag, obj.Parent.Text.Length);
                UpdateBitmap();
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
                    for (var i = 0; i < nameLength; i += sizeof(long))
                        Bw.Write((long)0);//long because 8bytes at a time -> fewer cycles

                    //clear end of sector value
                    Stream.Position = objOffset + _sectorSize - sizeof(long);
                    Bw.Write((long)0);
                    CleanParentDir(objOffset, (long)obj.Parent.Tag, obj.Parent.Text.Length);
                }
                //not empty -> run DeleteFile with all offsets from directory
                else
                {
                    var currPosition = objOffset + 1 + nameLength;
                    
                    while (true)//check if there are child dirs inside the curr dir
                    {
                        Stream.Position = currPosition;
                        currPosition += 8;
                        if (currPosition <= _sectorSize - sizeof(long) - 1)
                            break;
                        var bytes = Br.ReadBytes(sizeof(long));
                        var fileOffset = BitConverter.ToInt64(bytes , 0);

                        if (fileOffset == 0)//empty space
                            continue;
                        if (fileOffset <= -1)//end of sector
                            break;

                        Stream.Position = fileOffset;
                        var firstByte = Br.ReadByte();

                        if (firstByte == 0)
                        {
                            MessageBox.Show("Error: Delete child directories first.");
                            return;
                        }
                    }

                    currPosition = objOffset + 1 + nameLength;
                    while (true)
                    {
                        Stream.Position = currPosition;
                        currPosition += 8;
                        var bytes = Br.ReadBytes(sizeof(long));
                        var fileOffset = BitConverter.ToInt64(bytes , 0);

                        if (fileOffset == 0)//empty space
                            continue;
                        if (fileOffset <= -1)//end of sector
                            break;

                        DeleteFile(fileOffset, (long)obj.Parent.Tag, obj.Parent.Text.Length);
                    }
                    //delete dir
                    Stream.Position = objOffset;
                    for (var i = 0; i < _sectorSize; i += sizeof(long))
                        Bw.Write((long)0);
                    CleanParentDir(objOffset, (long)obj.Parent.Tag, obj.Parent.Text.Length);
                }
                UpdateBitmap();
                MainForm.DeleteNode(obj);
            }
        }

        private static void DeleteFile(long fileOffset, long parentOffset, int parentNameLength)
        {
            if (fileOffset < RootOffset)
                return;
            var offsets = new long[32]; 
            offsets[0] = fileOffset;
            Stream.Position = fileOffset + (_sectorSize - sizeof(long));
            //read sectors and save all offsets of the file
            long nextSector = 0;
            var indx = 1;
            while (nextSector != -1)
            {
                var bytes = Br.ReadBytes(sizeof(long));
                nextSector = BitConverter.ToInt64(bytes , 0);
                if (nextSector == -1) break;
                offsets[indx++] = nextSector;
                Stream.Position = nextSector + (_sectorSize - sizeof(long));
            }
            //replace sectors with 0 bytes
            foreach (var currOffset in offsets)
            {
                Stream.Position = currOffset;
                for (var i = 0; i < _sectorSize; i += sizeof(long))
                    Bw.Write((long)0);//long because 8bytes at a time -> fewer cycles
            }
            //delete offset from parent directory
            CleanParentDir(fileOffset, parentOffset, parentNameLength);
        }

        private static bool IsDirEmpty(long offset, int nameLength)
        {
            Stream.Position = offset + nameLength + 1;
            //stream is between name and parity bit
            while (Stream.Position < offset + (_sectorSize - nameLength - 1 - sizeof(long)))
            {
                var currBytes = Br.ReadBytes(sizeof(long));
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
            while (Stream.Position < objOffset + (_sectorSize - 1 - parentNameLength - sizeof(long)))
            {
                var currBytes = Br.ReadBytes(sizeof(long));
                var value = BitConverter.ToInt64(currBytes , 0);
                if (value != objOffset)
                    continue;
                else
                {
                    Stream.Position -= sizeof(long);
                    Bw.Write((long)0);
                }
            }
            ParityCheck.UpdateParityBitOfCWD(parentOffset, _sectorSize);
        }

        private static void CreateLongFile(string? fileName, string fileContents, int requiredSectors)
        {
            if (fileName == null) return;

            var writeOffsets = Bitmap.FindFreeSectors(Br, requiredSectors, (int)BitmapSectors, (int)_sectorSize);
            var strings = SplitString(fileContents, requiredSectors);
            Stream.Position = writeOffsets[0];
            Bw.Write(true);
            Bw.Write(fileName.ToCharArray());
            Stream.Position = writeOffsets[0] + 1 + NameLength;

            for (var i = 0; i < requiredSectors; i++)
            {
                Bw.Write(strings[i].ToCharArray());

                if (i < writeOffsets.Length - 1)
                {
                    Stream.Position++;
                    Bw.Write(writeOffsets[i + 1]);
                    ParityCheck.WriteParityBit(writeOffsets[i], _sectorSize);
                    Stream.Position = writeOffsets[i + 1];
                }
            }
            //special value for end of last sector
            Stream.Position = writeOffsets[^1] + _sectorSize - sizeof(long);
            Bw.Write((long)-1);

            ParityCheck.WriteParityBit(writeOffsets[^1], _sectorSize);
            UpdateDir(writeOffsets[0], MainForm.CWD);
            UpdateBitmap();

            if (MainForm.CWD.ForeColor == MainForm.BadObjColor)
                MainForm.ChangeToRootWhenCwdBad();

            MainForm.AddTreeviewNodes(fileName, writeOffsets[0], true);
        }

        private static string[] SplitString(string str, int requiredSectors)
        {
            var strs = new string[requiredSectors];
            var firstPart = "";
            var indx = 0;
            int i;
            for (i = 0; i < _sectorSize - NameLength - sizeof(long) - 2; i++)
                firstPart += str[i];
            strs[indx++] = firstPart;

            var counter = 0;
            for (; i < str.Length; i++)
            {
                strs[indx] += str[i];
                counter++;
                if (counter == _sectorSize - sizeof(long) - 1)
                {
                    indx++;
                    counter = 0;
                }
            }
            return strs;
        }
        
        private static void UpdateBitmap()
        {
            Stream.Position = 0;
            Bitmap.UpdateBitmap(new BinaryReader(Stream), (int)_sectorSize, (int)BitmapSectors, (int)_sectorCount, RootOffset);
        }

        private static string MyToString(char[] chars)
        {
            var str = "";
            foreach (var ch in chars)
                str += ch;
            return MyCutString(str);
        }

        private static string MyCutString(string str)
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
