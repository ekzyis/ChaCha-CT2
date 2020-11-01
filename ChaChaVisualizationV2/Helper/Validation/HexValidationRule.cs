using System;
using System.Globalization;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Helper.Validation
{
    internal class HexValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string inputText = (string)value;

            try
            {
                ulong.Parse(inputText, NumberStyles.HexNumber);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"{e.Message}");
            }
            return ValidationResult.ValidResult;
        }
    }
}