using Cryptool.Plugins.ChaCha.Util;
using System.Text;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Helper
{
    internal static class Formatter
    {
        /// <summary>
        /// Return hex representation of bytes.
        /// </summary>
        public static string HexString(byte[] bytes, int offset, int length)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = offset; i < offset + length; ++i)
            {
                sb.Append(bytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Return hex representation of bytes.
        /// </summary>
        public static string HexString(byte[] bytes)
        {
            return HexString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Return hex representation of UInt64.
        /// </summary>
        public static string HexString(ulong u)
        {
            return HexString(ByteUtil.GetBytesBE(u));
        }

        /// <summary>
        /// Return hex representation of UInt32.
        /// </summary>
        public static string HexString(uint u)
        {
            return HexString(ByteUtil.GetBytesBE(u));
        }
    }
}