﻿namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    internal class StartViewModel : ViewModelBase, INavigation
    {
        public StartViewModel()
        {
            Name = "Start";
        }

        private string _name;

        public string Name
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
    }
}