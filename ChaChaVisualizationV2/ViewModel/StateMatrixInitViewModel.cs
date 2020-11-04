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
            InitDescriptionVisibilityFlags();
            InitActions();
        }

        private void HideDescriptions()
        {
            Description.Clear();
            InitDescriptionVisibilityFlags();
        }

        private void InitDescriptionVisibilityFlags()
        {
            Description.Add(true);
            Description.Add(false);
            Description.Add(false);
            Description.Add(false);
            Description.Add(false);
            Description.Add(false);
        }

        private void HideState()
        {
            // Hide state values.
            ConstantsMatrix = false;
            KeyMatrix = false;
            CounterMatrix = false;
            State13Matrix = false;
            IVMatrix = false;
        }

        private void HideEncoding()
        {
            ConstantsEncoding = false;
            ConstantsEncodingInput = false;
            ConstantsEncodingASCII = false;
            ConstantsEncodingChunkify = false;
            ConstantsEncodingLittleEndian = false;

            KeyEncoding = false;
            KeyEncodingInput = false;
            KeyEncodingChunkify = false;
            KeyEncodingLittleEndian = false;

            CounterEncoding = false;
            CounterEncodingInput = false;
            CounterEncodingReverse = false;
            CounterEncodingChunkify = false;
            CounterEncodingLittleEndian = false;

            IVEncoding = false;
            IVEncodingInput = false;
            IVEncodingChunkify = false;
            IVEncodingLittleEndian = false;
        }

        private void InitActions()
        {
            #region Constants

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

            #endregion Constants

            #region Key

            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                Description[1] = true;
                Description[2] = true;
                KeyEncoding = true;
            });
            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                Description[1] = true;
                Description[2] = true;
                KeyEncoding = true;
                KeyEncodingInput = true;
            });
            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                Description[1] = true;
                Description[2] = true;
                KeyEncoding = true;
                KeyEncodingInput = true;
                KeyEncodingChunkify = true;
            });
            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                Description[1] = true;
                Description[2] = true;
                KeyEncoding = true;
                KeyEncodingInput = true;
                KeyEncodingChunkify = true;
                KeyEncodingLittleEndian = true;
            });
            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                Description[1] = true;
                Description[2] = true;
                KeyEncoding = true;
                KeyEncodingInput = true;
                KeyEncodingChunkify = true;
                KeyEncodingLittleEndian = true;
                KeyMatrix = true;
            });

            #endregion Key

            #region Counter

            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                KeyMatrix = true;
                Description[1] = true;
                Description[2] = true;
                Description[3] = true;
                CounterEncoding = true;
            });
            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                KeyMatrix = true;
                Description[1] = true;
                Description[2] = true;
                Description[3] = true;
                CounterEncoding = true;
                CounterEncodingInput = true;
            });
            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                KeyMatrix = true;
                Description[1] = true;
                Description[2] = true;
                Description[3] = true;
                CounterEncoding = true;
                CounterEncodingInput = true;
                CounterEncodingReverse = true;
            });
            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                KeyMatrix = true;
                Description[1] = true;
                Description[2] = true;
                Description[3] = true;
                CounterEncoding = true;
                CounterEncodingInput = true;
                CounterEncodingReverse = true;
                CounterEncodingChunkify = true;
            });
            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                KeyMatrix = true;
                Description[1] = true;
                Description[2] = true;
                Description[3] = true;
                CounterEncoding = true;
                CounterEncodingInput = true;
                CounterEncodingReverse = true;
                CounterEncodingChunkify = true;
                CounterEncodingLittleEndian = true;
            });
            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                KeyMatrix = true;
                Description[1] = true;
                Description[2] = true;
                Description[3] = true;
                CounterEncoding = true;
                CounterEncodingInput = true;
                CounterEncodingReverse = true;
                CounterEncodingChunkify = true;
                CounterEncodingLittleEndian = true;
                CounterMatrix = true;
                if (Settings.Version.CounterBits == 64) State13Matrix = true;
            });

            #endregion Counter

            #region IV

            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                KeyMatrix = true;
                CounterMatrix = true;
                if (Settings.Version.CounterBits == 64) State13Matrix = true;
                Description[1] = true;
                Description[2] = true;
                Description[3] = true;
                Description[4] = true;
                IVEncoding = true;
            });
            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                KeyMatrix = true;
                CounterMatrix = true;
                if (Settings.Version.CounterBits == 64) State13Matrix = true;
                Description[1] = true;
                Description[2] = true;
                Description[3] = true;
                Description[4] = true;
                IVEncoding = true;
                IVEncodingInput = true;
            });
            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                KeyMatrix = true;
                CounterMatrix = true;
                if (Settings.Version.CounterBits == 64) State13Matrix = true;
                Description[1] = true;
                Description[2] = true;
                Description[3] = true;
                Description[4] = true;
                IVEncoding = true;
                IVEncodingInput = true;
                IVEncodingChunkify = true;
            });
            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                KeyMatrix = true;
                CounterMatrix = true;
                if (Settings.Version.CounterBits == 64) State13Matrix = true;
                Description[1] = true;
                Description[2] = true;
                Description[3] = true;
                Description[4] = true;
                IVEncoding = true;
                IVEncodingInput = true;
                IVEncodingChunkify = true;
                IVEncodingLittleEndian = true;
            });
            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                KeyMatrix = true;
                CounterMatrix = true;
                if (Settings.Version.CounterBits == 64) State13Matrix = true;
                Description[1] = true;
                Description[2] = true;
                Description[3] = true;
                Description[4] = true;
                IVEncoding = true;
                IVEncodingInput = true;
                IVEncodingChunkify = true;
                IVEncodingLittleEndian = true;
                IVMatrix = true;
            });

            #endregion IV

            Actions.Add(() =>
            {
                ConstantsMatrix = true;
                KeyMatrix = true;
                CounterMatrix = true;
                if (Settings.Version.CounterBits == 64) State13Matrix = true;
                IVMatrix = true;
                Description[1] = true;
                Description[2] = true;
                Description[3] = true;
                Description[4] = true;
                Description[5] = true;
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

        private bool _keyEncoding; public bool KeyEncoding
        {
            get
            {
                return _keyEncoding;
            }
            set
            {
                _keyEncoding = value;
                OnPropertyChanged();
            }
        }

        private bool _keyEncodingInput; public bool KeyEncodingInput
        {
            get
            {
                return _keyEncodingInput;
            }
            set
            {
                _keyEncodingInput = value;
                OnPropertyChanged();
            }
        }

        private bool _keyEncodingChunkify; public bool KeyEncodingChunkify
        {
            get
            {
                return _keyEncodingChunkify;
            }
            set
            {
                _keyEncodingChunkify = value;
                OnPropertyChanged();
            }
        }

        private bool _keyEncodingLittleEndian; public bool KeyEncodingLittleEndian
        {
            get
            {
                return _keyEncodingLittleEndian;
            }
            set
            {
                _keyEncodingLittleEndian = value;
                OnPropertyChanged();
            }
        }

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

        private bool _counterEncoding; public bool CounterEncoding
        {
            get
            {
                return _counterEncoding;
            }
            set
            {
                _counterEncoding = value;
                OnPropertyChanged();
            }
        }

        private bool _counterEncodingInput; public bool CounterEncodingInput
        {
            get
            {
                return _counterEncodingInput;
            }
            set
            {
                _counterEncodingInput = value;
                OnPropertyChanged();
            }
        }

        private bool _counterEncodingReverse; public bool CounterEncodingReverse
        {
            get
            {
                return _counterEncodingReverse;
            }
            set
            {
                _counterEncodingReverse = value;
                OnPropertyChanged();
            }
        }

        private bool _counterEncodingChunkify; public bool CounterEncodingChunkify
        {
            get
            {
                return _counterEncodingChunkify;
            }
            set
            {
                _counterEncodingChunkify = value;
                OnPropertyChanged();
            }
        }

        private bool _counterEncodingLittleEndian; public bool CounterEncodingLittleEndian
        {
            get
            {
                return _counterEncodingLittleEndian;
            }
            set
            {
                _counterEncodingLittleEndian = value;
                OnPropertyChanged();
            }
        }

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

        private bool _ivEncoding; public bool IVEncoding
        {
            get
            {
                return _ivEncoding;
            }
            set
            {
                _ivEncoding = value;
                OnPropertyChanged();
            }
        }

        private bool _ivEncodingInput; public bool IVEncodingInput
        {
            get
            {
                return _ivEncodingInput;
            }
            set
            {
                _ivEncodingInput = value;
                OnPropertyChanged();
            }
        }

        private bool _ivEncodingChunkify; public bool IVEncodingChunkify
        {
            get
            {
                return _ivEncodingChunkify;
            }
            set
            {
                _ivEncodingChunkify = value;
                OnPropertyChanged();
            }
        }

        private bool _ivEncodingLittleEndian; public bool IVEncodingLittleEndian
        {
            get
            {
                return _ivEncodingLittleEndian;
            }
            set
            {
                _ivEncodingLittleEndian = value;
                OnPropertyChanged();
            }
        }

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
            HideDescriptions();
            InitDescriptionVisibilityFlags();
            HideEncoding();
            HideState();
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
            MoveToFirstAction();
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