namespace Cryptool.Plugins.ChaCha.Helper
{
    /// <summary>
    /// Class with functions related to bit flips.
    /// </summary>
    internal static class BitFlips
    {
        /// <summary>
        /// Calculate amount of flipped bits between two byte arrays.
        /// </summary>
        public static int FlippedBits(byte[] a, byte[] b)
        {
            // Left pad byte arrays wiht zeroes such that they have equal length
            LeftPad(a, b.Length);
            LeftPad(b, a.Length);
            int flippedBits = 0;
            for (int i = 0; i < a.Length; ++i)
            {
                flippedBits += CountBits((ulong)(a[i] ^ b[i]));
            }
            return flippedBits;
        }

        /// <summary>
        /// Left pad the input bytes array with zeroes. If the input array is already bigger, nothing happens.
        /// </summary>
        /// <param name="bytes">The byte array to left-pad with zeroes.</param>
        /// <param name="size">How long the byte array should be at least in the end.</param>
        /// <returns></returns>
        public static byte[] LeftPad(byte[] b, int size)
        {
            if (b.Length >= size)
            {
                return b;
            }
            byte[] padded = new byte[size];
            int leftPadAmount = size - b.Length;
            for (int i = 0; i < leftPadAmount; ++i)
            {
                padded[i] = 0x00;
            }
            b.CopyTo(padded, leftPadAmount);
            return padded;
        }

        /// <summary>
        /// Calculate amount of flipped bits between two UInt64.
        /// </summary>
        public static int FlippedBits(ulong a, ulong b)
        {
            return CountBits(a ^ b);
        }

        /// <summary>
        /// Calculate how many bits are set.
        /// </summary>
        public static int CountBits(ulong x)
        {
            int count = 0;
            while (x > 0)
            {
                count += (int)(x & 1);
                x >>= 1;
            }
            return count;
        }

        /// <summary>
        /// Calculate the amount of bit flips between the two given 512-bit states.
        /// </summary>
        public static int FlippedBits(uint?[] diffusionState, uint?[] primaryState)
        {
            int count = 0;
            for (int i = 0; i < 16; ++i)
            {
                uint? dv = diffusionState[i];
                uint? pv = primaryState[i];
                if (dv != null && pv != null) count += FlippedBits((ulong)dv, (ulong)pv);
            }
            return count;
        }
    }
}