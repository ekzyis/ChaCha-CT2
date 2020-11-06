using Cryptool.Plugins.ChaCha;
using Cryptool.Plugins.ChaChaVisualizationV2.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Numerics;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    internal class ChaChaHashViewModel : ActionViewModelBase, INavigation, ITitle, IChaCha
    {
        public ChaChaHashViewModel(ChaChaPresentationViewModel chachaPresentationViewModel)
        {
            PresentationViewModel = chachaPresentationViewModel;
            Name = "ChaCha hash";
            Title = "ChaCha hash function";
            InitActions();
        }

        /// <summary>
        /// Replace the state with the state before the ChaCha hash function was applied.
        /// </summary>
        private void ResetStateMatrixValues()
        {
            uint[] state = ChaChaVisualization.OriginalState[0];
            for (int i = 0; i < 16; ++i)
            {
                StateValues[i].Value = state[i];
                StateValues[i].Mark = false;
            }
        }

        private void InitActions()
        {
            Actions.Add(() =>
            {
                StateValues[0].Mark = true;
                StateValues[4].Mark = true;
                StateValues[8].Mark = true;
                StateValues[12].Mark = true;
            });
        }

        #region Binding Properties

        private ObservableCollection<StateValue> _stateValues; public ObservableCollection<StateValue> StateValues
        {
            get
            {
                if (_stateValues == null) _stateValues = new ObservableCollection<StateValue>();
                return _stateValues;
            }
            private set
            {
                if (_stateValues != value)
                {
                    _stateValues = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<QRStep> _qrStep; public ObservableCollection<QRStep> QRStep
        {
            get
            {
                if (_qrStep == null) _qrStep = new ObservableCollection<QRStep>();
                return _qrStep;
            }
            private set
            {
                if (_qrStep != value)
                {
                    _qrStep = value;
                    OnPropertyChanged();
                }
            }
        }

        #region QRInX

        private QRValue _qrInA; public QRValue QRInA
        {
            get
            {
                return _qrInA;
            }
            set
            {
                _qrInA = value;
                OnPropertyChanged();
            }
        }

        private QRValue _qrInB; public QRValue QRInB
        {
            get
            {
                return _qrInB;
            }
            set
            {
                _qrInB = value;
                OnPropertyChanged();
            }
        }

        private QRValue _qrInC; public QRValue QRInC
        {
            get
            {
                return _qrInC;
            }
            set
            {
                _qrInC = value;
                OnPropertyChanged();
            }
        }

        private QRValue _qrInD; public QRValue QRInD
        {
            get
            {
                return _qrInD;
            }
            set
            {
                _qrInD = value;
                OnPropertyChanged();
            }
        }

        #endregion QRInX

        #region QROutX

        private QRValue _qrOutA; public QRValue QROutA
        {
            get
            {
                return _qrOutA;
            }
            set
            {
                _qrOutA = value;
                OnPropertyChanged();
            }
        }

        private QRValue _qrOutB; public QRValue QROutB
        {
            get
            {
                return _qrOutB;
            }
            set
            {
                _qrOutB = value;
                OnPropertyChanged();
            }
        }

        private QRValue _qrOutC; public QRValue QROutC
        {
            get
            {
                return _qrOutC;
            }
            set
            {
                _qrOutC = value;
                OnPropertyChanged();
            }
        }

        private QRValue _qrOutD; public QRValue QROutD
        {
            get
            {
                return _qrOutD;
            }
            set
            {
                _qrOutD = value;
                OnPropertyChanged();
            }
        }

        #endregion QROutX

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

        #region Action Navigation

        public override void Reset()
        {
            ResetStateMatrixValues();
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
            Debug.Assert(StateValues.Count == 0, "StateValues should be empty during ChaCha hash setup.");
            uint[] state = ChaChaVisualization.OriginalState[0];
            for (int i = 0; i < state.Length; ++i)
            {
                StateValues.Add(new StateValue(state[i]));
            }
        }

        public void Teardown()
        {
            MoveToFirstAction();
            // Clear state to undo Setup.
            StateValues.Clear();
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