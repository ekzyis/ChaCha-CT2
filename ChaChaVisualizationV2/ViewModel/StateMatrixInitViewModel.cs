﻿namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    internal class StateMatrixInitViewModel : ViewModelBase, INavigation, ITitle
    {
        public StateMatrixInitViewModel()
        {
            Name = "State Matrix";
            Title = "State Matrix Initialization";
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

        private string _title;

        public string Title
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
    }
}