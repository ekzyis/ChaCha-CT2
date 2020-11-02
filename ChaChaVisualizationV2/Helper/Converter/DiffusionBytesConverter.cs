using System;
using System.Globalization;
using System.Windows.Data;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Helper.Converter
{
    internal class DiffusionBytesConverter : IValueConverter
    {
        /// <summary>
        /// Specifies how long the hex string should be. If hex string is too small, zero-left-padding is applied.
        /// </summary>
        private int Padding { get; set; }

        public DiffusionBytesConverter(int padding) : base()
        {
            Padding = padding;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string hex = Formatter.HexString((byte[])value);
            return hex.PadLeft(Padding).Replace(" ", "0");
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