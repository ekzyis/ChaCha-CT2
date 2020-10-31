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
    }
}