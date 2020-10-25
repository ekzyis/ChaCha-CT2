using System;
using System.Linq;

namespace Cryptool.Plugins.ChaCha.Util
{
    internal static class ByteUtil
    {
        /// <summary>
        /// Return the byte array in big-endian order, independent of system architecture.
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
    }
}