namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    internal abstract class TitledPageViewModel : ViewModelBase
    {
        /// <summary>
        /// Base class for all pages with a title.
        /// </summary>
        /// <param name="name"></param>
        public TitledPageViewModel(string name) : base(name)
        {
            this.Title = name;
        }

        public TitledPageViewModel(string name, string title) : base(name)
        {
            this.Title = title;
        }

        private string _title; public string Title
        {
            get
            {
                return _title;
            }
            private set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }
}