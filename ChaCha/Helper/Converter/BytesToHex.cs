using System;
using System.Globalization;
using System.Windows.Data;

namespace Cryptool.Plugins.ChaCha.Helper.Converter
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
            string inputText = (string)value;
            // Left-pad hex string with zero such that is has an even amount of characters.
            if (inputText.Length % 2 == 1)
            {
                inputText = $"0{inputText}";
            }
            return Formatter.Bytes(inputText);
        }
    }
}