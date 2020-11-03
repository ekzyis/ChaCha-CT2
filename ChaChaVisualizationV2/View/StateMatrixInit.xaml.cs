using Cryptool.Plugins.ChaCha.Util;
using Cryptool.Plugins.ChaChaVisualizationV2.Helper;
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

                // Create FlowDocuments of diffusion values during state encoding
                InitDiffusionValue(DiffusionKeyEncodingInput, ViewModel.DiffusionInputKey, ViewModel.ChaCha.InputKey);

                string dHexChunks = Formatter.Chunkify(Formatter.HexString(ViewModel.DiffusionInputKey), 8);
                string pHexChunks = Formatter.Chunkify(Formatter.HexString(ViewModel.ChaCha.InputKey), 8);
                InitDiffusionValue(DiffusionKeyEncodingChunkify, dHexChunks, pHexChunks);

                // Add value changed event handler to action slider
                Root.ApplyTemplate();
                Slider actionSlider = (Slider)Root.Template.FindName("ActionSlider", Root);
                actionSlider.ValueChanged += ViewModel.ActionSliderValueChange;
            }
        }

        /// <summary>
        /// Set the Document of the RichTextBox wit the diffusion value as hex string; using byte array for comparison; marking differences red.
        /// </summary>
        private void InitDiffusionValue(RichTextBox rtb, byte[] diffusion, byte[] primary)
        {
            string dHex = Formatter.HexString(diffusion);
            string pHex = Formatter.HexString(primary);
            InitDiffusionValue(rtb, dHex, pHex);
        }

        /// <summary>
        /// Set the Document of the RichTextBox with the diffusion value; marking differences red.
        /// Version is used to determine counter size.
        /// </summary>
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
        /// Set the document of the RichTextBox with the diffusion value as hex string; using strings for comparison; marking differences red.
        /// </summary>
        private void InitDiffusionValue(RichTextBox rtb, string dHex, string pHex)
        {
            if (dHex.Length != pHex.Length) throw new ArgumentException("Diffusion value must be of same length as primary value.");
            if (dHex.Length % 2 != 0) throw new ArgumentException("Length must be even");
            FlowDocument flowDocument = new FlowDocument();
            Paragraph paragraph = new Paragraph();
            for (int i = 0; i < dHex.Length; i += 2)
            {
                char dChar1 = dHex[i];
                char dChar2 = dHex[i + 1];
                char pChar1 = pHex[i];
                char pChar2 = pHex[i + 1];
                paragraph.Inlines.Add(RedIfDifferent(dChar1, pChar1));
                paragraph.Inlines.Add(RedIfDifferent(dChar2, pChar2));
            }
            flowDocument.Blocks.Add(paragraph);
            rtb.Document = flowDocument;
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