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
        /// Return the byte array in big-endian of the UInt32.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static byte[] GetBytesBE(uint x)
        {
            byte[] bytes = BitConverter.GetBytes(x);
            if (BitConverter.IsLittleEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// Return the byte array in little-endian of the UInt32.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static byte[] GetBytesLE(uint x)
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

        /// <summary>
        /// Assume input is in big-endian. Reverse byte order.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static uint ToUInt32LE(uint x)
        {
            byte[] b = ByteUtil.GetBytesLE(x);
            // If system architecture is little-endian, BitConverter.ToUInt32 will interpret byte array in little-endian.
            // We reverse the array in this case before passing it to BitConverter.
            if (BitConverter.IsLittleEndian)
            {
                b = b.Reverse().ToArray();
            }
            return BitConverter.ToUInt32(b, 0);
        }

        /// <summary>
        /// Assume input is in big-endian. Return a UInt32 Array by combining each consecutive four bytes into a UInt32.
        /// </summary>
        /// <param name="b"></param>
        public static uint[] ToUInt32Array(byte[] b)
        {
            if (b.Length % 4 != 0)
            {
                throw new ArgumentOutOfRangeException("b", b.Length, "Input bytes length must be multiple of four.");
            }

            uint[] u = new uint[b.Length / 4];
            for (int i = 0; i < u.Length; ++i)
            {
                u[i] = (uint)(b[i] << 24 | b[i + 1] << 16 | b[i + 2] << 8 | b[i + 3]);
            }
            return u;
        }

        /// <summary>
        /// Return a byte array of the UInt32 array by creating a byte array in big-endian notation for each UInt32.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(uint[] u)
        {
            byte[] b = new byte[u.Length * 4];
            for (int i = 0; i < u.Length; ++i)
            {
                byte[] uBytes = ByteUtil.GetBytesBE(u[i]);
                b[i * 4] = uBytes[0];
                b[i * 4 + 1] = uBytes[1];
                b[i * 4 + 2] = uBytes[2];
                b[i * 4 + 3] = uBytes[3];
            }
            return b;
        }

        /// <summary>
        /// Circular left shift.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static uint RotateLeft(uint x, int shift)
        {
            return (x << shift) | (x >> -shift);
        }
    }
}