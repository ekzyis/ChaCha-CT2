using System;
using System.Globalization;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Helper.Validation
{
    internal class UserActionInputValidationRule : ValidationRule
    {
        public UserActionInputValidationRule(int totalActions) : base()
        {
            MaxActionIndex = totalActions - 1;
        }

        private int MaxActionIndex { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int input = 0;

            try
            {
                input = int.Parse((string)value);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Illegal characters or {e.Message}");
            }

            if ((input < 0) || (input > MaxActionIndex))
            {
                return new ValidationResult(false,
                    $"Please enter a value between {0} and {MaxActionIndex}.");
            }
            return ValidationResult.ValidResult;
        }
    }
}