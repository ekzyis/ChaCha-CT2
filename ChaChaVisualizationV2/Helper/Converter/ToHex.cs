using Cryptool.Plugins.ChaCha;
using System;
using System.Globalization;
using System.Numerics;
using System.Windows.Data;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Helper.Converter
{
    internal class ToHex : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var value = values[0];
            if (value is byte[] bytes)
            {
                return Formatter.HexString(bytes);
            }
            else if (value is BigInteger bigInteger)
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
            else
            {
                throw new ArgumentException("value was neither byte[] nor BigInteger");
            }
        }

        public object[] ConvertBack(object values, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}