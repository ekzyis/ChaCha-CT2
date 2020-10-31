using System;
using System.Globalization;
using System.Windows.Data;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Helper.Converter
{
    internal class BytesToHex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] bytes = (byte[])value;
            return Formatter.HexString(bytes);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}