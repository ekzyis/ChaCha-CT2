using System.Globalization;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Helper.Validation
{
    internal class DiffusionInputValidationRule : ValidationRule
    {
        private int MaxKeyBytesLength { get; set; }
        private int MaxHexKeyStringLength { get; set; }

        public DiffusionInputValidationRule(int maxKeyBytesLength) : base()
        {
            MaxKeyBytesLength = maxKeyBytesLength;
            MaxHexKeyStringLength = maxKeyBytesLength / 2;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string inputText = (string)value;
            if (!System.Text.RegularExpressions.Regex.IsMatch(inputText, @"\A\b[0-9a-fA-F]+\b\Z"))
            {
                return new ValidationResult(false, $"Input is not a valid hex string.");
            }

            if (inputText.Length > MaxHexKeyStringLength)
            {
                return new ValidationResult(false, $"Hex input exceeds maximum size of {MaxKeyBytesLength} bytes / {MaxKeyBytesLength / 8}-bit.");
            }
            return ValidationResult.ValidResult;
        }
    }
}