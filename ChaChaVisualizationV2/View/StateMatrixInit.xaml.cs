using Cryptool.Plugins.ChaCha.Util;
using Cryptool.Plugins.ChaChaVisualizationV2.Helper;
using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;
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

        private StateMatrixInitViewModel ViewModel { get; set; }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            StateMatrixInitViewModel ViewModel = (StateMatrixInitViewModel)e.NewValue;
            if (ViewModel != null)
            {
                this.ViewModel = ViewModel;
                // Colorize state background depending on Version
                ChaCha.Version v = ViewModel.Settings.Version;
                State13.Background = v.CounterBits == 64 ? Brushes.SkyBlue : Brushes.PaleGreen;

                // Add value changed event handler to action slider
                Root.ApplyTemplate();
                Slider actionSlider = (Slider)Root.Template.FindName("ActionSlider", Root);
                actionSlider.ValueChanged += ViewModel.ActionSliderValueChange;

                // State parameter diffusion values
                InitDiffusionStateParameters();

                // State encoding diffusion values
                InitDiffusionStateEncoding();

                // State matrix diffusion values
                InitDiffusionStateMatrix();
            }
        }

        /// <summary>
        /// Initialize the diffusion values in the state parameters section.
        /// </summary>
        private void InitDiffusionStateParameters()
        {
            ChaCha.Version v = ViewModel.Settings.Version;
            InitDiffusionValue(DiffusionInputKey, ViewModel.DiffusionInputKey, ViewModel.ChaCha.InputKey);
            InitDiffusionValue(DiffusionInputIV, ViewModel.DiffusionInputIV, ViewModel.ChaCha.InputIV);
            InitDiffusionValue(DiffusionInitialCounter, ViewModel.DiffusionInitialCounter, ViewModel.ChaCha.InitialCounter, v);
        }

        /// <summary>
        /// Initialize the diffusion values in the state encoding section.
        /// </summary>
        private void InitDiffusionStateEncoding()
        {
            InitDiffusionStateEncodingKey();
            InitDiffusionStateEncodingCounter();
            InitDiffusionStateEncodingIV();
        }

        /// <summary>
        /// Initialize the diffusion key values in the state encoding section.
        /// </summary>
        private void InitDiffusionStateEncodingKey()
        {
            InitDiffusionValue(DiffusionKeyEncodingInput, ViewModel.DiffusionInputKey, ViewModel.ChaCha.InputKey);

            string dKeyHexChunks = Formatter.Chunkify(Formatter.HexString(ViewModel.DiffusionInputKey), 8);
            string pKeyHexChunks = Formatter.Chunkify(Formatter.HexString(ViewModel.ChaCha.InputKey), 8);
            InitDiffusionValue(DiffusionKeyEncodingChunkify, dKeyHexChunks, pKeyHexChunks);

            string dKeyHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.DiffusionInputKey)), 8);
            string pKeyHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.ChaCha.InputKey)), 8);
            InitDiffusionValue(DiffusionKeyEncodingLittleEndian, dKeyHexChunksLE, pKeyHexChunksLE);
        }

        /// <summary>
        /// Initialize the diffusion counter values in the state encoding section.
        /// </summary>
        private void InitDiffusionStateEncodingCounter()
        {
            ChaCha.Version v = ViewModel.Settings.Version;

            InitDiffusionValue(DiffusionCounterEncodingInput, ViewModel.DiffusionInitialCounter, ViewModel.ChaCha.InitialCounter, v);

            if (v.CounterBits == 64)
            {
                ulong diffusionInitialCounter = (ulong)ViewModel.DiffusionInitialCounter;
                ulong initialCounter = (ulong)ViewModel.ChaCha.InitialCounter;

                string dCounterHexReverse = Formatter.HexString(Formatter.ReverseBytes(diffusionInitialCounter));
                string pCounterHexReverse = Formatter.HexString(Formatter.ReverseBytes(initialCounter));
                InitDiffusionValue(DiffusionCounterEncodingReverse, dCounterHexReverse, pCounterHexReverse);

                string dCounterHexChunks = Formatter.Chunkify(dCounterHexReverse, 8);
                string pCounterHexChunks = Formatter.Chunkify(pCounterHexReverse, 8);
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

                string dCounterHexChunks = Formatter.Chunkify(dCounterHexReverse, 8);
                string pCounterHexChunks = Formatter.Chunkify(pCounterHexReverse, 8);
                InitDiffusionValue(DiffusionCounterEncodingChunkify, dCounterHexChunks, pCounterHexChunks);

                string dCounterHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(Formatter.ReverseBytes(diffusionInitialCounter))), 8);
                string pCounterHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(Formatter.ReverseBytes(initialCounter))), 8);
                InitDiffusionValue(DiffusionCounterEncodingLittleEndian, dCounterHexChunksLE, pCounterHexChunksLE);
            }
        }

        /// <summary>
        /// Initialize the diffusion IV values in the state encoding section.
        /// </summary>
        private void InitDiffusionStateEncodingIV()
        {
            InitDiffusionValue(DiffusionIVEncodingInput, ViewModel.DiffusionInputIV, ViewModel.ChaCha.InputIV);

            string dIVHexChunks = Formatter.Chunkify(Formatter.HexString(ViewModel.DiffusionInputIV), 8);
            string pIVHexChunks = Formatter.Chunkify(Formatter.HexString(ViewModel.ChaCha.InputIV), 8);
            InitDiffusionValue(DiffusionIVEncodingChunkify, dIVHexChunks, pIVHexChunks);

            string dIVHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.DiffusionInputIV)), 8);
            string pIVHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.ChaCha.InputIV)), 8);
            InitDiffusionValue(DiffusionIVEncodingLittleEndian, dIVHexChunksLE, pIVHexChunksLE);
        }

        /// <summary>
        /// Initialize the diffusion values in the state matrix.
        /// </summary>
        private void InitDiffusionStateMatrix()
        {
            InitDiffusionStateMatrixKey();
            InitDiffusionStateMatrixCounter();
            InitDiffusionStateMatrixIV();
        }

        /// <summary>
        /// Initialize the diffusion key values in the state matrix.
        /// </summary>
        private void InitDiffusionStateMatrixKey()
        {
            string dKeyHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.DiffusionInputKey)), 8);
            string pKeyHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.ChaCha.InputKey)), 8);
            string[] encodedDkey = Regex.Replace(dKeyHexChunksLE, @" $", "").Split(' ');
            string[] encodedPKey = Regex.Replace(pKeyHexChunksLE, @" $", "").Split(' ');
            Debug.Assert(encodedDkey.Length == encodedPKey.Length, "key and diffusion key length should be the same.");
            Debug.Assert(encodedDkey.Length == 4 || encodedDkey.Length == 8, $"Encoded diffusion key length should be either 16 or 32 bytes. Is {encodedDkey.Length}");
            for (int i = 0; i < encodedDkey.Length; ++i)
            {
                RichTextBox rtb = (RichTextBox)this.FindName($"DiffusionState{i + 4}");
                InitDiffusionValue(rtb, encodedDkey[i], encodedPKey[i]);
            }
            if (encodedDkey.Length == 4)
            {
                for (int i = 0; i < encodedDkey.Length; ++i)
                {
                    RichTextBox rtb = (RichTextBox)this.FindName($"DiffusionState{i + 8}");
                    InitDiffusionValue(rtb, encodedDkey[i], encodedPKey[i]);
                }
            }
        }

        /// <summary>
        /// Initialize the diffusion counter values in the state matrix.
        /// </summary>
        private void InitDiffusionStateMatrixCounter()
        {
            ChaCha.Version v = ViewModel.Settings.Version;

            if (v.CounterBits == 64)
            {
                ulong diffusionInitialCounter = (ulong)ViewModel.DiffusionInitialCounter;
                ulong initialCounter = (ulong)ViewModel.ChaCha.InitialCounter;

                string dCounterHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(diffusionInitialCounter)), 8);
                string pCounterHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(initialCounter)), 8);
                string[] encodedDCounter = Regex.Replace(dCounterHexChunksLE, @" $", "").Split(' ');
                string[] encodedPCounter = Regex.Replace(pCounterHexChunksLE, @" $", "").Split(' ');
                Debug.Assert(encodedDCounter.Length == encodedPCounter.Length, "key and diffusion key length should be the same.");
                Debug.Assert(encodedDCounter.Length == 2, $"Encoded diffusion counter length should be 8 bytes for 64-bit counter. Is {encodedDCounter.Length}");
                InitDiffusionValue(DiffusionState12, encodedDCounter[0], encodedPCounter[0]);
                InitDiffusionValue(DiffusionState13, encodedDCounter[1], encodedPCounter[1]);
            }
            else
            {
                uint diffusionInitialCounter = (uint)ViewModel.DiffusionInitialCounter;
                uint initialCounter = (uint)ViewModel.ChaCha.InitialCounter;

                string dCounterHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(diffusionInitialCounter)), 8);
                string pCounterHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(initialCounter)), 8);
                string[] encodedDCounter = Regex.Replace(dCounterHexChunksLE, @" $", "").Split(' ');
                string[] encodedPCounter = Regex.Replace(pCounterHexChunksLE, @" $", "").Split(' ');
                Debug.Assert(encodedDCounter.Length == encodedPCounter.Length, "counter and diffusion counter length should be the same.");
                Debug.Assert(encodedDCounter.Length == 1, $"Encoded diffusion counter length should be 4 bytes for 32-bit counter. Is {encodedDCounter.Length}");
                InitDiffusionValue(DiffusionState12, encodedDCounter[0], encodedPCounter[0]);
            }
        }

        /// <summary>
        /// Initialize the diffusion IV values in the state matrix.
        /// </summary>
        private void InitDiffusionStateMatrixIV()
        {
            ChaCha.Version v = ViewModel.Settings.Version;
            string dIVHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.DiffusionInputIV)), 8);
            string pIVHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.ChaCha.InputIV)), 8);
            string[] encodedDIV = Regex.Replace(dIVHexChunksLE, @" $", "").Split(' ');
            string[] encodedPIV = Regex.Replace(pIVHexChunksLE, @" $", "").Split(' ');
            Debug.Assert(encodedDIV.Length == encodedPIV.Length, "iv and diffusion iv length should be the same.");
            if (v.CounterBits == 64)
            {
                Debug.Assert(encodedDIV.Length == 2, $"Encoded diffusion iv length should be 8 bytes for 64-bit counter. Is {encodedDIV.Length}");
                InitDiffusionValue(DiffusionState14, encodedDIV[0], encodedPIV[0]);
                InitDiffusionValue(DiffusionState15, encodedDIV[1], encodedPIV[1]);
            }
            else
            {
                Debug.Assert(encodedDIV.Length == 3, $"Encoded diffusion iv length should be 12 bytes for 64-bit counter. Is {encodedDIV.Length}");
                InitDiffusionValue(DiffusionState13, encodedDIV[0], encodedPIV[0]);
                InitDiffusionValue(DiffusionState14, encodedDIV[1], encodedPIV[1]);
                InitDiffusionValue(DiffusionState15, encodedDIV[2], encodedPIV[2]);
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