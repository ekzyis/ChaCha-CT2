using Cryptool.Plugins.ChaCha;
using System.ComponentModel;
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
                }
            }
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