using Cryptool.Plugins.ChaCha.ViewModel.Components;

namespace Cryptool.Plugins.ChaCha.ViewModel
{
    internal class OverviewViewModel : ViewModelBase, INavigation, ITitle, IChaCha
    {
        public OverviewViewModel(ChaChaPresentationViewModel chachaPresentationViewModel)
        {
            PresentationViewModel = chachaPresentationViewModel;
            Name = "Overview";
            Title = "Overview";
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

        public void Setup()
        {
        }

        public void Teardown()
        {
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