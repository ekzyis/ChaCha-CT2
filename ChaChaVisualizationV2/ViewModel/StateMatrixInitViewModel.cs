using Cryptool.Plugins.ChaCha;
using System.Collections.ObjectModel;
using System.Numerics;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    internal class StateMatrixInitViewModel : ActionViewModelBase, INavigation, ITitle, IChaCha
    {
        public StateMatrixInitViewModel(ChaChaPresentationViewModel chachaPresentationViewModel)
        {
            PresentationViewModel = chachaPresentationViewModel;
            Name = "State Matrix";
            Title = "State Matrix Initialization";
            InitDescriptions();
            InitActions();
        }

        private void ClearDescriptions()
        {
            Description.Clear();
            InitDescriptions();
        }

        private void InitDescriptions()
        {
            Description.Add(true);
            Description.Add(false);
            Description.Add(false);
            Description.Add(false);
        }

        private void ClearState()
        {
            // Hide state values.
            ConstantsMatrix = false;
        }

        private void ClearEncoding()
        {
            ConstantsEncoding = false;
            ConstantsEncodingInput = false;
            ConstantsEncodingASCII = false;
            ConstantsEncodingChunkify = false;
            ConstantsEncodingLittleEndian = false;
        }

        private void InitActions()
        {
            Actions.Add(() =>
            {
                Description[1] = true;
                ConstantsEncoding = true;
            });
            Actions.Add(() =>
            {
                Description[1] = true;
                ConstantsEncoding = true;
                ConstantsEncodingInput = true;
            });
            Actions.Add(() =>
            {
                Description[1] = true;
                ConstantsEncoding = true;
                ConstantsEncodingInput = true;
                ConstantsEncodingASCII = true;
            });
            Actions.Add(() =>
            {
                Description[1] = true;
                ConstantsEncoding = true;
                ConstantsEncodingInput = true;
                ConstantsEncodingASCII = true;
                ConstantsEncodingChunkify = true;
            });
            Actions.Add(() =>
            {
                Description[1] = true;
                ConstantsEncoding = true;
                ConstantsEncodingInput = true;
                ConstantsEncodingASCII = true;
                ConstantsEncodingChunkify = true;
                ConstantsEncodingLittleEndian = true;
            });
            Actions.Add(() =>
            {
                Description[1] = true;
                ConstantsEncoding = true;
                ConstantsEncodingInput = true;
                ConstantsEncodingASCII = true;
                ConstantsEncodingChunkify = true;
                ConstantsEncodingLittleEndian = true;
                ConstantsMatrix = true;
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

        #region Binding Properties

        private ObservableCollection<uint> _stateMatrixValues; public ObservableCollection<uint> StateMatrixValues
        {
            get
            {
                if (_stateMatrixValues == null) _stateMatrixValues = new ObservableCollection<uint>();
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

        #endregion Binding Properties

        #region Binding Properties (Diffusion)

        public byte[] DiffusionInputKey
        {
            get => PresentationViewModel.DiffusionInputKey;
        }

        public byte[] DiffusionInputIV
        {
            get => PresentationViewModel.DiffusionInputIV;
        }

        public BigInteger DiffusionInitialCounter
        {
            get => PresentationViewModel.DiffusionInitialCounter;
        }

        public bool DiffusionActive
        {
            get => PresentationViewModel.DiffusionActive;
        }

        #endregion Binding Properties (Diffusion)

        #region Binding Properties (Constants)

        public string ASCIIConstants
        {
            get => ChaCha.InputKey.Length == 16 ? "expand 16-byte k" : "expand 32-byte k";
        }

        private bool _constantsEncoding; public bool ConstantsEncoding
        {
            get
            {
                return _constantsEncoding;
            }
            set
            {
                _constantsEncoding = value;
                OnPropertyChanged();
            }
        }

        private bool _constantsEncodingInput; public bool ConstantsEncodingInput
        {
            get
            {
                return _constantsEncodingInput;
            }
            set
            {
                _constantsEncodingInput = value;
                OnPropertyChanged();
            }
        }

        private bool _constantsEncodingASCII; public bool ConstantsEncodingASCII
        {
            get
            {
                return _constantsEncodingASCII;
            }
            set
            {
                _constantsEncodingASCII = value;
                OnPropertyChanged();
            }
        }

        private bool _constantsEncodingChunkify; public bool ConstantsEncodingChunkify
        {
            get
            {
                return _constantsEncodingChunkify;
            }
            set
            {
                _constantsEncodingChunkify = value;
                OnPropertyChanged();
            }
        }

        public bool _constantsEncodingLittleEndian; public bool ConstantsEncodingLittleEndian
        {
            get
            {
                return _constantsEncodingLittleEndian;
            }
            set
            {
                _constantsEncodingLittleEndian = value;
                OnPropertyChanged();
            }
        }

        private bool _constantsMatrix; public bool ConstantsMatrix
        {
            get
            {
                return _constantsMatrix;
            }
            set
            {
                _constantsMatrix = value;
                OnPropertyChanged();
            }
        }

        #endregion Binding Properties (Constants)

        #region Binding Properties (Key)

        private bool _keyMatrix; public bool KeyMatrix
        {
            get
            {
                return _keyMatrix;
            }
            set
            {
                _keyMatrix = value;
                OnPropertyChanged();
            }
        }

        #endregion Binding Properties (Key)

        #region Binding Properties (Counter)

        private bool _counterMatrix; public bool CounterMatrix
        {
            get
            {
                return _counterMatrix;
            }
            set
            {
                _counterMatrix = value;
                OnPropertyChanged();
            }
        }

        private bool _state13Matrix; public bool State13Matrix
        {
            get
            {
                return _state13Matrix;
            }
            set
            {
                _state13Matrix = value;
                OnPropertyChanged();
            }
        }

        #endregion Binding Properties (Counter)

        #region Binding Properties (IV)

        private bool _ivMatrix; public bool IVMatrix
        {
            get
            {
                return _ivMatrix;
            }
            set
            {
                _ivMatrix = value;
                OnPropertyChanged();
            }
        }

        #endregion Binding Properties (IV)

        #region Action Navigation

        public override void Reset()
        {
            ClearDescriptions();
            ClearEncoding();
            ClearState();
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

        public void Setup()
        {
            uint[] state = ChaChaVisualization.OriginalState[0];
            StateMatrixValues.Clear();
            for (int i = 0; i < state.Length; ++i)
            {
                StateMatrixValues.Add(state[i]);
            }
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
        public ChaChaVisualizationV2 ChaChaVisualization { get => PresentationViewModel.ChaChaVisualization; }
        public ChaCha.ChaCha ChaCha { get => ChaChaVisualization; }
        public ChaCha.ChaChaSettings Settings { get => (ChaChaSettings)ChaChaVisualization.Settings; }

        #endregion IChaCha
    }
}