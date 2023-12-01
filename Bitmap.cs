using System.Collections;

namespace Kursova
{
    internal class Bitmap
    {
        //TODO: don't read whole BitArray, read only what I need
        //Its wrong to load the whole BitArray in memory.
        //Only load the byte i need to edit the bits in!!
        //but this works for now...

        public static void UpdateBitmap(BinaryReader br, int sectorSize, int bitmapSectors, int sectorCount)
        {
            BitArray bitArr = new(sectorCount);
            for (var i = 0; i < bitmapSectors; i++)
                bitArr[i] = true;

            br.BaseStream.Position = bitmapSectors * sectorSize + 1;
            for (var i = bitmapSectors; i < sectorCount; i++)
            {
                var tmp = br.ReadChars(2);//TODO fix out of bounds error
                if (tmp[1] != 0)    //first char of file/dir name
                    bitArr[i] = true;//taken sector
                else
                    bitArr[i] = false;//free sector
                br.BaseStream.Position += sectorSize - 1;
            }

            WriteBitmap(bitArr, new BinaryWriter(Program.GetFileStream()));
        }

        public static long FindFreeSector(BinaryReader br,int requiredSectors, int bitmapSectors, int sectorCount, int sectorSize)
        {
            long offset = -1;
            br.BaseStream.Position = bitmapSectors * sectorSize + 1;
            for (var i = bitmapSectors; i < sectorCount; i++)
            {
                var tmp = br.ReadChars(2);
                if(tmp[1] != 0)
                    continue;
                if (requiredSectors == 1)
                    return br.BaseStream.Position;
                if (requiredSectors > 1)
                {
                    offset = br.BaseStream.Position;
                    var noNameYet = FindSectorChain(br, requiredSectors, bitmapSectors, sectorCount, sectorSize);
                    return noNameYet ? offset : -1;
                }
            }

            return offset;//-1 couldn't find a suitable offset for the file
        }

        private static bool FindSectorChain(BinaryReader br, int requiredSectors,int bitmapSectors, int sectorCount, int sectorSize)
        {
            //try to find free sectors next to one another
            br.BaseStream.Position = bitmapSectors * sectorSize + 1;
            for (var i = bitmapSectors; i < sectorCount; i++)
            {
                
            }
            //if there isn't any -> need to have a custom linked list >:(
            return false;
        }

        private static void WriteBitmap(BitArray bitArr, BinaryWriter bw)
        {
            bw.Seek(0, SeekOrigin.Begin);
            for (var i = 0; i < bitArr.Length;)
            {
                byte tmp = 0;
                for (var j = i; j < i + 8; j++)//get a byte from 8 bool values
                    if(bitArr[j])
                        tmp |= (byte)(1 << j);

                bw.Write(tmp);
                i += 8;
            }
        }
    }
}
