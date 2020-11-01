using System;
using System.Globalization;
using System.Windows.Data;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Helper.Converter
{
    internal class ToHex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Formatter.HexString((byte[])value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}