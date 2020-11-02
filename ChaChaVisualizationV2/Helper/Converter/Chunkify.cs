using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Helper.Converter
{
    /// <summary>
    /// Insert a space after every 8 characters.
    /// </summary>
    internal class Chunkify : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Regex.Replace((string)value, @".{8}", "$0 ");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}