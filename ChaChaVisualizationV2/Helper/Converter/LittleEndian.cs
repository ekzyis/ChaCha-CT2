using System;
using System.Globalization;
using System.Windows.Data;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Helper.Converter
{
    /// <summary>
    /// Reverse byte order of each consecutive 4 bytes.
    /// </summary>
    internal class LittleEndian : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            byte[] b = (byte[])value;
            if (b.Length % 4 != 0) throw new ArgumentException("Byte length must be divisible by four.");
            byte[] le = new byte[b.Length];
            for (int i = 0; i < b.Length; i += 4)
            {
                le[i] = b[i + 3];
                le[i + 1] = b[i + 2];
                le[i + 2] = b[i + 1];
                le[i + 3] = b[i];
            }
            return le;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}