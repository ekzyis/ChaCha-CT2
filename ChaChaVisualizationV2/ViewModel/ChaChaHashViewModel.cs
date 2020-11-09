using Cryptool.Plugins.ChaChaVisualizationV2.Model;
using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel.Components;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    internal class ChaChaHashViewModel : ActionViewModelBase, INavigation, ITitle, IChaCha
    {
        /// <summary>
        /// Convenience list to write cleaner code which modifies all input values.
        /// </summary>
        private readonly QRValue[] qrInValues;

        /// <summary>
        /// Convenience list to write cleaner code which modifies all output values.
        /// </summary>
        private readonly QRValue[] qrOutValues;

        public ChaChaHashViewModel(ChaChaPresentationViewModel chachaPresentationViewModel) : base(chachaPresentationViewModel)
        {
            Name = "ChaCha hash";
            Title = "ChaCha hash function";
            qrInValues = new QRValue[] { QRInA, QRInB, QRInC, QRInD };
            qrOutValues = new QRValue[] { QROutA, QROutB, QROutC, QROutD };
        }

        #region ActionViewModelBase

        protected override void InitActions()
        {
            QRIOActionCreator qrIO = new QRIOActionCreator(this);

            QRAdditionActionCreator qrAdd = new QRAdditionActionCreator(this);
            QRXORActionCreator qrXOR = new QRXORActionCreator(this);
            QRShiftActionCreator qrShift = new QRShiftActionCreator(this);

            // ChaCha Hash sequence
            ActionCreator.StartSequence();

            for (int round = 0; round < Settings.Rounds; ++round)
            {
                // Column round sequence
                ActionCreator.StartSequence();
                for (int qr = 0; qr < 4; ++qr)
                {
                    //  Quarterround sequence
                    ActionCreator.StartSequence();

                    // Copy from state into qr input
                    ActionCreator.StartSequence();
                    Seq(qrIO.MarkState(round, qr));
                    Seq(qrIO.InsertQRInputs(round, qr).Extend(qrIO.MarkQRInputs));
                    ActionCreator.EndSequence();

                    // Keep inserted qr input for the rest of the qr sequence
                    Seq(qrIO.InsertQRInputs(round, qr));

                    // Run quarterround steps
                    for (int qrStep = 0; qrStep < 4; ++qrStep)
                    {
                        // Execute addition
                        ActionCreator.StartSequence();
                        Seq(qrAdd.MarkInputs(qrStep));
                        Seq(qrAdd.Insert(round, qr, qrStep).Extend(qrAdd.Mark(qrStep)));
                        ActionCreator.EndSequence();

                        // Keep addition values
                        Seq(qrAdd.Insert(round, qr, qrStep));

                        // Execute XOR
                        ActionCreator.StartSequence();
                        Seq(qrXOR.MarkInputs(qrStep));
                        Seq(qrXOR.Insert(round, qr, qrStep).Extend(qrXOR.Mark(qrStep)));
                        ActionCreator.EndSequence();

                        // Keep XOR values
                        Seq(qrXOR.Insert(round, qr, qrStep));

                        // Execute shift
                        ActionCreator.StartSequence();
                        Seq(qrShift.MarkInputs(qrStep));
                        Seq(qrShift.Insert(round, qr, qrStep).Extend(qrShift.Mark(qrStep)));
                        ActionCreator.EndSequence();

                        // Keep shift values
                        Seq(qrShift.Insert(round, qr, qrStep));
                    }

                    // Fill quarterround output
                    ActionCreator.StartSequence();
                    Seq(qrIO.MarkQROutputPaths);
                    Seq(qrIO.InsertQROutputs(round, qr).Extend(qrIO.MarkQROutputs));
                    ActionCreator.EndSequence();

                    // Keep qr output values
                    Seq(qrIO.InsertQROutputs(round, qr));

                    // Copy from qr output to state
                    ActionCreator.StartSequence();
                    Seq(qrIO.MarkQROutputs);
                    Seq(qrIO.UpdateState(round, qr).Extend(qrIO.MarkState(round, qr)));
                    ActionCreator.EndSequence();

                    // End quarterround sequence
                    ActionCreator.EndSequence();

                    // Keep state update for rest of round sequence
                    // FIXME There is a bug that the state update order is not as expected.
                    //   We need to apply previous state updates ( from the previous round ) and then the new state update.
                    //   But it for some reasons first applies the latest state and then the state updates fro last round in the correct order.
                    //   So basically like this: 4, 0, 3, 2, 1.
                    //   This leads to an overwrite of the diagonal.
                    Seq(qrIO.UpdateState(round, qr));
                }
                // End round sequence
                ActionCreator.EndSequence();

                // Keep state updates from all quarterrounds of the last round for rest of ChaCha hash sequence.
                // This replaces for performance (and bug) reasons the last sequential action in the ChaCha hash sequence
                // because the complete state will be modified in every round anyway and thus we would just "overdraw" if we apply all state updates from each round
                // in a sequence.
                ActionCreator.Replace(qrIO.UpdateState(round, 3).Extend(qrIO.UpdateState(round, 2), qrIO.UpdateState(round, 1), qrIO.UpdateState(round, 0)));
            }

            ActionCreator.EndSequence();
        }

        public override void Reset()
        {
            ResetStateMatrixValues();
            ResetQuarterroundValues();
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

        /// <summary>
        /// Clear the values and background of each cell in the quarterround visualzation.
        /// </summary>
        private void ResetQuarterroundValues()
        {
            foreach (QRValue v in qrInValues.Concat(qrOutValues))
            {
                v.Reset();
            }
            foreach (VisualQRStep qrStep in QRStep)
            {
                qrStep.Reset();
            }
        }

        #endregion ActionViewModelBase

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

        private ObservableCollection<VisualQRStep> _qrStep; public ObservableCollection<VisualQRStep> QRStep
        {
            get
            {
                if (_qrStep == null) _qrStep = new ObservableCollection<VisualQRStep>();
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
                if (_qrInA == null) _qrInA = new QRValue();
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
                if (_qrInB == null) _qrInB = new QRValue();
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
                if (_qrInC == null) _qrInC = new QRValue();
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
                if (_qrInD == null) _qrInD = new QRValue();
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
                if (_qrOutA == null) _qrOutA = new QRValue();
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
                if (_qrOutB == null) _qrOutB = new QRValue();
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
                if (_qrOutC == null) _qrOutC = new QRValue();
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
                if (_qrOutD == null) _qrOutD = new QRValue();
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

        #region INavigation

        public override void Setup()
        {
            Debug.Assert(StateValues.Count == 0, "StateValues should be empty during ChaCha hash setup.");
            uint[] state = ChaChaVisualization.OriginalState[0];
            for (int i = 0; i < state.Length; ++i)
            {
                StateValues.Add(new StateValue(state[i]));
            }
            Debug.Assert(QRStep.Count == 0, "QRStep should be empty during ChaCha hash setup.");
            // There are four steps inside a quarterround. The array indices will be reused between different (quarter)rounds.
            for (int i = 0; i < 4; ++i)
            {
                QRStep.Add(new VisualQRStep());
            }
            // First setup page, then call base setup because action buffer handler may depend on things being setup already.
            base.Setup();
        }

        public override void Teardown()
        {
            base.Teardown();
            // Clear lists to undo Setup.
            StateValues.Clear();
            QRStep.Clear();
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