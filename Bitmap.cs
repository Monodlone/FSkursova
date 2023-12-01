using System.Collections;

namespace Kursova
{
    internal class Bitmap
    {
        //TODO: don't read whole BitArray, read only what I need
        //Its wrong to load the whole BitArray in memory.
        //Only load the byte i need to edit the bits in!!
        //but this works for now...
        public static void UpdateBitmap(BinaryReader br, int bitmapSectors, int sectorCount)
        {
            BitArray bitArr = new(sectorCount);
            for (var i = 0; i < bitmapSectors; i++)
                bitArr[i] = true;

            for (var i = bitmapSectors; i < sectorCount; i++)
            {
                var tmp = br.ReadChars(3);
                if (tmp[2] != 0)    //first char of file/dir name
                    bitArr[i] = true;//taken sector
                else
                    bitArr[i] = false;//free sector
            }

            WriteBitmap(bitArr, new BinaryWriter(Program.GetFileStream()));
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
