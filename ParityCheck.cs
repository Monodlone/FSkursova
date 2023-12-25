using System.Text;

namespace Kursova
{
    //TODO even parity check
    //end of every data sector have a parity bit
    //when reading first CheckSectorIntegrity 
    //when false -> don't read file -> mark as bad file with BadObjColor
    internal static class ParityCheck
    {
        internal static void WriteParityBit(long offset, long sectorSize)
        {
            var stream = FileSystem.GetStream();
            var bw = new BinaryWriter(stream);
            var br = new BinaryReader(stream);

            stream.Position = offset;
            var data = br.ReadBytes((int)sectorSize);
            var parityBit = CalculateEvenParityBit(data);

            stream.Position -= sizeof(long) + 1;
            bw.Write((byte)parityBit);
        }

        internal static bool CheckSectorIntegrity(long offset, long sectorSize)
        {
            var stream = FileSystem.GetStream();
            var br = new BinaryReader(stream);
            stream.Position = offset;

            while (offset != -1)
            {
                var data = br.ReadBytes((int)sectorSize);
                if (!CheckEvenParity(data))
                    return false;
                stream.Position -= sizeof(long);
                var next = br.ReadBytes(sizeof(long));
                offset = BitConverter.ToInt64(next , 0);
                if (offset == -1)
                    break;
                stream.Position = offset;
            }
            return true;
        }

        internal static void UpdateParityBitOfCWD(long offset, long sectorSize)
        {
            var stream = FileSystem.GetStream();
            stream.Position = offset + (sectorSize - 1 - sizeof(long));
            var bw = new BinaryWriter(stream);
            bw.Write((byte)0);

            WriteParityBit(offset, sectorSize);
        }

        //0 if even num of true bits, 1 otherwise
        private static bool CheckEvenParity(byte[] data) => CalculateEvenParityBit(data) == 0;

        private static int CalculateEvenParityBit(byte[] data)
        {
            var countOnes = 0;
            foreach (var b in data)
                countOnes += CountTrueBits(b);

            return countOnes % 2 == 0 ? 0 : 1;
        }

        private static int CountTrueBits(byte value)
        {
            var count = 0;
            while (value > 0)
            {
                count += value & 1;
                value >>= 1;
            }
            return count;
        }
    }
}
