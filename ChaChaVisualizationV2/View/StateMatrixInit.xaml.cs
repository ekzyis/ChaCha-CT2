using Cryptool.Plugins.ChaCha.Util;
using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel;
using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Cryptool.Plugins.ChaChaVisualizationV2.View
{
    /// <summary>
    /// Interaction logic for StateMatrixInitialization.xaml
    /// </summary>
    public partial class StateMatrixInit : UserControl
    {
        public StateMatrixInit()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            StateMatrixInitViewModel ViewModel = (StateMatrixInitViewModel)e.NewValue;
            if (ViewModel != null)
            {
                // Colorize state background depending on Version
                ChaCha.Version v = ViewModel.Settings.Version;
                State13.Background = v.CounterBits == 64 ? Brushes.SkyBlue : Brushes.PaleGreen;

                // Create the FlowDocuments of the RichTextBoxes for the diffusion values.
                // Marks differences in the diffusion value red.
                InitDiffusionValue(DiffusionInputKey, ViewModel.DiffusionInputKey, ViewModel.ChaCha.InputKey);
                InitDiffusionValue(DiffusionInputIV, ViewModel.DiffusionInputIV, ViewModel.ChaCha.InputIV);
                InitDiffusionValue(DiffusionInitialCounter, ViewModel.DiffusionInitialCounter, ViewModel.ChaCha.InitialCounter, v);
            }
        }

        private void InitDiffusionValue(RichTextBox rtb, byte[] diffusion, byte[] primary)
        {
            if (diffusion.Length != primary.Length) throw new ArgumentException("Diffusion value must be of same length as primary value.");
            if (diffusion.Length % 2 != 0) throw new ArgumentException("Length must be even");
            FlowDocument flowDocument = new FlowDocument();
            Paragraph paragraph = new Paragraph();
            for (int i = 0; i < diffusion.Length; ++i)
            {
                string dHex = diffusion[i].ToString("X2");
                string pHex = primary[i].ToString("x2");
                char dHex1 = dHex[0];
                char dHex2 = dHex[1];
                char pHex1 = pHex[0];
                char pHex2 = pHex[1];
                paragraph.Inlines.Add(RedIfDifferent(dHex1, pHex1));
                paragraph.Inlines.Add(RedIfDifferent(dHex2, pHex2));
            }
            flowDocument.Blocks.Add(paragraph);
            rtb.Document = flowDocument;
        }

        private void InitDiffusionValue(RichTextBox rtb, BigInteger diffusion, BigInteger primary, ChaCha.Version version)
        {
            if (version.CounterBits == 64)
            {
                byte[] diffusionBytes = ByteUtil.GetBytesBE((ulong)diffusion);
                byte[] primaryBytes = ByteUtil.GetBytesBE((ulong)primary);
                InitDiffusionValue(rtb, diffusionBytes, primaryBytes);
            }
            else
            {
                byte[] diffusionBytes = ByteUtil.GetBytesBE((uint)diffusion);
                byte[] primaryBytes = ByteUtil.GetBytesBE((uint)primary);
                InitDiffusionValue(rtb, diffusionBytes, primaryBytes);
            }
        }

        /// <summary>
        /// Returns a TextBox with the character d in red if d != v else black.
        /// </summary>
        private TextBlock RedIfDifferent(char d, char v)
        {
            return new TextBlock() { Text = d.ToString(), Foreground = d != v ? Brushes.Red : Brushes.Black };
        }
    }
}