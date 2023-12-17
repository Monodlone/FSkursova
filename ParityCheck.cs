using System.Text;

namespace Kursova
{
    //TODO even parity check
    //end of every data sector have a parity bit
    //when reading first CheckFileIntegrity 
    //when false -> don't read file -> mark as bad file with BadObjColor
    internal static class ParityCheck
    {
        internal static void WriteParityBit(long offset, long sectorSize)
        {
            var stream = FileSystem.GetFileStream();
            var bw = new BinaryWriter(stream, Encoding.UTF8);
            var br = new BinaryReader(stream, Encoding.UTF8);

            stream.Position = offset;
            var data = br.ReadBytes((int)sectorSize);
            var parityBit = CalculateEvenParityBit(data);

            stream.Position = offset + (sectorSize - 1 - sizeof(long));
            var b = (byte)parityBit;
            bw.Write(b);
        }

        internal static bool CheckFileIntegrity(long fileOffset, long sectorSize)
        {
            var stream = FileSystem.GetFileStream();
            stream.Position = fileOffset;
            var br = new BinaryReader(stream, Encoding.UTF8);

            while (fileOffset != -1)
            {
                var data = br.ReadBytes((int)sectorSize);
                if (!CheckEvenParity(data))
                    return false;
                stream.Position -= sizeof(long);
                var next = br.ReadBytes(sizeof(long));
                fileOffset = BitConverter.ToInt64(next , 0);
                if (fileOffset == -1)
                    break;
                stream.Position = fileOffset;
            }
            return true;
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
