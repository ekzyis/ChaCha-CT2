using Cryptool.Plugins.ChaChaVisualizationV2.Helper.Converter;
using Cryptool.Plugins.ChaChaVisualizationV2.Model;
using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
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
            else if (e.PropertyName == "DiffusionQRIn")
            {
                HandleDiffusionQRInChange();
            }
            else if (e.PropertyName == "DiffusionQROut")
            {
                HandleDiffusionQROutChange();
            }
            else if (e.PropertyName.StartsWith("DiffusionQRStep"))
            {
                HandleDiffusionQRStepChange(e.PropertyName);
            }
        }

        private void InitOrClearDiffusionValue(RichTextBox rtb, uint? diffusionStateValue, uint? stateValue)
        {
            if (diffusionStateValue != null)
            {
                Plugins.ChaChaVisualizationV2.ViewModel.Components.Diffusion.InitDiffusionValue(rtb, (uint)diffusionStateValue, (uint)stateValue);
            }
            else
            {
                rtb.Document.Blocks.Clear();
            }
        }

        private void HandleDiffusionStateValuesChange()
        {
            for (int i = 0; i < 16; ++i)
            {
                RichTextBox rtb = (RichTextBox)FindName($"DiffusionState{i}");
                uint? diffusionStateValue = ViewModel.DiffusionStateValues[i].Value;
                uint? stateValue = ViewModel.StateValues[i].Value;
                InitOrClearDiffusionValue(rtb, diffusionStateValue, stateValue);
            }
        }

        private void HandleDiffusionOriginalStateChange()
        {
            for (int i = 0; i < 16; ++i)
            {
                RichTextBox rtb = (RichTextBox)FindName($"DiffusionOriginalState{i}");
                uint? diffusionStateValue = ViewModel.DiffusionOriginalState[i].Value;
                uint? stateValue = ViewModel.OriginalState[i].Value;
                InitOrClearDiffusionValue(rtb, diffusionStateValue, stateValue);
            }
        }

        private void HandleDiffusionAdditionResultStateChange()
        {
            for (int i = 0; i < 16; ++i)
            {
                RichTextBox rtb = (RichTextBox)FindName($"DiffusionAdditionResultState{i}");
                uint? diffusionStateValue = ViewModel.DiffusionAdditionResultState[i].Value;
                uint? stateValue = ViewModel.AdditionResultState[i].Value;
                InitOrClearDiffusionValue(rtb, diffusionStateValue, stateValue);
            }
        }

        private void HandleDiffusionLittleEndianStateChange()
        {
            for (int i = 0; i < 16; ++i)
            {
                RichTextBox rtb = (RichTextBox)FindName($"DiffusionLittleEndianState{i}");
                uint? diffusionStateValue = ViewModel.DiffusionLittleEndianState[i].Value;
                uint? stateValue = ViewModel.LittleEndianState[i].Value;
                InitOrClearDiffusionValue(rtb, diffusionStateValue, stateValue);
            }
        }

        private void HandleDiffusionQRInChange()
        {
            RichTextBox rtbA = (RichTextBox)FindName($"QRInADiffusion");
            uint? diffusionStateValueA = ViewModel.DiffusionQRInA.Value;
            uint? stateValueA = ViewModel.QRInA.Value;
            InitOrClearDiffusionValue(rtbA, diffusionStateValueA, stateValueA);

            RichTextBox rtbB = (RichTextBox)FindName($"QRInADiffusion");
            uint? diffusionStateValueB = ViewModel.DiffusionQRInB.Value;
            uint? stateValueB = ViewModel.QRInB.Value;
            InitOrClearDiffusionValue(rtbB, diffusionStateValueB, stateValueB);

            RichTextBox rtbC = (RichTextBox)FindName($"QRInADiffusion");
            uint? diffusionStateValueC = ViewModel.DiffusionQRInC.Value;
            uint? stateValueC = ViewModel.QRInC.Value;
            InitOrClearDiffusionValue(rtbC, diffusionStateValueC, stateValueC);

            RichTextBox rtbD = (RichTextBox)FindName($"QRInADiffusion");
            uint? diffusionStateValueD = ViewModel.DiffusionQRInD.Value;
            uint? stateValueD = ViewModel.QRInD.Value;
            InitOrClearDiffusionValue(rtbD, diffusionStateValueD, stateValueD);
        }

        private void HandleDiffusionQROutChange()
        {
            RichTextBox rtbA = (RichTextBox)FindName($"QROutADiffusion");
            uint? diffusionStateValueA = ViewModel.DiffusionQROutA.Value;
            uint? stateValueA = ViewModel.QROutA.Value;
            InitOrClearDiffusionValue(rtbA, diffusionStateValueA, stateValueA);

            RichTextBox rtbB = (RichTextBox)FindName($"QROutADiffusion");
            uint? diffusionStateValueB = ViewModel.DiffusionQROutB.Value;
            uint? stateValueB = ViewModel.QROutB.Value;
            InitOrClearDiffusionValue(rtbB, diffusionStateValueB, stateValueB);

            RichTextBox rtbC = (RichTextBox)FindName($"QROutADiffusion");
            uint? diffusionStateValueC = ViewModel.DiffusionQROutC.Value;
            uint? stateValueC = ViewModel.QROutC.Value;
            InitOrClearDiffusionValue(rtbC, diffusionStateValueC, stateValueC);

            RichTextBox rtbD = (RichTextBox)FindName($"QROutADiffusion");
            uint? diffusionStateValueD = ViewModel.DiffusionQROutD.Value;
            uint? stateValueD = ViewModel.QROutD.Value;
            InitOrClearDiffusionValue(rtbD, diffusionStateValueD, stateValueD);
        }

        private void HandleDiffusionQRStepChange(string propertyName)
        {
            var pattern = @"DiffusionQRStep\[([0-9])\]\.(Add|XOR|Shift)";
            var match = Regex.Match(propertyName, pattern);
            if (match.Success)
            {
                int index = int.Parse(match.Groups[1].Value);
                string operation = match.Groups[2].Value;
                HandleDiffusionQRStepChange(index, operation);
            }
            else
            {
                Debug.Assert(propertyName == "DiffusionQRStep");
                for (int i = 0; i < 4; ++i)
                {
                    HandleDiffusionQRStepChange(i, "Add");
                    HandleDiffusionQRStepChange(i, "XOR");
                    HandleDiffusionQRStepChange(i, "Shift");
                }
            }
        }

        private void HandleDiffusionQRStepChange(int index, string operation)
        {
            RichTextBox rtb = (RichTextBox)FindName($"QRValue{operation}Diffusion_{index}");
            VisualQRStep diffusionQrStep = ViewModel.DiffusionQRStep[index];
            VisualQRStep primaryQrStep = ViewModel.QRStep[index];
            uint? diffusionValue; uint? primaryValue;
            if (operation == "Add")
            {
                diffusionValue = diffusionQrStep.Add.Value;
                primaryValue = primaryQrStep.Add.Value;
            }
            else if (operation == "XOR")
            {
                diffusionValue = diffusionQrStep.XOR.Value;
                primaryValue = primaryQrStep.XOR.Value;
            }
            else if (operation == "Shift")
            {
                diffusionValue = diffusionQrStep.Shift.Value;
                primaryValue = primaryQrStep.Shift.Value;
            }
            else
            {
                throw new InvalidOperationException("No matching operation found.");
            }
            InitOrClearDiffusionValue(rtb, diffusionValue, primaryValue);
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