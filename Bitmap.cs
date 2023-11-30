using System.Collections;

namespace Kursova
{
    internal class Bitmap
    {
        //TODO is this in memory or in the file?????
        private readonly BitArray _bitArr;

        public Bitmap(int length)
        {
            _bitArr = new BitArray(length);
        }

        public bool UpdateBitmap(Bitmap bitmap, long bitmapSectors, BinaryReader br, long sectorCount)
        {
            if (_bitArr.Count == 0)
                return false;

            for (int i = 0; i < bitmapSectors; i++)//reserved == taken
                _bitArr[i] = true;

            for (long i = bitmapSectors; i < sectorCount; i++)//rest of fs
            {
                char[] tmp = br.ReadChars(31);

            }

            return true;
        }
    }
}
