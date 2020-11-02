using Cryptool.Plugins.ChaCha;
using Cryptool.Plugins.ChaChaVisualizationV2.Helper;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    internal class DiffusionViewModel : ViewModelBase, INavigation, ITitle, IChaCha
    {
        public DiffusionViewModel(ChaChaVisualizationV2 chachaVisualization)
        {
            ChaChaVisualization = chachaVisualization;
            ChaChaVisualization.PropertyChanged += new PropertyChangedEventHandler(PluginInputChanged);
            Name = "Diffusion";
            Title = "Diffusion";
        }

        private void PluginInputChanged(object sender, PropertyChangedEventArgs e)
        {
            DiffusionInputKey = ChaChaVisualization.InputKey;
            DiffusionInputIV = ChaChaVisualization.InputIV;
            DiffusionInitialCounter = ChaChaVisualization.InitialCounter;
        }

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
            get => ByteUtil.FlippedBits(DiffusionInputKey, ChaChaVisualization.InputKey) + ByteUtil.FlippedBits(DiffusionInputIV, ChaChaVisualization.InputIV) + ByteUtil.FlippedBits((ulong)DiffusionInitialCounter, (ulong)ChaChaVisualization.InitialCounter);
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
            get => !(DiffusionInputKey.SequenceEqual(ChaChaVisualization.InputKey) && DiffusionInputIV.SequenceEqual(ChaChaVisualization.InputIV) && DiffusionInitialCounter == ChaChaVisualization.InitialCounter);
        }

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

        public ChaChaVisualizationV2 ChaChaVisualization { get; private set; }
        public ChaCha.ChaCha ChaCha { get => ChaChaVisualization; }
        public ChaCha.ChaChaSettings Settings { get => (ChaChaSettings)ChaChaVisualization.Settings; }

        #endregion IChaCha
    }
}