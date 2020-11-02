using System;
using System.Globalization;
using System.Windows.Data;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Helper.Converter
{
    internal class DoubleToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double d = (double)value;
            return string.Format("{0:0.00}", d);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}