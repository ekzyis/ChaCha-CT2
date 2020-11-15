using Cryptool.Plugins.ChaCha.Helper;
using Cryptool.Plugins.ChaCha.ViewModel.Components;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Cryptool.Plugins.ChaCha.ViewModel
{
    internal class DiffusionViewModel : ViewModelBase, INavigation, ITitle, IChaCha
    {
        public DiffusionViewModel(ChaChaPresentationViewModel chachaPresentationViewModel)
        {
            PresentationViewModel = chachaPresentationViewModel;
            ChaCha.PropertyChanged += new PropertyChangedEventHandler(PluginInputChanged);
            Name = "Diffusion";
            Title = "Diffusion";
        }

        private void PluginInputChanged(object sender, PropertyChangedEventArgs e)
        {
            DiffusionKey = ChaCha.InputKey;
            DiffusionIV = ChaCha.InputIV;
            DiffusionInitialCounter = ChaCha.InitialCounter;
        }

        #region Binding Properties

        /// <summary>
        /// The value which is shown in the diffusion key input box.
        /// </summary>
        public byte[] _diffusionInputKey; public byte[] DiffusionInputKey

        {
            get
            {
                return _diffusionInputKey;
            }
            set
            {
                if (_diffusionInputKey != value)
                {
                    _diffusionInputKey = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The value which is shown in the diffusion XOR key input box.
        /// </summary>
        public byte[] _diffusionXORKey; public byte[] DiffusionXORKey

        {
            get
            {
                return _diffusionXORKey;
            }
            set
            {
                if (_diffusionXORKey != value)
                {
                    _diffusionXORKey = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The actual diffusion key which will be used for cipher execution.
        /// </summary>
        private byte[] _diffusionKey; public byte[] DiffusionKey

        {
            get
            {
                return _diffusionKey;
            }
            set
            {
                if (_diffusionKey != value)
                {
                    _diffusionKey = value;
                    DiffusionInputKey = value;
                    DiffusionXORKey = ByteUtil.XOR(_diffusionKey, ChaCha.InputKey);
                    OnPropertyChanged();
                    OnPropertyChanged("DiffusionActive");
                    OnPropertyChanged("FlippedBits");
                    OnPropertyChanged("FlippedBitsPercentage");
                }
            }
        }

        private byte[] _diffusionIV; public byte[] DiffusionIV
        {
            get
            {
                return _diffusionIV;
            }
            set
            {
                if (_diffusionIV != value)
                {
                    _diffusionIV = value;
                    OnPropertyChanged();
                    OnPropertyChanged("DiffusionActive");
                    OnPropertyChanged("FlippedBits");
                    OnPropertyChanged("FlippedBitsPercentage");
                }
            }
        }

        private BigInteger _diffusionInitialCounter; public BigInteger DiffusionInitialCounter
        {
            get
            {
                return _diffusionInitialCounter;
            }
            set
            {
                if (_diffusionInitialCounter != value)
                {
                    _diffusionInitialCounter = value;
                    OnPropertyChanged();
                    OnPropertyChanged("DiffusionActive");
                    OnPropertyChanged("FlippedBits");
                    OnPropertyChanged("FlippedBitsPercentage");
                }
            }
        }

        public int FlippedBits
        {
            get => BitFlips.FlippedBits(DiffusionKey, ChaCha.InputKey) + BitFlips.FlippedBits(DiffusionIV, ChaCha.InputIV) + BitFlips.FlippedBits((ulong)DiffusionInitialCounter, (ulong)ChaCha.InitialCounter);
        }

        public int TotalBits
        {
            get => DiffusionKey.Length * 8 + DiffusionIV.Length * 8 + (int)Settings.Version.CounterBits;
        }

        public double FlippedBitsPercentage
        {
            get => (double)FlippedBits / TotalBits;
        }

        public bool DiffusionActive
        {
            get => !(DiffusionKey.SequenceEqual(ChaCha.InputKey) && DiffusionIV.SequenceEqual(ChaCha.InputIV) && DiffusionInitialCounter == ChaCha.InitialCounter);
        }

        #endregion Binding Properties

        #region INavigation

        private string _name; public string Name
        {
            get
            {
                if (_name == null) _name = "";
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Setup()
        {
        }

        public void Teardown()
        {
            if (DiffusionActive)
            {
                // Execute ChaCha with Diffusion values.
                ChaCha.ExecuteDiffusion(DiffusionKey, DiffusionIV, (ulong)DiffusionInitialCounter);
            }
        }

        #endregion INavigation

        #region ITitle

        private string _title; public string Title
        {
            get
            {
                if (_title == null) _title = "";
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion ITitle

        #region IChaCha

        public ChaChaPresentationViewModel PresentationViewModel { get; private set; }
        public ChaCha ChaCha { get => PresentationViewModel.ChaCha; }
        public ChaChaSettings Settings { get => (ChaChaSettings)ChaCha.Settings; }

        #endregion IChaCha
    }
}