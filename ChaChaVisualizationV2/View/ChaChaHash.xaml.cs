using Cryptool.Plugins.ChaChaVisualizationV2.Helper.Converter;
using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel;
using System.ComponentModel;
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
                ViewModel.PropertyChanged += new PropertyChangedEventHandler(OnViewModelPropertyChange);
                ActionViewBase.AddEventHandlers(ViewModel, Root);

                InitKeystreamBlockInput();
                InitRoundInput();
                InitQRInput();
            }
        }

        #region Diffusion

        private void OnViewModelPropertyChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DiffusionStateValues")
            {
                HandleDiffusionStateValuesChange();
            }
            else if (e.PropertyName == "DiffusionOriginalState")
            {
                HandleDiffusionOriginalStateChange();
            }
            else if (e.PropertyName == "DiffusionAdditionResultState")
            {
                HandleDiffusionAdditionResultStateChange();
            }
            else if (e.PropertyName == "DiffusionLittleEndianState")
            {
                HandleDiffusionLittleEndianStateChange();
            }
        }

        private void HandleDiffusionStateValuesChange()
        {
            for (int i = 0; i < 16; ++i)
            {
                RichTextBox rtb = (RichTextBox)FindName($"DiffusionState{i}");
                uint? diffusionStateValue = ViewModel.DiffusionStateValues[i].Value;
                uint? stateValue = ViewModel.StateValues[i].Value;
                if (diffusionStateValue != null)
                {
                    Plugins.ChaChaVisualizationV2.ViewModel.Components.Diffusion.InitDiffusionValue(rtb, (uint)diffusionStateValue, (uint)stateValue);
                }
                else
                {
                    rtb.Document.Blocks.Clear();
                }
            }
        }

        private void HandleDiffusionOriginalStateChange()
        {
        }

        private void HandleDiffusionAdditionResultStateChange()
        {
        }

        private void HandleDiffusionLittleEndianStateChange()
        {
        }

        #endregion Diffusion

        #region User Input

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

        #endregion User Input
    }
}