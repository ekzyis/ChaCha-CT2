using Cryptool.Plugins.ChaChaVisualizationV2.Helper.Converter;
using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cryptool.Plugins.ChaChaVisualizationV2.View
{
    /// <summary>
    /// Interaction logic for ChaChaHash.xaml
    /// </summary>
    public partial class ChaChaHash : UserControl
    {
        public ChaChaHash()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
        }

        private ChaChaHashViewModel ViewModel { get; set; }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = (ChaChaHashViewModel)e.NewValue;
            if (ViewModel != null)
            {
                ActionViewBase.AddEventHandlers(ViewModel, Root);

                InitKeystreamBlockInput();
                InitRoundInput();
                InitQRInput();
            }
        }

        private void InitKeystreamBlockInput()
        {
            TextBox keystreamBlockInput = (TextBox)this.FindName("KeystreamBlockInput");
            int maxKeystreamBlock = ViewModel.ChaChaVisualization.TotalKeystreamBlocks;
            Binding binding = new Binding("CurrentKeystreamBlockIndex")
            {
                Mode = BindingMode.TwoWay,
                Converter = new ZeroBasedIndexToOneBasedIndex(),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            ActionViewBase.InitUserInputField(keystreamBlockInput, binding, 1, maxKeystreamBlock, ViewModel.KeystreamBlockInputHandler);
        }

        private void InitRoundInput()
        {
            TextBox roundInputTextBox = (TextBox)this.FindName("RoundInput");
            int maxRound = ViewModel.Settings.Rounds;
            Binding binding = new Binding("CurrentUserRoundIndex")
            {
                Mode = BindingMode.TwoWay,
                Converter = new ZeroBasedIndexToOneBasedIndex(),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            ActionViewBase.InitUserInputField(roundInputTextBox, binding, 1, maxRound, ViewModel.RoundInputHandler);
        }

        private void InitQRInput()
        {
            TextBox qrInputTextBox = (TextBox)this.FindName("QRInput");
            Binding binding = new Binding("CurrentUserQRIndex")
            {
                Mode = BindingMode.TwoWay,
                Converter = new ZeroBasedIndexToOneBasedIndex(),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            ActionViewBase.InitUserInputField(qrInputTextBox, binding, 1, 4, ViewModel.QRInputHandler);
        }
    }
}