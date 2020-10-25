using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cryptool.Plugins.ChaCha
{
    internal sealed class KeyValidator : ValidationAttribute
    {
        public KeyValidator(string errorMessage) : base(errorMessage)
        {
        }

        public override bool IsValid(object value)
        {
            // A key is valid if it is lower than 128-bit or exactly 256-bit.
            // This means the following equation must be true where K is the amount of key bits:
            //   0 < K <= 128 || K == 256
            // The reason for this is that a 128-bit key can be expanded into a 256-bit
            // but a key larger than 128-bit cannot.
            byte[] inputKey = value as byte[];
            string hexKey = string.Join("", inputKey.Select(b => b.ToString("X2")));

            // Because one byte consists of 2 hexadecimal letters, we multiply by 2.
            var check128Bit = new StringLengthAttribute(16 * 2) { MinimumLength = 1 };
            var check256Bit = new StringLengthAttribute(32 * 2) { MinimumLength = 32 * 2 };
            return check128Bit.IsValid(hexKey) || check256Bit.IsValid(hexKey);
        }
    }

    internal sealed class IVValidator : ValidationAttribute
    {
        public IVValidator(string errorMessage) : base(errorMessage)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            ChaCha chacha = context.ObjectInstance as ChaCha;
            Version currentVersion = ((ChaChaSettings)chacha.Settings).Version;
            byte[] inputIV = value as byte[];
            string hexIV = string.Join("", inputIV.Select(b => b.ToString("X2")));
            int maxBits = (int)currentVersion.IVBits;
            int maxBytes = maxBits * 8;

            var required = new StringLengthAttribute(maxBytes * 2) { MinimumLength = 1 };
            return required.IsValid(hexIV) ?
                ValidationResult.Success :
                new ValidationResult(ErrorMessage);
        }
    }
}