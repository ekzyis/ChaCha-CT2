namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    /// <summary>
    /// ViewModel for pages with a title.
    /// </summary>
    internal class TitlePageViewModel : ViewModelBase
    {
        private string _title; public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }
}