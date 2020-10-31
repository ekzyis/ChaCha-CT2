using Cryptool.Plugins.ChaCha;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    internal class DiffusionViewModel : ViewModelBase, INavigation, ITitle, IChaCha
    {
        public DiffusionViewModel(ChaCha.ChaCha chacha, ChaChaSettings settings)
        {
            ChaCha = chacha;
            Settings = settings;
            Name = "Diffusion";
            Title = "Diffusion";
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

        public ChaCha.ChaCha ChaCha { get; private set; }
        public ChaCha.ChaChaSettings Settings { get; private set; }

        #endregion IChaCha
    }
}