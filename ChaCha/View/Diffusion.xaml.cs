using Cryptool.Plugins.ChaCha.Helper;
using Cryptool.Plugins.ChaCha.Helper.Converter;
using Cryptool.Plugins.ChaCha.Helper.Validation;
using Cryptool.Plugins.ChaCha.ViewModel;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace Cryptool.Plugins.ChaCha.View
{
    /// <summary>
    /// Interaction logic for Diffusion.xaml
    /// </summary>
    public partial class Diffusion : UserControl
    {
        public Diffusion()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DiffusionViewModel ViewModel = (DiffusionViewModel)e.NewValue;
            if (ViewModel != null)
            {
                Version v = ViewModel.Settings.Version;

                InputDiffusionKey.Text = Formatter.HexString(ViewModel.ChaCha.InputKey);
                int keyLength = ViewModel.ChaCha.InputKey.Length;
                ValidationRule keyRule = new DiffusionInputValidationRule(keyLength);
                IValueConverter keyConverter = new DiffusionBytesConverter(keyLength);
                InitDiffusionInputField(InputDiffusionKey, keyRule, keyConverter, "DiffusionInputKey", ViewModel.DiffusionInputKeyKeyHandler(keyRule));
                InitDiffusionInputField(InputDiffusionXORKey, keyRule, keyConverter, "DiffusionXORKey", ViewModel.DiffusionXORKeyKeyHandler(keyRule));

                InputDiffusionIV.Text = Formatter.HexString(ViewModel.ChaCha.InputIV);
                int ivLength = ViewModel.ChaCha.InputIV.Length;
                ValidationRule ivRule = new DiffusionInputValidationRule(ivLength);
                IValueConverter ivConverter = new DiffusionBytesConverter(ivLength);
                InitDiffusionInputField(InputDiffusionIV, ivRule, ivConverter, "DiffusionIV");

                BigInteger initialCounter = ViewModel.ChaCha.InitialCounter;
                InputDiffusionInitialCounter.Text = Formatter.HexString(v == Version.DJB ? (ulong)initialCounter : (uint)initialCounter);
                int counterLength = (int)v.CounterBits / 8;
                ValidationRule counterRule = new DiffusionInputValidationRule(counterLength);
                IValueConverter counterConverter = new DiffusionCounterConverter(counterLength);
                InitDiffusionInputField(InputDiffusionInitialCounter, counterRule, counterConverter, "DiffusionInitialCounter");
            }
        }

        private void InitDiffusionInputField(TextBox inputField, ValidationRule validationRule, IValueConverter converter, string bindingPath)
        {
            Binding binding = new Binding(bindingPath) { Mode = BindingMode.TwoWay, Converter = converter };
            binding.ValidationRules.Add(validationRule);
            inputField.SetBinding(TextBox.TextProperty, binding);
            EditingCommands.ToggleInsert.Execute(null, inputField);
        }

        private void InitDiffusionInputField(TextBox inputField, ValidationRule validationRule, IValueConverter converter, string bindingPath, TextChangedEventHandler handleUserInput)
        {
            InitDiffusionInputField(inputField, validationRule, converter, bindingPath);
            inputField.TextChanged += handleUserInput;
        }
    }
}