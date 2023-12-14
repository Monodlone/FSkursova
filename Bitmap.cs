using System.Collections;

namespace Kursova
{
    internal static class Bitmap
    {
        //TODO: don't read whole BitArray, read only what I need
        //Its wrong to load the whole BitArray in memory.
        //Only load the byte I need to edit the bits in!!
        //but this works for now...

        internal static void UpdateBitmap(BinaryReader br, int sectorSize, int bitmapSectors, int sectorCount)
        {
            BitArray bitArr = new(sectorCount);
            for (var i = 0; i < bitmapSectors; i++)
                bitArr[i] = true;

            br.BaseStream.Position = bitmapSectors * sectorSize + 2;//raises weird error sometimes
            for (var i = bitmapSectors; i < sectorCount; i++)
            {
                var tmp = br.ReadChar();//first char of file/dir name
                if (tmp != 0)
                    bitArr[i] = true;//taken sector
                else
                    bitArr[i] = false;//free sector
                br.BaseStream.Position += sectorSize - 1;
            }

            WriteBitmap(bitArr, new BinaryWriter(FileSystem.GetFileStream()));
        }

        internal static long FindFreeSector(BinaryReader br, int bitmapSectors, int sectorSize)
        {
            br.BaseStream.Position = 0;
            var writeOffset = -1;
            for (var i = 0; i < bitmapSectors * sectorSize; i++)
            {
                var currByte = br.ReadByte();
                if (currByte == 255)
                    continue;
                var bits = new BitArray(new[] { currByte });
                
                for (var j = 0; j < 8; j++)
                {
                    if(bits[j])
                        continue;
                    writeOffset = j * sectorSize;
                    break;
                }

                if (writeOffset != -1)
                    break;
                //scan byte for free bits
                //number of free bit is number of free sector
            }
            if (writeOffset == -1)
                return -1;

            return writeOffset + 1;
        }

        internal static long[] FindFreeSectors(BinaryReader br, int requiredSectors, int bitmapSectors, int sectorSize)
        {
            br.BaseStream.Position = 0;
            var offsets = new long[requiredSectors];
            var indx = 0;
            var byteNum = 0;
            for (var i = 0; i < bitmapSectors * sectorSize; i++)
            {
                var currByte = br.ReadByte();
                byteNum++;
                if (currByte == 255)
                    continue;
                var bits = new BitArray(new[] { currByte });

                for (var j = 0; j < 8; j++)
                {
                    if(bits[j])
                        continue;
                    var sectorIndx = j + ((byteNum - 1) * 8);//eyesore
                    offsets[indx++] = sectorSize * sectorIndx + 1;
                    requiredSectors--;
                    if (requiredSectors == 0)
                        break;
                }
                if (requiredSectors == 0)
                    break;
            }
            return offsets;
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
