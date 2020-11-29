using Cryptool.Plugins.ChaCha.Helper;
using Cryptool.Plugins.ChaCha.ViewModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cryptool.Plugins.ChaCha.View
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
                ActionViewBase.AddEventHandlers(ViewModel, Root);

                this.ViewModel = ViewModel;
                // Colorize state background depending on Version
                Version v = ViewModel.Settings.Version;
                State13.Background = v.CounterBits == 64 ? Brushes.SkyBlue : Brushes.PaleGreen;

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
            Version v = ViewModel.Settings.Version;
            Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionInputKey, ViewModel.DiffusionInputKey, ViewModel.ChaCha.InputKey);
            Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionInputIV, ViewModel.DiffusionInputIV, ViewModel.ChaCha.InputIV);
            Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionInitialCounter, ViewModel.DiffusionInitialCounter, ViewModel.ChaCha.InitialCounter, v);
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
            Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionKeyEncodingInput, ViewModel.DiffusionInputKey, ViewModel.ChaCha.InputKey);

            string dKeyHexChunks = Formatter.Chunkify(Formatter.HexString(ViewModel.DiffusionInputKey), 8);
            string pKeyHexChunks = Formatter.Chunkify(Formatter.HexString(ViewModel.ChaCha.InputKey), 8);
            Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionKeyEncodingChunkify, dKeyHexChunks, pKeyHexChunks);

            string dKeyHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.DiffusionInputKey)), 8);
            string pKeyHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.ChaCha.InputKey)), 8);
            Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionKeyEncodingLittleEndian, dKeyHexChunksLE, pKeyHexChunksLE);
        }

        /// <summary>
        /// Initialize the diffusion counter values in the state encoding section.
        /// </summary>
        private void InitDiffusionStateEncodingCounter()
        {
            Version v = ViewModel.Settings.Version;

            Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionCounterEncodingInput, ViewModel.DiffusionInitialCounter, ViewModel.ChaCha.InitialCounter, v);

            if (v.CounterBits == 64)
            {
                ulong diffusionInitialCounter = (ulong)ViewModel.DiffusionInitialCounter;
                ulong initialCounter = (ulong)ViewModel.ChaCha.InitialCounter;

                string dCounterHexReverse = Formatter.HexString(Formatter.ReverseBytes(diffusionInitialCounter));
                string pCounterHexReverse = Formatter.HexString(Formatter.ReverseBytes(initialCounter));
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionCounterEncodingReverse, dCounterHexReverse, pCounterHexReverse);

                string dCounterHexChunks = Formatter.Chunkify(dCounterHexReverse, 8);
                string pCounterHexChunks = Formatter.Chunkify(pCounterHexReverse, 8);
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionCounterEncodingChunkify, dCounterHexChunks, pCounterHexChunks);

                string dCounterHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(diffusionInitialCounter)), 8);
                string pCounterHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(initialCounter)), 8);
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionCounterEncodingLittleEndian, dCounterHexChunksLE, pCounterHexChunksLE);
            }
            else
            {
                uint diffusionInitialCounter = (uint)ViewModel.DiffusionInitialCounter;
                uint initialCounter = (uint)ViewModel.ChaCha.InitialCounter;

                string dCounterHexReverse = Formatter.HexString(Formatter.ReverseBytes(diffusionInitialCounter));
                string pCounterHexReverse = Formatter.HexString(Formatter.ReverseBytes(initialCounter));
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionCounterEncodingReverse, dCounterHexReverse, pCounterHexReverse);

                string dCounterHexChunks = Formatter.Chunkify(dCounterHexReverse, 8);
                string pCounterHexChunks = Formatter.Chunkify(pCounterHexReverse, 8);
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionCounterEncodingChunkify, dCounterHexChunks, pCounterHexChunks);

                string dCounterHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(Formatter.ReverseBytes(diffusionInitialCounter))), 8);
                string pCounterHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(Formatter.ReverseBytes(initialCounter))), 8);
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionCounterEncodingLittleEndian, dCounterHexChunksLE, pCounterHexChunksLE);
            }
        }

        /// <summary>
        /// Initialize the diffusion IV values in the state encoding section.
        /// </summary>
        private void InitDiffusionStateEncodingIV()
        {
            Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionIVEncodingInput, ViewModel.DiffusionInputIV, ViewModel.ChaCha.InputIV);

            string dIVHexChunks = Formatter.Chunkify(Formatter.HexString(ViewModel.DiffusionInputIV), 8);
            string pIVHexChunks = Formatter.Chunkify(Formatter.HexString(ViewModel.ChaCha.InputIV), 8);
            Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionIVEncodingChunkify, dIVHexChunks, pIVHexChunks);

            string dIVHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.DiffusionInputIV)), 8);
            string pIVHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.ChaCha.InputIV)), 8);
            Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionIVEncodingLittleEndian, dIVHexChunksLE, pIVHexChunksLE);
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
            byte[] dKeyLe = Formatter.LittleEndian(ViewModel.DiffusionInputKey);
            byte[] pKeyLe = Formatter.LittleEndian(ViewModel.ChaCha.InputKey);

            byte[] xor = ByteUtil.XOR(dKeyLe, pKeyLe);

            string dKeyHexChunksLE = Formatter.Chunkify(Formatter.HexString(dKeyLe), 8);
            string pKeyHexChunksLE = Formatter.Chunkify(Formatter.HexString(pKeyLe), 8);
            string xorHex = Formatter.Chunkify(Formatter.HexString(xor), 8);

            string[] encodedDkey = Regex.Replace(dKeyHexChunksLE, @" $", "").Split(' ');
            string[] encodedPKey = Regex.Replace(pKeyHexChunksLE, @" $", "").Split(' ');
            string[] encodedXor = Regex.Replace(xorHex, @" $", "").Split(' ');

            Debug.Assert(encodedDkey.Length == encodedPKey.Length, "key and diffusion key length should be the same.");
            Debug.Assert(encodedPKey.Length == encodedXor.Length, "key and xor length should be the same");
            Debug.Assert(encodedDkey.Length == 4 || encodedDkey.Length == 8, $"Encoded diffusion key length should be either 16 or 32 bytes. Is {encodedDkey.Length}");
            for (int i = 0; i < encodedDkey.Length; ++i)
            {
                RichTextBox rtb = (RichTextBox)this.FindName($"DiffusionState{i + 4}");
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(rtb, encodedDkey[i], encodedPKey[i]);
                TextBox tbXOR = (TextBox)this.FindName($"XORState{i + 4}");
                tbXOR.Text = encodedXor[i];
            }
            if (encodedDkey.Length == 4)
            {
                for (int i = 0; i < encodedDkey.Length; ++i)
                {
                    RichTextBox rtb = (RichTextBox)this.FindName($"DiffusionState{i + 8}");
                    Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(rtb, encodedDkey[i], encodedPKey[i]);
                    TextBox tbXOR = (TextBox)this.FindName($"XORState{i + 8}");
                    tbXOR.Text = encodedXor[i];
                }
            }
        }

        /// <summary>
        /// Initialize the diffusion counter values in the state matrix.
        /// </summary>
        private void InitDiffusionStateMatrixCounter()
        {
            Version v = ViewModel.Settings.Version;

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
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionState12, encodedDCounter[0], encodedPCounter[0]);
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionState13, encodedDCounter[1], encodedPCounter[1]);
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
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionState12, encodedDCounter[0], encodedPCounter[0]);
            }
        }

        /// <summary>
        /// Initialize the diffusion IV values in the state matrix.
        /// </summary>
        private void InitDiffusionStateMatrixIV()
        {
            Version v = ViewModel.Settings.Version;
            string dIVHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.DiffusionInputIV)), 8);
            string pIVHexChunksLE = Formatter.Chunkify(Formatter.HexString(Formatter.LittleEndian(ViewModel.ChaCha.InputIV)), 8);
            string[] encodedDIV = Regex.Replace(dIVHexChunksLE, @" $", "").Split(' ');
            string[] encodedPIV = Regex.Replace(pIVHexChunksLE, @" $", "").Split(' ');
            Debug.Assert(encodedDIV.Length == encodedPIV.Length, "iv and diffusion iv length should be the same.");
            if (v.CounterBits == 64)
            {
                Debug.Assert(encodedDIV.Length == 2, $"Encoded diffusion iv length should be 8 bytes for 64-bit counter. Is {encodedDIV.Length}");
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionState14, encodedDIV[0], encodedPIV[0]);
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionState15, encodedDIV[1], encodedPIV[1]);
            }
            else
            {
                Debug.Assert(encodedDIV.Length == 3, $"Encoded diffusion iv length should be 12 bytes for 64-bit counter. Is {encodedDIV.Length}");
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionState13, encodedDIV[0], encodedPIV[0]);
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionState14, encodedDIV[1], encodedPIV[1]);
                Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionState15, encodedDIV[2], encodedPIV[2]);
            }
        }
    }
}