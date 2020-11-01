using System;
using System.Globalization;
using System.Windows.Data;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Helper.Converter
{
    internal class BytesToHex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            return Formatter.HexString((byte[])value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Formatter.Bytes((string)value);
        }
    }
}