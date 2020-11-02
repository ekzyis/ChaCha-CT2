using System;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Helper
{
    /// <summary>
    /// Class with helper functions related to bytes.
    /// </summary>
    internal static class ByteUtil
    {
        /// <summary>
        /// Calculate amount of flipped bits between two byte arrays.
        /// </summary>
        public static int FlippedBits(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) throw new ArgumentException("Arrays must be of equal size");
            int flippedBits = 0;
            for (int i = 0; i < a.Length; ++i)
            {
                flippedBits += CountBits((ulong)(a[i] ^ b[i]));
            }
            return flippedBits;
        }

        /// <summary>
        /// Calculate amount of flipped bits between two BigInteger.
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
    }
}