using System;
using System.Linq;

namespace Cryptool.Plugins.ChaCha.Util
{
    internal static class ByteUtil
    {
        /// <summary>
        /// Return the byte array in big-endian order, independent of system architecture.
        ///
        /// (One should only call this function once on a byte array else the name does not make sense :D)
        /// </summary>
        /// <param name="bytes">The byte array we want to have in big-endian.</param>
        /// <returns></returns>
        public static void ConvertToBigEndian(ref byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }
        }

        /// <summary>
        /// Return the byte array in big-endian of the UInt64.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static byte[] GetBytesBE(ulong x)
        {
            byte[] bytes = BitConverter.GetBytes(x);
            if (BitConverter.IsLittleEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// Return the byte array in little-endian of the UInt64.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static byte[] GetBytesLE(ulong x)
        {
            byte[] bytes = BitConverter.GetBytes(x);
            if (!BitConverter.IsLittleEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// Assume input is in big-endian. Return a UInt32 with reversed byte order, starting at the offset.
        /// </summary>
        /// <param name="x">Byte array of which we want to return the UInt32 in reversed byte order.</param>
        /// <param name="offset">Array offset for first byte.</param>
        /// <returns>UInt32 in reversed byte order</returns>
        public static uint ToUInt32LE(byte[] x, int offset)
        {
            byte b1 = x[offset];
            byte b2 = x[offset + 1];
            byte b3 = x[offset + 2];
            byte b4 = x[offset + 3];

            return (uint)(b4 << 24 | b3 << 16 | b2 << 8 | b1);
        }
    }
}