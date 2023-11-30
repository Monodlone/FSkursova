using System.Collections;

namespace Kursova
{
    internal class Bitmap
    {
        public static void UpdateBitmap(BinaryReader br, int bitmapSectors, int sectorCount)
        {
            BitArray bitArr = new(sectorCount);
            for (var i = 0; i < bitmapSectors; i++)
                bitArr[i] = true;

            for (var i = bitmapSectors; i < sectorCount; i++)
            {
                char[] tmp = br.ReadChars(3);
                if (tmp[2] != 0)        //first char of file/dir name
                    bitArr[(int)i] = true;//taken sector
                else
                    bitArr[(int)i] = false;//free sector
            }

            BinaryWriter bw = new BinaryWriter(Program.GetFileStream());
            WriteBitmap(bitArr, bw);
        }

        private static void WriteBitmap(BitArray bitArr, BinaryWriter bw)
        {
            bw.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < bitArr.Length;)
            {
                byte tmp = 0;
                for (int j = i; j < i + 8; j++)
                    if(bitArr[j])
                        tmp |= (byte)(1 << j);

                bw.Write(tmp);
                i += 8;
            }
        }
    }
}
