using System.Globalization;
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
            return HexString(ChaCha.Util.ByteUtil.GetBytesBE(u));
        }

        /// <summary>
        /// Return hex representation of UInt32.
        /// </summary>
        public static string HexString(uint u)
        {
            return HexString(ChaCha.Util.ByteUtil.GetBytesBE(u));
        }

        /// <summary>
        /// Return bytes of hex string.
        /// </summary>
        public static byte[] Bytes(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = System.Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        /// <summary>
        /// Return BigInteger of hex string.
        /// </summary>
        public static System.Numerics.BigInteger BigInteger(string hex)
        {
            string unsignedHex = $"0{hex}";
            return System.Numerics.BigInteger.Parse(unsignedHex, NumberStyles.HexNumber);
        }
    }
}