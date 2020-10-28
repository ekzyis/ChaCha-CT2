using Cryptool.Plugins.ChaChaVisualizationV2.Model;
using System;
using System.Collections.ObjectModel;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    internal class StateMatrixInitViewModel : ViewModelBase, INavigation, ITitle
    {
        public StateMatrixInitViewModel()
        {
            Name = "State Matrix";
            Title = "State Matrix Initialization";
            var rnd = new Random();
            for (int i = 0; i < 16; ++i)
            {
                int row = i / 4;
                int col = i % 4;
                uint value = (uint)rnd.Next();
                StateMatrixValues.Add(new StateMatrixValue(value, row, col));
            }
        }

        private ObservableCollection<IGrid<uint>> _stateMatrixValues; public ObservableCollection<IGrid<uint>> StateMatrixValues
        {
            get
            {
                if (_stateMatrixValues == null) _stateMatrixValues = new ObservableCollection<IGrid<uint>>();
                return _stateMatrixValues;
            }
            private set
            {
                if (_stateMatrixValues != value)
                {
                    _stateMatrixValues = value;
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
    }
}