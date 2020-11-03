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

                // Add value changed event handler to action slider
                Root.ApplyTemplate();
                Slider actionSlider = (Slider)Root.Template.FindName("ActionSlider", Root);
                actionSlider.ValueChanged += ViewModel.ActionSliderValueChange;

                // State parameter diffusion values
                InitDiffusionValue(DiffusionInputKey, ViewModel.DiffusionInputKey, ViewModel.ChaCha.InputKey);
                InitDiffusionValue(DiffusionInputIV, ViewModel.DiffusionInputIV, ViewModel.ChaCha.InputIV);
                InitDiffusionValue(DiffusionInitialCounter, ViewModel.DiffusionInitialCounter, ViewModel.ChaCha.InitialCounter, v);

                // State encoding diffusion values
                // -- Key
                InitDiffusionValue(DiffusionKeyEncodingInput, ViewModel.DiffusionInputKey, ViewModel.ChaCha.InputKey);

                string dKeyHexChunks = Formatter.Chunkify(Formatter.HexString(ViewModel.DiffusionInputKey), 8);
                string pKeyHexChunks = Formatter.Chunkify(Formatter.HexString(ViewModel.ChaCha.InputKey), 8);
                InitDiffusionValue(DiffusionKeyEncodingChunkify, dKeyHexChunks, pKeyHexChunks);

                string dKeyHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.DiffusionInputKey)), 8);
                string pKeyHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.ChaCha.InputKey)), 8);
                InitDiffusionValue(DiffusionKeyEncodingLittleEndian, dKeyHexChunksLE, pKeyHexChunksLE);

                // -- Counter
                InitDiffusionValue(DiffusionCounterEncodingInput, ViewModel.DiffusionInitialCounter, ViewModel.ChaCha.InitialCounter, v);

                if (v.CounterBits == 64)
                {
                    ulong diffusionInitialCounter = (ulong)ViewModel.DiffusionInitialCounter;
                    ulong initialCounter = (ulong)ViewModel.ChaCha.InitialCounter;

                    string dCounterHexChunks = Formatter.Chunkify(Formatter.HexString(diffusionInitialCounter), 8);
                    string pCounterHexChunks = Formatter.Chunkify(Formatter.HexString(initialCounter), 8);
                    InitDiffusionValue(DiffusionCounterEncodingChunkify, dCounterHexChunks, pCounterHexChunks);

                    string dCounterHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(diffusionInitialCounter)), 8);
                    string pCounterHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(initialCounter)), 8);
                    InitDiffusionValue(DiffusionCounterEncodingLittleEndian, dCounterHexChunksLE, pCounterHexChunksLE);
                }
                else
                {
                    uint diffusionInitialCounter = (uint)ViewModel.DiffusionInitialCounter;
                    uint initialCounter = (uint)ViewModel.ChaCha.InitialCounter;

                    string dCounterHexReverse = Formatter.HexString(Formatter.ReverseBytes(diffusionInitialCounter));
                    string pCounterHexReverse = Formatter.HexString(Formatter.ReverseBytes(initialCounter));
                    InitDiffusionValue(DiffusionCounterEncodingReverse, dCounterHexReverse, pCounterHexReverse);

                    string dCounterHexChunks = Formatter.Chunkify(Formatter.HexString(diffusionInitialCounter), 8);
                    string pCounterHexChunks = Formatter.Chunkify(Formatter.HexString(initialCounter), 8);
                    InitDiffusionValue(DiffusionCounterEncodingChunkify, dCounterHexChunks, pCounterHexChunks);

                    string dCounterHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(diffusionInitialCounter)), 8);
                    string pCounterHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(initialCounter)), 8);
                    InitDiffusionValue(DiffusionCounterEncodingLittleEndian, dCounterHexChunksLE, pCounterHexChunksLE);
                }

                // -- IV
                InitDiffusionValue(DiffusionIVEncodingInput, ViewModel.DiffusionInputIV, ViewModel.ChaCha.InputIV);

                string dIVHexChunks = Formatter.Chunkify(Formatter.HexString(ViewModel.DiffusionInputIV), 8);
                string pIVHexChunks = Formatter.Chunkify(Formatter.HexString(ViewModel.ChaCha.InputIV), 8);
                InitDiffusionValue(DiffusionIVEncodingChunkify, dIVHexChunks, pIVHexChunks);

                string dIVHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.DiffusionInputIV)), 8);
                string pIVHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.ChaCha.InputIV)), 8);
                InitDiffusionValue(DiffusionIVEncodingLittleEndian, dIVHexChunksLE, pIVHexChunksLE);
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