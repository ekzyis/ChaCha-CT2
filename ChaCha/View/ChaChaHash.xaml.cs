using Cryptool.Plugins.ChaCha.Helper.Converter;
using Cryptool.Plugins.ChaCha.Model;
using Cryptool.Plugins.ChaCha.ViewModel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cryptool.Plugins.ChaCha.View
{
    /// <summary>
    /// Interaction logic for ChaChaHash.xaml
    /// </summary>
    [PluginBase.Attributes.Localization("Cryptool.Plugins.ChaCha.Properties.Resources")]
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
            this.Dispatcher.Invoke(delegate
            {
                // Seems like WPF somehow calls this function but without a ViewModel even though it is attached to one?
                // Mhhh... we'll ignore it for now and just return, if this happens.
                // Maybe some async issues?
                if (ViewModel == null) return;
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
                else if (e.PropertyName.StartsWith("DiffusionQRIn"))
                {
                    HandleDiffusionQRInChange();
                }
                else if (e.PropertyName.StartsWith("DiffusionQROut"))
                {
                    HandleDiffusionQROutChange();
                }
                else if (e.PropertyName.StartsWith("DiffusionQRStep"))
                {
                    HandleDiffusionQRStepChange(e.PropertyName);
                }
            });
        }

        private void InitOrClearDiffusionValue(RichTextBox rtb, uint? diffusionStateValue, uint? stateValue)
        {
            // TODO(clarify) Why can the stateValue be null if the diffusionValue is not null? Shouldn't they always be together null or not?
            if (diffusionStateValue != null && stateValue != null)
            {
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(rtb, (uint)diffusionStateValue, (uint)stateValue);
            }
            else
            {
                rtb.Document.Blocks.Clear();
            }
        }

        private void InitOrClearXorValue(RichTextBox rtb, uint? diffusionValue, uint? value)
        {
            if (diffusionValue != null && value != null)
            {
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitXORValue(rtb, (uint)diffusionValue, (uint)value);
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
                RichTextBox rtbXor = (RichTextBox)FindName($"DiffusionStateXOR{i}");
                uint? diffusionStateValue = ViewModel.DiffusionStateValues[i].Value;
                uint? stateValue = ViewModel.StateValues[i].Value;
                InitOrClearDiffusionValue(rtb, diffusionStateValue, stateValue);
                InitOrClearXorValue(rtbXor, diffusionStateValue, stateValue);
            }
        }

        private void HandleDiffusionOriginalStateChange()
        {
            for (int i = 0; i < 16; ++i)
            {
                RichTextBox rtb = (RichTextBox)FindName($"DiffusionOriginalState{i}");
                RichTextBox rtbXor = (RichTextBox)FindName($"DiffusionOriginalStateXOR{i}");
                uint? diffusionStateValue = ViewModel.DiffusionOriginalState[i].Value;
                uint? stateValue = ViewModel.OriginalState[i].Value;
                InitOrClearDiffusionValue(rtb, diffusionStateValue, stateValue);
                InitOrClearXorValue(rtbXor, diffusionStateValue, stateValue);
            }
        }

        private void HandleDiffusionAdditionResultStateChange()
        {
            for (int i = 0; i < 16; ++i)
            {
                RichTextBox rtb = (RichTextBox)FindName($"DiffusionAdditionResultState{i}");
                RichTextBox rtbXor = (RichTextBox)FindName($"DiffusionAdditionResultStateXOR{i}");
                uint? diffusionStateValue = ViewModel.DiffusionAdditionResultState[i].Value;
                uint? stateValue = ViewModel.AdditionResultState[i].Value;
                InitOrClearDiffusionValue(rtb, diffusionStateValue, stateValue);
                InitOrClearXorValue(rtbXor, diffusionStateValue, stateValue);
            }
        }

        private void HandleDiffusionLittleEndianStateChange()
        {
            for (int i = 0; i < 16; ++i)
            {
                RichTextBox rtb = (RichTextBox)FindName($"DiffusionLittleEndianState{i}");
                RichTextBox rtbXor = (RichTextBox)FindName($"DiffusionLittleEndianStateXOR{i}");
                uint? diffusionStateValue = ViewModel.DiffusionLittleEndianState[i].Value;
                uint? stateValue = ViewModel.LittleEndianState[i].Value;
                InitOrClearDiffusionValue(rtb, diffusionStateValue, stateValue);
                InitOrClearXorValue(rtbXor, diffusionStateValue, stateValue);
            }
        }

        private void HandleDiffusionQRInChange()
        {
            RichTextBox rtbA = (RichTextBox)FindName($"DiffusionQRInA");
            RichTextBox rtbAXor = (RichTextBox)FindName($"DiffusionQRInAXOR");
            uint? diffusionStateValueA = ViewModel.DiffusionQRInA.Value;
            uint? stateValueA = ViewModel.QRInA.Value;
            InitOrClearDiffusionValue(rtbA, diffusionStateValueA, stateValueA);
            InitOrClearXorValue(rtbAXor, diffusionStateValueA, stateValueA);

            RichTextBox rtbB = (RichTextBox)FindName($"DiffusionQRInB");
            RichTextBox rtbBXor = (RichTextBox)FindName($"DiffusionQRInBXOR");
            uint? diffusionStateValueB = ViewModel.DiffusionQRInB.Value;
            uint? stateValueB = ViewModel.QRInB.Value;
            InitOrClearDiffusionValue(rtbB, diffusionStateValueB, stateValueB);
            InitOrClearXorValue(rtbBXor, diffusionStateValueB, stateValueB);

            RichTextBox rtbC = (RichTextBox)FindName($"DiffusionQRInC");
            RichTextBox rtbCXor = (RichTextBox)FindName($"DiffusionQRInCXOR");
            uint? diffusionStateValueC = ViewModel.DiffusionQRInC.Value;
            uint? stateValueC = ViewModel.QRInC.Value;
            InitOrClearDiffusionValue(rtbC, diffusionStateValueC, stateValueC);
            InitOrClearXorValue(rtbCXor, diffusionStateValueC, stateValueC);

            RichTextBox rtbD = (RichTextBox)FindName($"DiffusionQRInD");
            RichTextBox rtbDXor = (RichTextBox)FindName($"DiffusionQRInDXOR");
            uint? diffusionStateValueD = ViewModel.DiffusionQRInD.Value;
            uint? stateValueD = ViewModel.QRInD.Value;
            InitOrClearDiffusionValue(rtbD, diffusionStateValueD, stateValueD);
            InitOrClearXorValue(rtbDXor, diffusionStateValueD, stateValueD);
        }

        private void HandleDiffusionQROutChange()
        {
            RichTextBox rtbA = (RichTextBox)FindName($"DiffusionQROutA");
            RichTextBox rtbAXor = (RichTextBox)FindName($"DiffusionQROutAXOR");
            uint? diffusionStateValueA = ViewModel.DiffusionQROutA.Value;
            uint? stateValueA = ViewModel.QROutA.Value;
            InitOrClearDiffusionValue(rtbA, diffusionStateValueA, stateValueA);
            InitOrClearXorValue(rtbAXor, diffusionStateValueA, stateValueA);

            RichTextBox rtbB = (RichTextBox)FindName($"DiffusionQROutB");
            RichTextBox rtbBXor = (RichTextBox)FindName($"DiffusionQROutBXOR");
            uint? diffusionStateValueB = ViewModel.DiffusionQROutB.Value;
            uint? stateValueB = ViewModel.QROutB.Value;
            InitOrClearDiffusionValue(rtbB, diffusionStateValueB, stateValueB);
            InitOrClearXorValue(rtbBXor, diffusionStateValueB, stateValueB);

            RichTextBox rtbC = (RichTextBox)FindName($"DiffusionQROutC");
            RichTextBox rtbCXor = (RichTextBox)FindName($"DiffusionQROutCXOR");
            uint? diffusionStateValueC = ViewModel.DiffusionQROutC.Value;
            uint? stateValueC = ViewModel.QROutC.Value;
            InitOrClearDiffusionValue(rtbC, diffusionStateValueC, stateValueC);
            InitOrClearXorValue(rtbCXor, diffusionStateValueC, stateValueC);

            RichTextBox rtbD = (RichTextBox)FindName($"DiffusionQROutD");
            RichTextBox rtbDXor = (RichTextBox)FindName($"DiffusionQROutDXOR");
            uint? diffusionStateValueD = ViewModel.DiffusionQROutD.Value;
            uint? stateValueD = ViewModel.QROutD.Value;
            InitOrClearDiffusionValue(rtbD, diffusionStateValueD, stateValueD);
            InitOrClearXorValue(rtbDXor, diffusionStateValueD, stateValueD);
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
            RichTextBox rtbXor = (RichTextBox)FindName($"QRValue{operation}DiffusionXOR_{index}");
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
            InitOrClearXorValue(rtbXor, diffusionValue, primaryValue);
        }

        #endregion Diffusion

        #region User Input

        private void InitKeystreamBlockInput()
        {
            TextBox keystreamBlockInput = (TextBox)this.FindName("KeystreamBlockInput");
            int maxKeystreamBlock = ViewModel.ChaCha.TotalKeystreamBlocks;
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