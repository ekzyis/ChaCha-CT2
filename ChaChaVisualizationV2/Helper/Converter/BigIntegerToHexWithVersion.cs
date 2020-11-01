using Cryptool.Plugins.ChaCha;
using System;
using System.Globalization;
using System.Numerics;
using System.Windows.Data;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Helper.Converter
{
    internal class BigIntegerToHexWithVersion : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var value = values[0];
            if (value is BigInteger bigInteger)
            {
                var version = ((ChaChaSettings)values[1]).Version;
                if (version.CounterBits == 64)
                {
                    return Formatter.HexString((ulong)bigInteger);
                }
                else
                {
                    return Formatter.HexString((uint)bigInteger);
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            string hex = (string)value;
            System.Diagnostics.Debug.Assert(hex.Length == 16 || hex.Length == 24, $"value must be of length 16 or 24. Actual: {hex.Length}");
            // 64-bit are 8 bytes thus if the hex string has 16 characters, it has 8 bytes and is the counter of version DJB.
            ChaCha.Version v = hex.Length == 16 ? ChaCha.Version.DJB : ChaCha.Version.IETF;
            return new object[] { Formatter.BigInteger((string)value), v };
        }
    }
}