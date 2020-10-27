using Cryptool.Plugins.ChaChaVisualizationV2.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    internal class ChaChaPresentationViewModel : ViewModelBase
    {
        public ChaChaPresentationViewModel() : base("ChaChaPresentation")
        {
            // Add available pages
            Pages.Add(new StartViewModel());
            Pages.Add(new OverviewViewModel());

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
                        p => ChangePage((TitledPageViewModel)p),
                        p => p is ViewModelBase);
                }

                return _changePageCommand;
            }
        }

        private List<TitledPageViewModel> _pages; public List<TitledPageViewModel> Pages
        {
            get
            {
                if (_pages == null)
                    _pages = new List<TitledPageViewModel>();

                return _pages;
            }
        }

        private TitledPageViewModel _currentPage; public TitledPageViewModel CurrentPage
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

        #endregion Properties / Commands

        #region Methods

        private void ChangePage(TitledPageViewModel viewModel)
        {
            if (!Pages.Contains(viewModel))
                Pages.Add(viewModel);

            CurrentPage = Pages
                .FirstOrDefault(vm => vm == viewModel);
        }

        #endregion Methods
    }
}