using Cryptool.Plugins.ChaCha;
using Cryptool.Plugins.ChaChaVisualizationV2.Model;
using System.Collections.ObjectModel;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    internal class StateMatrixInitViewModel : ActionViewModelBase, INavigation, ITitle, IChaCha
    {
        public StateMatrixInitViewModel(ChaCha.ChaCha chacha, ChaChaSettings settings)
        {
            ChaCha = chacha;
            Settings = settings;
            Name = "State Matrix";
            Title = "State Matrix Initialization";
            InitDescriptions();
            InitActions();
        }

        private void InitDescriptions()
        {
            Description.Add(true);
            Description.Add(false);
            Description.Add(false);
            Description.Add(false);
        }

        private void InitActions()
        {
            Actions.Add(() =>
            {
                Description[1] = true;
            });
            Actions.Add(() =>
            {
                Description[1] = true;
                Description[2] = true;
            });
            Actions.Add(() =>
            {
                Description[1] = true;
                Description[2] = true;
                Description[3] = true;
            });
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

        private ObservableCollection<bool> _description; public ObservableCollection<bool> Description
        {
            get
            {
                if (_description == null) _description = new ObservableCollection<bool>();
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        #region Action Navigation

        public override void Reset()
        {
            Description.Clear();
            InitDescriptions();
            StateMatrixValues.Clear();
        }

        #endregion Action Navigation

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