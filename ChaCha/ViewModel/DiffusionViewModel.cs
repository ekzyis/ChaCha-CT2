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
            DiffusionInputKey = ChaCha.InputKey;
            DiffusionInputIV = ChaCha.InputIV;
            DiffusionInitialCounter = ChaCha.InitialCounter;
        }

        #region Binding Properties

        private byte[] _diffusionKey; public byte[] DiffusionInputKey
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
                    OnPropertyChanged();
                    OnPropertyChanged("DiffusionActive");
                    OnPropertyChanged("FlippedBits");
                    OnPropertyChanged("FlippedBitsPercentage");
                }
            }
        }

        private byte[] _diffusionInputIV; public byte[] DiffusionInputIV
        {
            get
            {
                return _diffusionInputIV;
            }
            set
            {
                if (_diffusionInputIV != value)
                {
                    _diffusionInputIV = value;
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
            get => BitFlips.FlippedBits(DiffusionInputKey, ChaCha.InputKey) + BitFlips.FlippedBits(DiffusionInputIV, ChaCha.InputIV) + BitFlips.FlippedBits((ulong)DiffusionInitialCounter, (ulong)ChaCha.InitialCounter);
        }

        public int TotalBits
        {
            get => DiffusionInputKey.Length * 8 + DiffusionInputIV.Length * 8 + (int)Settings.Version.CounterBits;
        }

        public double FlippedBitsPercentage
        {
            get => (double)FlippedBits / TotalBits;
        }

        public bool DiffusionActive
        {
            get => !(DiffusionInputKey.SequenceEqual(ChaCha.InputKey) && DiffusionInputIV.SequenceEqual(ChaCha.InputIV) && DiffusionInitialCounter == ChaCha.InitialCounter);
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
                ChaCha.ExecuteDiffusion(DiffusionInputKey, DiffusionInputIV, (ulong)DiffusionInitialCounter);
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