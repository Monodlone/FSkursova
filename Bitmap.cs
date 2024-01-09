using System.Collections;

namespace Kursova
{
    internal static class Bitmap
    {
        internal static void UpdateBitmap(BinaryReader br, int sectorSize, int bitmapSectors, int ItemListSectors, int sectorCount, long rootOffset)
        {
            BitArray bitArr = new(sectorCount);
            for (var i = 0; i < bitmapSectors; i++)
                bitArr[i] = true;

            br.BaseStream.Position = rootOffset + 1;//+1 cuz dirs first byte is empty
            for (var i = bitmapSectors; i < sectorCount - ItemListSectors; i++)
            {
                var tmp = br.ReadChar();
                if (tmp != 0)
                    bitArr[i] = true;//taken sector
                else
                    bitArr[i] = false;//free sector
                br.BaseStream.Position += sectorSize - 1;
            }
            WriteBitmap(bitArr, new BinaryWriter(FileSystem.GetStream()));
        }

        internal static long FindFreeSector(BinaryReader br, int bitmapSectors, int sectorSize)
        {
            br.BaseStream.Position = sizeof(long) * 2 + 1;//skip metadata
            var writeOffset = -1;
            for (var i = 0; i < bitmapSectors * sectorSize; i++)
            {
                var currByte = br.ReadByte();
                if (currByte == 255)
                    continue;
                var bits = new BitArray(new[] { currByte });
                var freeBits = 0;

                foreach (bool bit in bits)
                {
                    if (!bit)
                    {
                        // The first zero bit is found within the current byte
                        writeOffset = i * 8 + freeBits;
                        break;
                    }
    
                    freeBits++;
                }
                //writeOffset = (i + 1) * 8 - freeBits;

                if (writeOffset != -1)
                    break;
            }
            if (writeOffset == -1)
                return writeOffset;

            return writeOffset * sectorSize + 1;
        }

        internal static long[] FindFreeSectors(BinaryReader br, int requiredSectors, int bitmapSectors, int sectorSize)
        {
            br.BaseStream.Position = sizeof(long) * 2 + 1;//skip metadata
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
                    var sectorIndx = j + (byteNum - 1) * 8;//eyesore
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
            var stream  = FileSystem.GetStream();
            stream.Position = sizeof(long) * 2 + 1;//skip metadata
            for (var i = 0; i < bitArr.Length; i += sizeof(long))
            {
                byte curr = 0;

                for (var j = i; j < i + 8; j++)//get a byte from 8 bool values
                    if(bitArr[j])
                        curr |= (byte)(1 << (j - i));//if bitArr is true set the bit in byte to 1

                bw.Write(curr);
            } 
        }
    }
}
