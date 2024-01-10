using System.Text;
using Kursova.Forms;

namespace Kursova
{
    //TODO DON'T DEPEND ON TREEVIEW
    //USING MEMORY FOR ITEMS LIST!!!!!!!!!!!!!

    //calculate space needed for 
    //sectorCount / 22 zakrugleno nagore = x
    //x * 22 = y
    //y / sectorSize = sectorsNeeded for items dictionary


    internal static class FileSystem
    {
        private static FileStream Stream { get; set; }
        private static BinaryWriter Bw { get; set; }
        private static BinaryReader Br { get; set; }
        private static long BitmapSectors { get; set; } = 1;
        private static long RootOffset { get; set; }

        private static long _sectorCount;
        private static long _sectorSize;
        private static long _totalSize;
        private static long _bitmapSize;
        private static long _itemListTotalSectors;
        private static readonly MyDictionary Items = new();

        internal static long CWDOffset { get; set; }
        internal const int NameLength = 14;
        internal static int ItemInfoSize = NameLength + sizeof(long);
        internal static long ItemInfoOffset;

        internal static void Initiate(long sectorSize, long sectorCount, bool restore)
        {
            _sectorSize = sectorSize;
            _sectorCount = sectorCount;
            _totalSize = _sectorCount * _sectorSize;
            _bitmapSize = _sectorCount / sizeof(long) + 1;

            //for _itemListTotalSectors and ItemInfoOffset
            var tmp1 = sectorCount / ItemInfoSize + 1;
            var tmp2 = tmp1 * ItemInfoSize;
            _itemListTotalSectors = tmp2 / sectorSize + 1;
            ItemInfoOffset = (sectorCount - _itemListTotalSectors) * sectorSize;
            //End

            if (restore)
            {
                Stream = File.Open("fsFile", FileMode.Open);
                Bw = new BinaryWriter(Stream, Encoding.UTF8, true);
                Br = new BinaryReader(Stream, Encoding.UTF8, true);
            }
            else
            {
                Stream = File.Create("fsFile");
                Bw = new BinaryWriter(Stream, Encoding.UTF8, true);
                Br = new BinaryReader(Stream, Encoding.UTF8, true);
                Stream.SetLength(_totalSize);

                //write metadata for restoring
                Stream.Position = 0;
                Bw.Write(_sectorSize);
                Bw.Write(_sectorCount);
            }

            var tmp = _bitmapSize;
            while (tmp > _sectorSize)
            {
                BitmapSectors++;
                tmp -= _sectorSize;
            }

            RootOffset = BitmapSectors * _sectorSize + 1;
            CWDOffset = RootOffset;
            if (restore)
            {
                Stream.Position = RootOffset + 1 + sizeof(long);
                var rootName = MyToString(Br.ReadChars(NameLength));

                var rootNode = new TreeNode(rootName) { Tag = RootOffset };
                MainForm.InitRoot(rootNode);

                InitializeItemList();
            }
            else
            {
                Stream.Position = RootOffset;
                Bw.Write(false);
                Bw.Write((long)-1);
                Bw.Write(MyToCharArray("Root"));
                Stream.Position = RootOffset + (_sectorSize - sizeof(long));
                Bw.Write((long)-1);
                var rootNode = new TreeNode("Root") { Tag = RootOffset };
                MainForm.InitRoot(rootNode);
                UpdateBitmap();
                ParityCheck.WriteParityBit(RootOffset, _sectorSize);

                Items.Add("Root", RootOffset);
                UpdateItemListMetadata(MyToCharArray("Root"), RootOffset);
            }
        }

        internal static void CreateFile(string? fileName, string fileContents)
        {
            if (fileName == null)
                return;

            //override old file with new one with same name
            if (Items.ContainsKey(fileName))
            {
               var offset = Items.GetValue(fileName);
               Stream.Position = offset;
               var firstByte = Br.ReadByte();

               var offsetFromBack = Items.GetValueBackwards(fileName);
               Stream.Position = offsetFromBack;
               var firstByteFromBack = Br.ReadByte();

               if (firstByte == 1)
                   DeleteFile(offset);
               else if (firstByteFromBack == 1)
                   DeleteFile(offsetFromBack);
            }
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
                Bw.Write(CWDOffset);
                Bw.Write(MyToCharArray(fileName));
                Stream.Position = writeOffset + 1 + 8 + NameLength;

                if (fileContents != null)
                    Bw.Write(MyToCharArray(fileContents));

                //write special value at end of sector
                Stream.Position = writeOffset + _sectorSize - sizeof(long);
                Bw.Write((long)-1);

                ParityCheck.WriteParityBit(writeOffset, _sectorSize);
                UpdateDir(writeOffset);
                UpdateBitmap();

                MainForm.ChangeToRootWhenIfCwdBad();

                MainForm.ResetParentDir();
                ReadDirectory(GetParentOffset(writeOffset));

                Items.Add(fileName, writeOffset);
                UpdateItemListMetadata(MyToCharArray(fileName), writeOffset);
            }
        }

        internal static void CreateDirectory(string? dirName)
        {
            if (dirName == null)
                return;

            if (Items.ContainsKey(dirName))
            {
                var offset = Items.GetValue(dirName);
                Stream.Position = offset;
                var firstByte = Br.ReadByte();

                var offsetFromBack = Items.GetValueBackwards(dirName);
                Stream.Position = offsetFromBack;
                var firstByteFromBack = Br.ReadByte();

                if (firstByte == 0)
                {
                    MessageBox.Show("Error: Directory with the same name already exists");
                    return;
                }

                if (firstByteFromBack == 0)
                {
                    MessageBox.Show("Error: Directory with the same name already exists");
                    return;
                }
            }

            //find free sector
            //change stream position to it
            //write true byte and dir name
            //update the bitmap
            var writeOffset = Bitmap.FindFreeSector(Br, (int)BitmapSectors, (int)_sectorSize);
            Items.Add(dirName, writeOffset);
            UpdateItemListMetadata(MyToCharArray(dirName), writeOffset);

            Stream.Position = writeOffset;
            Bw.Write(false);
            Bw.Write(CWDOffset);
            Bw.Write(MyToCharArray(dirName));
            Stream.Position = writeOffset + 1 + 8 + NameLength;

            UpdateDir(writeOffset);
            Stream.Position = writeOffset + _sectorSize - sizeof(long);
            Bw.Write((long)-1);

            ParityCheck.WriteParityBit(writeOffset, _sectorSize);
            UpdateBitmap();

            MainForm.ResetParentDir();
            ReadDirectory(GetParentOffset(writeOffset));

        }

        internal static string[]? ReadFile(long offset)
        {
            //if the file is in another dir and removed i have to delete the node before i get to here
            if (!ParityCheck.CheckSectorIntegrity(offset, _sectorSize))
                return null;

            var info = new string[2];
            //skip metadata
            Stream.Position = offset + 1 + 8;

            info[0] = MyToString(Br.ReadChars(NameLength));

            var contents = "";
            long nextSector = 0;
            var readLength = (int)_sectorSize - 1 - 8 - NameLength - 1 - sizeof(long);
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

        internal static void ReadDirectory(long offset)
        {
            //check first byte if there is a dir at offset
            Stream.Position = offset;
            var firstByte = Br.ReadByte();
            if (firstByte != 0)//not a dir
                return;

            if (!ParityCheck.CheckSectorIntegrity(offset, _sectorSize))
                return;

            MainForm.ResetParentDir();
            //skip metadata and read each offset inside 
            Stream.Position = offset + 1 + 8 + NameLength;
            var currOffsetBytes = Br.ReadBytes(sizeof(long));
            var currOffset = BitConverter.ToInt64(currOffsetBytes , 0);
            var counter = 2;//keep track of offsets

            while (currOffset != -256 && currOffset != -255)
            {
                if (currOffset == 0)//empty space
                {
                    //go to next 8 bytes
                    Stream.Position = offset + 1 + NameLength + (counter * 8);
                    counter++;
                    currOffsetBytes = Br.ReadBytes(sizeof(long));
                    currOffset = BitConverter.ToInt64(currOffsetBytes, 0);
                }
                else
                {
                    //go to offset and read first byte and name
                    Stream.Position = currOffset;
                    var fByte = Br.ReadByte();
                    Stream.Position += 8;
                    //Read name, create node and add to treeview
                    if (fByte == 1) //File
                    {
                        var fileName = MyToString(Br.ReadChars(NameLength));
                        MainForm.AddTreeviewNodes(fileName, currOffset, true);
                    }
                    else if (fByte == 0) //Directory
                    {
                        var dirName = MyToString(Br.ReadChars(NameLength));
                        MainForm.AddTreeviewNodes(dirName, currOffset, false);
                    }

                    Stream.Position = offset + 1 + 8 + NameLength + ((counter - 1) * 8);
                    counter++;
                    currOffsetBytes = Br.ReadBytes(sizeof(long));
                    currOffset = BitConverter.ToInt64(currOffsetBytes, 0);
                }
            }
        }

        internal static FileStream GetStream() => Stream;

        internal static long GetParentOffset(long offset)
        {
            if (offset == RootOffset)
                return -1;

            Stream.Position = offset + 1;
            var parentBytes = Br.ReadBytes(sizeof(long));
            return BitConverter.ToInt64(parentBytes, 0);;
        }

        //for moving files from one dir to another
        internal static void RemoveOffsetFromParent(long fileOffset, long parentOffset)
        {
            //scan all offsets until it finds fileOffset
            //replace with (long)0
            Stream.Position = parentOffset + NameLength + 1;
            for (var i = NameLength; i < _sectorSize - sizeof(long); i+=sizeof(long))
            {
                var currOffsetBytes = Br.ReadBytes(sizeof(long));
                var currOffset = BitConverter.ToInt64(currOffsetBytes, 0);

                if (currOffset != fileOffset)
                    continue;

                Stream.Position -= sizeof(long);
                Bw.Write((long)0);
                break;
            }

            ParityCheck.UpdateParityBit(parentOffset, _sectorSize);
        }

        internal static void UpdateDir(long fileOffset)
        {
            var parentOffset = GetParentOffset(fileOffset);
            //if target directory is corrupted switch to root
            if (!ParityCheck.CheckSectorIntegrity(parentOffset, _sectorSize))
            {
                MainForm.ChangeCWDColorToBadSectorColor();
                parentOffset = RootOffset;
                Stream.Position = fileOffset + 1;
                Bw.Write(parentOffset);
            }

            Stream.Position = parentOffset + 1 + 8 + NameLength;
            var isFull = false;
            for (var i = 1 + 8 + NameLength; i <= _sectorSize - sizeof(long) - 1; i += sizeof(long))
            {
                var bytes = Br.ReadBytes(sizeof(long));
                var currOffset = BitConverter.ToInt64(bytes, 0);

                if (i == _sectorSize - sizeof(long) - 1 && currOffset != 0)
                {
                    isFull = true;
                    break;
                }

                if (currOffset != 0)
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
            //delete old parity bit and write a new one
            ParityCheck.UpdateParityBit(parentOffset, _sectorSize);
            Stream.Position = fileOffset;
        }

        internal static void DeleteObject(TreeNode? obj)
        {
            if (obj == null)
                return;

            var objOffset = MainForm.GetOffsetOfNode(obj);

            if (CheckIfFile(objOffset))
                DeleteFile(objOffset);

            else
                DeleteDirectory(objOffset);

            UpdateBitmap();
        }

        internal static bool CheckIfFile(long offset)
        {
            Stream.Position = offset;
            return Br.ReadByte() == 1;
        }

        internal static void MoveFile(long fileOffset)
        {
            var parentOffset = GetParentOffset(fileOffset);

            if (parentOffset == CWDOffset)
                return;

            //replace old parent offset with new one in file
            Stream.Position = fileOffset + 1;
            var bw = new BinaryWriter(Stream, Encoding.UTF8);
            bw.Write(CWDOffset);
            ParityCheck.UpdateParityBit(fileOffset, _sectorSize);

            //bw.Close();
            //stream.Close();
            //remove file offset from old parent
            RemoveOffsetFromParent(fileOffset, parentOffset);

            //write file offset to new parent dir
            UpdateDir(fileOffset);
        }

        private static void DeleteFile(long fileOffset)
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
                if (nextSector == -1 || nextSector == 0)
                    break;
                offsets[indx++] = nextSector;
                Stream.Position = nextSector + (_sectorSize - sizeof(long));
            }

            //delete offset from parent directory and Items
            CleanParentDir(fileOffset);
            Stream.Position = fileOffset + 1 + sizeof(long);
            var fileName = MyToString(Br.ReadChars(NameLength));
            Items.Remove(fileName, fileOffset);
            RemoveItemListInfo(fileOffset);
            MainForm.DeleteNode(fileName, true);

            //replace sectors with 0 bytes
            foreach (var currOffset in offsets)
            {
                if (currOffset == 0)
                    break;
                Stream.Position = currOffset;
                for (var i = 0; i < _sectorSize; i += sizeof(long))
                    Bw.Write((long)0);//long because 8bytes at a time -> fewer cycles
            }
            Stream.Position = fileOffset;
        }

        private static void DeleteDirectory(long dirOffset)
        {
            //check if dir empty.
            if (IsDirEmpty(dirOffset))
            {
                //empty -> delete dir
                CleanParentDir(dirOffset);
                Stream.Position = dirOffset + 1 + sizeof(long);
                var dirName = MyToString(Br.ReadChars(NameLength));
                Items.Remove(dirName, dirOffset);
                RemoveItemListInfo(dirOffset);
                MainForm.DeleteNode(dirName, false);

                Stream.Position = dirOffset;
                for (var i = 0; i < NameLength; i += sizeof(long))
                    Bw.Write((long)0);//long because 8bytes at a time -> fewer cycles

                //clear end of sector value and parity bit
                Stream.Position = dirOffset + _sectorSize - 1 - sizeof(long);
                Bw.Write(false);
                Bw.Write((long)0);
                Stream.Position = dirOffset;

            }
            //not empty -> run DeleteFile with all offsets from directory
            else
            {
                var currPosition = dirOffset + 1 + NameLength;
                    
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

                CleanParentDir(dirOffset);

                Stream.Position = dirOffset + 1 + sizeof(long);
                var dirName = MyToString(Br.ReadChars(NameLength));

                Items.Remove(dirName, dirOffset);
                RemoveItemListInfo(dirOffset);

                currPosition = dirOffset + 1 + NameLength;
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

                    DeleteFile(fileOffset);
                }

                MainForm.DeleteNode(dirName, false);

                //delete dir
                Stream.Position = dirOffset;
                for (var i = 0; i < _sectorSize; i += sizeof(long))
                    Bw.Write((long)0);
                Stream.Position = dirOffset;
            }
        }

        private static bool IsDirEmpty(long offset)
        {
            Stream.Position = offset + 1 + 8 + NameLength;
            //stream is between name and parity bit
            while (Stream.Position < offset + (_sectorSize - 1 - 8 - NameLength - 1 - sizeof(long)))
            {
                var currBytes = Br.ReadBytes(sizeof(long));
                var value = BitConverter.ToInt64(currBytes , 0);

                if (value != 0)
                    return false;
            }

            return true;
        }

        private static void CleanParentDir(long objOffset)
        {
            var parentOffset = GetParentOffset(objOffset);
            //read dir and search for offset and delete it
            Stream.Position = parentOffset + 1 + 8 + NameLength;
            while (Stream.Position < parentOffset + (_sectorSize - 1 - 8 - NameLength - 1 - sizeof(long)))
            {
                var currBytes = Br.ReadBytes(sizeof(long));
                var value = BitConverter.ToInt64(currBytes , 0);
                if (value != objOffset)
                    continue;

                Stream.Position -= sizeof(long);
                Bw.Write((long)0);
                Stream.Position = parentOffset;
                break;
            }

            ParityCheck.UpdateParityBit(parentOffset, _sectorSize);
        }

        private static void CreateLongFile(string? fileName, string fileContents, int requiredSectors)
        {
            if (fileName == null) return;

            var writeOffsets = Bitmap.FindFreeSectors(Br, requiredSectors, (int)BitmapSectors, (int)_sectorSize);
            var strings = SplitString(fileContents, requiredSectors);
            Stream.Position = writeOffsets[0];
            Bw.Write(true);
            Bw.Write(CWDOffset);
            Bw.Write(MyToCharArray(fileName));
            Stream.Position = writeOffsets[0] + 1 + 8 + NameLength;

            for (var i = 0; i < requiredSectors; i++)
            {
                Bw.Write(MyToCharArray(strings[i]));

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
            UpdateDir(writeOffsets[0]);
            UpdateBitmap();

            MainForm.ChangeToRootWhenIfCwdBad();

            MainForm.ResetParentDir();
            ReadDirectory(GetParentOffset(writeOffsets[0]));

            Items.Add(fileName, writeOffsets[0]);
            UpdateItemListMetadata(MyToCharArray(fileName), writeOffsets[0]);
        }

        private static string[] SplitString(string str, int requiredSectors)
        {
            var strs = new string[requiredSectors];
            var firstPart = "";
            var indx = 0;
            int i;
            for (i = 0; i < _sectorSize - 1 - sizeof(long) - NameLength - 1 - sizeof(long); i++)
                firstPart += str[i];
            strs[indx++] = firstPart;

            var counter = 0;
            for (; i < str.Length; i++)
            {
                strs[indx] += str[i];
                counter++;
                if (counter == _sectorSize - 1 - sizeof(long))
                {
                    indx++;
                    counter = 0;
                }
            }
            return strs;
        }
        
        private static void UpdateBitmap() =>
            Bitmap.UpdateBitmap(new BinaryReader(Stream), (int)_sectorSize, (int)BitmapSectors, (int)_itemListTotalSectors, (int)_sectorCount, RootOffset);

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

        private static char[] MyToCharArray(string str)
        {
            var arr = new char[str.Length];
            for (var i = 0; i < str.Length; i++)
                arr[i] = str[i];
            return arr;
        }

        private static void UpdateItemListMetadata(char[] name, long offset)
        {
            //search for empty space
            Stream.Position = ItemInfoOffset;
            long currSpace = -1;
            while (currSpace != 0)
            {
                var bytes = Br.ReadBytes(ItemInfoSize);
                currSpace = BitConverter.ToInt64(bytes , 0);
            }

            //write info there
            Stream.Position -= ItemInfoSize;
            Bw.Write(name);

            Stream.Position += NameLength - name.Length;
            Bw.Write(offset);

            //without this below, Bw.Write(offset); doesn't write anything
            Stream.Position = ItemInfoOffset;
        }

        private static void RemoveItemListInfo(long offset)
        {
            Stream.Position = ItemInfoOffset;
            while (Stream.Position < Stream.Length)
            {
                var bytes = Br.ReadBytes(ItemInfoSize);
                var currSpace = BitConverter.ToInt64(bytes , NameLength);
                if (currSpace == offset)
                    break;
            }

            Stream.Position -= ItemInfoSize;
            for (var i = 0; i < NameLength; i++)
            {
                Bw.Write(false);
            }
            Bw.Write((long)0);
        }

        private static void InitializeItemList()
        {
            Stream.Position = ItemInfoOffset;

            while (Stream.Position < Stream.Length)
            {
                var bytes = Br.ReadBytes(ItemInfoSize);
                var currSpace = BitConverter.ToInt64(bytes , 0);
                if (currSpace != 0)
                {
                    Stream.Position -= ItemInfoSize;
                    var objName = MyToString(Br.ReadChars(NameLength));
                    var objOffset = BitConverter.ToInt64(Br.ReadBytes(sizeof(long)) , 0);
                    Items.Add(objName, objOffset);
                }
            }
        }
    }
}
