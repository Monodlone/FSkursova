using Kursova.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kursova
{

    internal static class FileSystem
    {
        //TODO LIST:
        //TODO shouldn't need -1 for calculating offset for new files: line 47 here
        //TODO not even using the bitmap properly: line 32 bitmap.cs
        //TODO don't have a way of looking for multiple sectors for big files: line 51 bitmap.cs
        //TODO i think its time for a separate primary file

        private static readonly FileStream Stream = File.Create("C:\\Users\\PiwKi\\Desktop\\fs_file");
        private static readonly BinaryWriter Bw = new BinaryWriter(Stream, Encoding.UTF8, true);
        private static readonly BinaryReader Br = new BinaryReader(Stream, Encoding.UTF8, true);
        private static long BitmapSectors { get; set; } = 1;
        private static readonly long RootOffset = BitmapSectors + 1;
        private static readonly long DataRegionOffset = RootOffset + 1;
        private static int RootFileAdressOffset = 0;

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

            Stream.Position = RootOffset * SectorSize - 1;
            Bw.Write(false);
            Bw.Write("Root");
            UpdateBitmap();

            CreateFile(CreateFileForm.GetFileInfo());
        }
        private static void CreateFile(string[] fileInfo)
        {
            if(fileInfo[0] == null)
                return;
            //1 find free sector using bitmap
            //2 change stream position to the free sector
            //3 write file name and contents using Bw
            //4 save file offset to Root
            //5 update the bitmap
            long fileSize = fileInfo[1].Length;
            var requiredSectors = 1;
            while (fileSize > SectorSize)
            {
                requiredSectors++;
                fileSize -= SectorSize;
            }
            var writeOffset = Bitmap.FindFreeSector(Br, requiredSectors, (int)BitmapSectors, (int)SectorCount, (int)SectorSize);

            Stream.Position = writeOffset;
            var firstFileOffset = requiredSectors;
            Bw.Write(true);
            Bw.Write(fileInfo[0] + ".txt");

            Stream.Position = writeOffset + fileInfo[0].Length + 6;
            Bw.Write(fileInfo[1] != null? fileInfo[1]: "");

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
            Stream.Position = RootOffset + 6 + RootFileAdressOffset;
            Stream.WriteByte((byte)fileOffset);
            RootFileAdressOffset++;
        }

        public static FileStream GetFileStream()
        {
            return Stream;
        }
    }
}
