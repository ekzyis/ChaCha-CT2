using Cryptool.Plugins.ChaChaVisualizationV2.Helper;
using Cryptool.Plugins.ChaChaVisualizationV2.Helper.Converter;
using Cryptool.Plugins.ChaChaVisualizationV2.Helper.Validation;
using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace Cryptool.Plugins.ChaChaVisualizationV2.View
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
                ChaCha.Version v = ViewModel.Settings.Version;

                DiffusionInputKey.Text = Formatter.HexString(ViewModel.ChaChaVisualization.InputKey);
                int keyLength = ViewModel.ChaChaVisualization.InputKey.Length;
                ValidationRule keyRule = new DiffusionInputValidationRule(keyLength);
                IValueConverter keyConverter = new DiffusionBytesConverter(keyLength);
                InitDiffusionInputField(DiffusionInputKey, keyRule, keyConverter, "DiffusionInputKey");

                DiffusionInputIV.Text = Formatter.HexString(ViewModel.ChaChaVisualization.InputIV);
                int ivLength = ViewModel.ChaChaVisualization.InputIV.Length;
                ValidationRule ivRule = new DiffusionInputValidationRule(ivLength);
                IValueConverter ivConverter = new DiffusionBytesConverter(ivLength);
                InitDiffusionInputField(DiffusionInputIV, ivRule, ivConverter, "DiffusionInputIV");

                BigInteger initialCounter = ViewModel.ChaChaVisualization.InitialCounter;
                DiffusionInitialCounter.Text = Formatter.HexString(v == ChaCha.Version.DJB ? (ulong)initialCounter : (uint)initialCounter);
                int counterLength = (int)v.CounterBits / 8;
                ValidationRule counterRule = new DiffusionInputValidationRule(counterLength);
                IValueConverter counterConverter = new DiffusionCounterConverter(counterLength);
                InitDiffusionInputField(DiffusionInitialCounter, counterRule, counterConverter, "DiffusionInitialCounter");
            }
        }

        private void InitDiffusionInputField(TextBox inputField, ValidationRule validationRule, IValueConverter converter, string bindingPath)
        {
            Binding binding = new Binding(bindingPath) { Mode = BindingMode.TwoWay, Converter = converter };
            binding.ValidationRules.Add(validationRule);
            inputField.SetBinding(TextBox.TextProperty, binding);
            EditingCommands.ToggleInsert.Execute(null, inputField);
        }
    }
}