using Cryptool.Plugins.ChaCha;
using Cryptool.Plugins.ChaChaVisualizationV2.Helper;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    internal class ChaChaPresentationViewModel : ViewModelBase, IChaCha
    {
        public ChaChaPresentationViewModel(ChaChaVisualizationV2 chachaVisualization)
        {
            ChaChaVisualization = chachaVisualization;
            ChaChaVisualization.PropertyChanged += new PropertyChangedEventHandler(NavigationEnabledChanged);
            // Add available pages
            Pages.Add(new StartViewModel());
            Pages.Add(new OverviewViewModel());
            Pages.Add(new DiffusionViewModel(chachaVisualization));
            Pages.Add(new StateMatrixInitViewModel(chachaVisualization));

            // Set starting page
            CurrentPage = Pages[0];
        }

        #region Properties / Commands

        private ICommand _changePageCommand; public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(
                        p => ChangePage((INavigation)p),
                        p => p is INavigation);
                }

                return _changePageCommand;
            }
        }

        private List<INavigation> _pages; public List<INavigation> Pages
        {
            get
            {
                if (_pages == null)
                    _pages = new List<INavigation>();

                return _pages;
            }
        }

        private INavigation _currentPage; public INavigation CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                }
            }
        }

        private void NavigationEnabledChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("NavigationEnabled");
        }

        public bool NavigationEnabled
        {
            get => ChaChaVisualization.ExecutionFinished && ChaChaVisualization.IsValid;
        }

        #endregion Properties / Commands

        #region Methods

        private void ChangePage(INavigation viewModel)
        {
            CurrentPage.Teardown();
            if (!Pages.Contains(viewModel))
                Pages.Add(viewModel);

            CurrentPage = Pages
                .FirstOrDefault(vm => vm == viewModel);
            CurrentPage.Setup();
        }

        #endregion Methods

        #region IChaCha

        public ChaChaVisualizationV2 ChaChaVisualization { get; private set; }
        public ChaCha.ChaCha ChaCha { get => ChaChaVisualization; }
        public ChaChaSettings Settings { get => (ChaChaSettings)ChaChaVisualization.Settings; }

        #endregion IChaCha
    }
}