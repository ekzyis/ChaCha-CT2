using Cryptool.Plugins.ChaChaVisualizationV2.Helper;
using Cryptool.Plugins.ChaChaVisualizationV2.Helper.Validation;
using Cryptool.Plugins.ChaChaVisualizationV2.Model;
using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel.Components;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Input;

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

        /// <summary>
        /// This variable is used during `InitActions` to tag actions.
        /// It always points to the last index.
        /// </summary>
        private int ActionIndex => Actions.Count - 1;

        protected override void InitActions()
        {
            QRIOActionCreator qrIO = new QRIOActionCreator(this);

            QRAdditionActionCreator qrAdd = new QRAdditionActionCreator(this);
            QRXORActionCreator qrXOR = new QRXORActionCreator(this);
            QRShiftActionCreator qrShift = new QRShiftActionCreator(this);
            StateActionCreator stateActions = new StateActionCreator(this);
            // ChaCha Hash sequence
            ActionCreator.StartSequence();

            ExtendLastAction(() => { CurrentKeystreamBlockIndex = null; });
            ExtendLastAction(() => { RoundsStep = true; });
            for (int keystreamBlock = 0; keystreamBlock < ChaChaVisualization.TotalKeystreamBlocks; ++keystreamBlock)
            {
                // Keystream Block sequence
                ActionCreator.StartSequence();

                // The very first action which is empty was added by ActionViewModelBase.
                // Thus the first action of every next keystream block, we add a empty action ourselves.
                // We also set the the correct original state as the base extension for all following actions
                // in this action sequence.
                int localKeystreamBlock = keystreamBlock; // fix for https://stackoverflow.com/questions/271440/captured-variable-in-a-loop-in-c-sharp
                // Set base extension for this keystream block sequence.
                ActionCreator.Sequential(() =>
                {
                    InsertOriginalState(localKeystreamBlock);
                    CurrentKeystreamBlockIndex = localKeystreamBlock;
                    RoundsStep = true;
                });
                // Add empty action if this is not the very first action because ActionViewModelBase
                // has already added an empty action at the very beginning.
                // This empty action added here will be extended by the base extension we just added.
                if (keystreamBlock != 0) Seq(() => { });

                // Extend the first action which was added by the base class.
                ExtendLastAction(() => { CurrentRoundIndex = null; });
                ExtendLastAction(() => { CurrentQRIndex = null; });

                for (int round = 0; round < Settings.Rounds; ++round)
                {
                    // round sequence
                    ActionCreator.StartSequence();
                    // Set a sequence base extension for round sequence.
                    // All further actions will now extend this action.
                    int localRound = round; // fix for https://stackoverflow.com/questions/271440/captured-variable-in-a-loop-in-c-sharp
                    ActionCreator.Sequential(() => { CurrentRoundIndex = localRound; });

                    for (int qr = 0; qr < 4; ++qr)
                    {
                        //  Quarterround sequence
                        ActionCreator.StartSequence();
                        // Set a sequence base extension for quarterround sequence.
                        // All further actions will now extend this action.
                        int localQr = qr; // fix for https://stackoverflow.com/questions/271440/captured-variable-in-a-loop-in-c-sharp
                        ActionCreator.Sequential(() => { CurrentQRIndex = localQr; });

                        // Copy from state into qr input
                        ActionCreator.StartSequence();
                        Seq(qrIO.MarkState(round, qr));
                        if (qr == 0)
                        {
                            TagRoundStartAction(keystreamBlock, round);
                            if (round == 0)
                            {
                                TagKeystreamBlockStartAction(keystreamBlock);
                            }
                        }
                        TagQRStartAction(keystreamBlock, round, qr);

                        Seq(qrIO.InsertQRInputs(keystreamBlock, round, qr).Extend(qrIO.MarkQRInputs));
                        ActionCreator.EndSequence();

                        // Keep inserted qr input for the rest of the qr sequence
                        Seq(qrIO.InsertQRInputs(keystreamBlock, round, qr));

                        // Run quarterround steps
                        for (int qrStep = 0; qrStep < 4; ++qrStep)
                        {
                            // Execute addition
                            ActionCreator.StartSequence();
                            Seq(qrAdd.MarkInputs(qrStep));
                            Seq(qrAdd.Insert(keystreamBlock, round, qr, qrStep).Extend(qrAdd.Mark(qrStep)));
                            ActionCreator.EndSequence();

                            // Keep addition values
                            Seq(qrAdd.Insert(keystreamBlock, round, qr, qrStep));

                            // Execute XOR
                            ActionCreator.StartSequence();
                            Seq(qrXOR.MarkInputs(qrStep));
                            Seq(qrXOR.Insert(keystreamBlock, round, qr, qrStep).Extend(qrXOR.Mark(qrStep)));
                            ActionCreator.EndSequence();

                            // Keep XOR values
                            Seq(qrXOR.Insert(keystreamBlock, round, qr, qrStep));

                            // Execute shift
                            ActionCreator.StartSequence();
                            Seq(qrShift.MarkInputs(qrStep));
                            Seq(qrShift.Insert(keystreamBlock, round, qr, qrStep).Extend(qrShift.Mark(qrStep)));
                            ActionCreator.EndSequence();

                            // Keep shift values
                            Seq(qrShift.Insert(keystreamBlock, round, qr, qrStep));
                        }

                        // Fill quarterround output
                        ActionCreator.StartSequence();
                        Seq(qrIO.MarkQROutputPaths);
                        Seq(qrIO.InsertQROutputs(keystreamBlock, round, qr).Extend(qrIO.MarkQROutputs));
                        ActionCreator.EndSequence();

                        // Keep qr output values
                        Seq(qrIO.InsertQROutputs(keystreamBlock, round, qr));

                        // Copy from qr output to state
                        ActionCreator.StartSequence();
                        Seq(qrIO.MarkQROutputs);

                        Seq(qrIO.UpdateState(keystreamBlock, round, qr).Extend(qrIO.MarkState(round, qr)));
                        if (qr == 3) TagRoundEndStartAction(keystreamBlock, round);
                        TagQREndAction(keystreamBlock, round, qr);

                        ActionCreator.EndSequence();

                        // End quarterround sequence
                        ActionCreator.EndSequence();

                        // Keep state update for rest of round sequence
                        // FIXME There is a bug that the state update order is not as expected.
                        //   We need to apply previous state updates ( from the previous round ) and then the new state update.
                        //   But it for some reasons first applies the latest state and then the state updates fro last round in the correct order.
                        //   So basically like this: 4, 0, 3, 2, 1.
                        //   This leads to an overwrite of the diagonal.
                        Seq(qrIO.UpdateState(keystreamBlock, round, qr));
                    }
                    // End round sequence
                    ActionCreator.EndSequence();

                    // Keep state updates from all quarterrounds of the last round for rest of ChaCha hash sequence.
                    // This replaces for performance (and bug) reasons the last sequential action in the ChaCha hash sequence
                    // because the complete state will be modified in every round anyway and thus we would just "overdraw" if we apply all state updates from each round
                    // in a sequence.
                    ActionCreator.ReplaceLast(
                        qrIO.UpdateState(keystreamBlock, round, 3)
                        .Extend(qrIO.UpdateState(keystreamBlock, round, 2), qrIO.UpdateState(keystreamBlock, round, 1), qrIO.UpdateState(keystreamBlock, round, 0)));
                }

                // Addition + little-endian step
                ActionCreator.StartSequence();

                Seq(() => { RoundsStep = false; });
                Seq(() => stateActions.ShowOriginalState(localKeystreamBlock));
                Seq(() => stateActions.ShowAdditionResultState(localKeystreamBlock));
                Seq(() => stateActions.ShowLittleEndianState(localKeystreamBlock));

                ActionCreator.EndSequence();

                // End keystraem block sequence
                ActionCreator.EndSequence();
            }

            // End ChaCha Hash sequence
            ActionCreator.EndSequence();
        }

        public override void Reset()
        {
            ResetStateMatrixValues();
            ResetQuarterroundValues();
        }

        /// <summary>
        /// Replace the state with the state before the very first ChaCha hash function was applied.
        /// </summary>
        private void ResetStateMatrixValues()
        {
            InsertOriginalState(0);
        }

        /// <summary>
        /// Reset the state matrix to the state at the start of the keystream block.
        /// </summary>
        /// <param name="keystreamBlock">Zero-based keystream block index.</param>
        private void InsertOriginalState(int keystreamBlock)
        {
            Debug.Assert(ChaChaVisualization.OriginalState.Count == ChaChaVisualization.TotalKeystreamBlocks,
                $"Count of OriginalState was not equal to TotalKeystreamBlocks. Expected: {ChaChaVisualization.TotalKeystreamBlocks}. Actual: {ChaChaVisualization.OriginalState.Count}");
            uint[] state = ChaChaVisualization.OriginalState[keystreamBlock];
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

        #region ChaCha Hash Navigation Bar

        #region Keystream Block input

        private ICommand _nextKeystreamBlockCommand; public ICommand NextKeystreamBlockCommand
        {
            get
            {
                if (_nextKeystreamBlockCommand == null) _nextKeystreamBlockCommand = new RelayCommand((arg) => NextKeystreamBlock(), (arg) => CanNextKeystreamBlock);
                return _nextKeystreamBlockCommand;
            }
        }

        public bool CanNextKeystreamBlock
        {
            get
            {
                return CurrentKeystreamBlockIndex == null || CurrentKeystreamBlockIndex < ChaChaVisualization.TotalKeystreamBlocks - 1;
            }
        }

        private ICommand _prevKeystreamBlockCommand; public ICommand PrevKeystreamBlockCommand
        {
            get
            {
                if (_prevKeystreamBlockCommand == null) _prevKeystreamBlockCommand = new RelayCommand((arg) => PrevKeystreamBlock(), (arg) => CanPrevKeystreamBlock);
                return _prevKeystreamBlockCommand;
            }
        }

        public bool CanPrevKeystreamBlock
        {
            get
            {
                return CurrentKeystreamBlockIndex != null && CurrentKeystreamBlockIndex != 0;
            }
        }

        private void NextKeystreamBlock()
        {
            int nextKeystreamBlockIndex = CurrentKeystreamBlockIndex == null ? 0 : (int)CurrentKeystreamBlockIndex + 1;
            int nextKeystreamBlockActionIndex = GetTaggedActionIndex(KeystreamBlockStartTag(nextKeystreamBlockIndex));
            MoveToAction(nextKeystreamBlockActionIndex);
        }

        private void PrevKeystreamBlock()
        {
            int prevKeystreamBlockIndex = (int)CurrentKeystreamBlockIndex - 1;
            int prevKeystreamBlockActionIndex = GetTaggedActionIndex(KeystreamBlockStartTag(prevKeystreamBlockIndex));
            MoveToAction(prevKeystreamBlockActionIndex);
        }

        private ValidationRule _keystreamBlockInputRule; private ValidationRule KeystreamBlockInputRule
        {
            get
            {
                if (_keystreamBlockInputRule == null) _keystreamBlockInputRule = new UserInputValidationRule(1, ChaChaVisualization.TotalKeystreamBlocks);
                return _keystreamBlockInputRule;
            }
        }

        private KeyEventHandler _keystreamBlockInputHandler; public KeyEventHandler KeystreamBlockInputHandler
        {
            get
            {
                if (_keystreamBlockInputHandler == null) _keystreamBlockInputHandler = UserInputHandler(KeystreamBlockInputRule, GoToKeystreamBlock);
                return _keystreamBlockInputHandler;
            }
        }

        /// <summary>
        /// Go to given keystream block.
        /// </summary>
        /// <param name="keystreamBlock">One-based keystreamBlock index.</param>
        private void GoToKeystreamBlock(int keystreamBlock)
        {
            // Value comes from user. Map to zero-based round index.
            int keystreamBlockIndex = keystreamBlock - 1;
            int keystreamBlockActionIndex = GetTaggedActionIndex(KeystreamBlockStartTag(keystreamBlockIndex));
            MoveToAction(keystreamBlockActionIndex);
        }

        #endregion Keystream Block input

        #region Round input

        private ICommand _nextRoundCommand; public ICommand NextRoundCommand
        {
            get
            {
                if (_nextRoundCommand == null) _nextRoundCommand = new RelayCommand((arg) => NextRound(), (arg) => CanNextRound);
                return _nextRoundCommand;
            }
        }

        public bool CanNextRound
        {
            get
            {
                return CurrentRoundIndex == null || CurrentRoundIndex < Settings.Rounds - 1;
            }
        }

        private ICommand _prevRoundCommand; public ICommand PrevRoundCommand
        {
            get
            {
                if (_prevRoundCommand == null) _prevRoundCommand = new RelayCommand((arg) => PrevRound(), (arg) => CanPrevRound);
                return _prevRoundCommand;
            }
        }

        public bool CanPrevRound
        {
            get
            {
                return CurrentRoundIndex != null && CurrentRoundIndex != 0;
            }
        }

        private void NextRound()
        {
            // TODO check for round overflow where we have to increase keystream block index
            int keystreamBlockIndex = CurrentKeystreamBlockIndex ?? 0;
            int nextRoundIndex = CurrentRoundIndex == null ? 0 : (int)CurrentRoundIndex + 1;
            int nextRoundActionIndex = GetTaggedActionIndex(RoundStartTag(keystreamBlockIndex, nextRoundIndex));
            MoveToAction(nextRoundActionIndex);
        }

        private void PrevRound()
        {
            if (CurrentRoundIndex == null || CurrentRoundIndex == 0)
                throw new InvalidOperationException("CurrentRoundIndex was null or zero in PrevRound.");
            // TODO check for round underflow where we have to decrease keystream block index
            int keystreamBlockIndex = CurrentKeystreamBlockIndex ?? 0;
            int currentRoundStartIndex = GetTaggedActionIndex(RoundStartTag(keystreamBlockIndex, (int)CurrentRoundIndex));
            // only go back to start of previous round if we are on the start of a round
            // else go to start of current round
            if (CurrentActionIndex == currentRoundStartIndex)
            {
                int prevRoundIndex = (int)CurrentRoundIndex - 1;
                int prevRoundActionIndex = GetTaggedActionIndex(RoundStartTag(keystreamBlockIndex, prevRoundIndex));
                MoveToAction(prevRoundActionIndex);
            }
            else
            {
                MoveToAction(currentRoundStartIndex);
            }
        }

        private ValidationRule _roundInputRule; private ValidationRule RoundInputRule
        {
            get
            {
                if (_roundInputRule == null) _roundInputRule = new UserInputValidationRule(1, Settings.Rounds);
                return _roundInputRule;
            }
        }

        private KeyEventHandler _roundInputHandler; public KeyEventHandler RoundInputHandler
        {
            get
            {
                if (_roundInputHandler == null) _roundInputHandler = UserInputHandler(RoundInputRule, GoToRound);
                return _roundInputHandler;
            }
        }

        /// <summary>
        /// Go to given round.
        /// </summary>
        /// <param name="round">One-based round index.</param>
        private void GoToRound(int round)
        {
            int keystreamBlockIndex = CurrentKeystreamBlockIndex ?? 0;
            // Value comes from user. Map to zero-based round index.
            int roundIndex = round - 1;
            int roundActionIndex = GetTaggedActionIndex(RoundStartTag(keystreamBlockIndex, roundIndex));
            MoveToAction(roundActionIndex);
        }

        #endregion Round input

        #region Quarterround input

        private ICommand _nextQRCommand; public ICommand NextQRCommand
        {
            get
            {
                if (_nextQRCommand == null) _nextQRCommand = new RelayCommand((arg) => NextQR(), (arg) => CanNextQR);
                return _nextQRCommand;
            }
        }

        public bool CanNextQR
        {
            get
            {
                return CanNextRound || CurrentQRIndex == null || CurrentQRIndex < 3;
            }
        }

        private ICommand _prevQRCommand; public ICommand PrevQRCommand
        {
            get
            {
                if (_prevQRCommand == null) _prevQRCommand = new RelayCommand((arg) => PrevQR(), (arg) => CanPrevQR);
                return _prevQRCommand;
            }
        }

        public bool CanPrevQR
        {
            get
            {
                return CanPrevRound || (CurrentQRIndex != null && CurrentQRIndex != 0);
            }
        }

        private void NextQR()
        {
            // TODO check for qr overflow which results in round overflow where we have to increase keystream block index
            int keystreamBlockIndex = CurrentKeystreamBlockIndex ?? 0;
            int roundIndex = CurrentRoundIndex ?? 0;
            int nextQRIndex = CurrentQRIndex == null ? 0 : ((int)(CurrentQRIndex + 1) % 4);
            if (CurrentQRIndex != null && (int)CurrentQRIndex == 3) roundIndex += 1;
            int nextQRActionIndex = GetTaggedActionIndex(QRStartTag(keystreamBlockIndex, roundIndex, nextQRIndex));
            MoveToAction(nextQRActionIndex);
        }

        private void PrevQR()
        {
            if (CurrentQRIndex == null)
                throw new InvalidOperationException("CurrentQRIndex was null in PrevQR.");
            // TODO check for qr underflow which results in round underflow where we have to increase keystream block index
            int keystreamBlockIndex = CurrentKeystreamBlockIndex ?? 0;
            int currentRoundIndex = (int)CurrentRoundIndex;
            int currentQRIndex = (int)CurrentQRIndex;
            int currentQRStartIndex = GetTaggedActionIndex(QRStartTag(keystreamBlockIndex, currentRoundIndex, currentQRIndex));
            // only go back to start of previous qr if we are on the start of a qr
            // else go to start of current qr
            if (CurrentActionIndex == currentQRStartIndex)
            {
                // check if we would "underflow" and thus have to go to the last quarterround of previous round
                int prevQRIndex = currentQRIndex == 0 ? 3 : currentQRIndex - 1;
                int roundIndexForPrevQR = currentQRIndex == 0 ? currentRoundIndex - 1 : currentRoundIndex;
                int prevRoundActionIndex = GetTaggedActionIndex(QRStartTag(keystreamBlockIndex, roundIndexForPrevQR, prevQRIndex));
                MoveToAction(prevRoundActionIndex);
            }
            else
            {
                MoveToAction(currentQRStartIndex);
            }
        }

        private ICommand _quarterroundStartCommand; public ICommand QuarterroundStartCommand
        {
            get
            {
                if (_quarterroundStartCommand == null) _quarterroundStartCommand = new RelayCommand((arg) => GoToQRStart(int.Parse((string)arg)));
                return _quarterroundStartCommand;
            }
        }

        private ICommand _quarterroundEndCommand; public ICommand QuarterroundEndCommand
        {
            get
            {
                if (_quarterroundEndCommand == null) _quarterroundEndCommand = new RelayCommand((arg) => GoToQREnd(int.Parse((string)arg)));
                return _quarterroundEndCommand;
            }
        }

        /// <summary>
        /// Go to the quarterround start of the given qr index of the current round.
        /// </summary>
        /// <param name="qr">Zero-based qr index.</param>
        private void GoToQRStart(int qr)
        {
            int keystreamBlockIndex = CurrentKeystreamBlockIndex ?? 0;
            int currentRoundIndex = (int)CurrentRoundIndex;
            int round = CurrentRoundIndex ?? 0;
            int qrStartActionIndex = GetTaggedActionIndex(QRStartTag(keystreamBlockIndex, round, qr));
            MoveToAction(qrStartActionIndex);
        }

        /// <summary>
        /// Go to the quarterround end of the given qr index of the current round.
        /// </summary>
        /// <param name="qr">Zero-based qr index.</param>
        private void GoToQREnd(int qr)
        {
            int keystreamBlockIndex = CurrentKeystreamBlockIndex ?? 0;
            int currentRoundIndex = (int)CurrentRoundIndex;
            int round = CurrentRoundIndex ?? 0;
            int qrStartActionIndex = GetTaggedActionIndex(QREndTag(keystreamBlockIndex, round, qr));
            MoveToAction(qrStartActionIndex);
        }

        private ValidationRule _qrInputRule; private ValidationRule QRInputRule
        {
            get
            {
                if (_qrInputRule == null) _qrInputRule = new UserInputValidationRule(1, 4);
                return _qrInputRule;
            }
        }

        private KeyEventHandler _qrInputHandler; public KeyEventHandler QRInputHandler
        {
            get
            {
                if (_qrInputHandler == null) _qrInputHandler = UserInputHandler(QRInputRule, GoToQR);
                return _qrInputHandler;
            }
        }

        /// <summary>
        /// Go to given quarterround.
        /// </summary>
        /// <param name="qr">One-based quarterround index.</param>
        private void GoToQR(int qr)
        {
            int keystreamBlockIndex = CurrentKeystreamBlockIndex ?? 0;
            int currentRoundIndex = (int)CurrentRoundIndex;
            // Value comes from user. Map to zero-based round index.
            int qrIndex = qr - 1;
            int round = CurrentRoundIndex ?? 0;
            int qrActionIndex = GetTaggedActionIndex(QRStartTag(keystreamBlockIndex, round, qrIndex));
            MoveToAction(qrActionIndex);
        }

        #endregion Quarterround input

        #endregion ChaCha Hash Navigation Bar

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

        private ObservableCollection<StateValue> _originalState; public ObservableCollection<StateValue> OriginalState
        {
            get
            {
                if (_originalState == null) _originalState = new ObservableCollection<StateValue>();
                return _originalState;
            }
            private set
            {
                if (_originalState != value)
                {
                    _originalState = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<StateValue> _additionResultState; public ObservableCollection<StateValue> AdditionResultState
        {
            get
            {
                if (_additionResultState == null) _additionResultState = new ObservableCollection<StateValue>();
                return _additionResultState;
            }
            private set
            {
                if (_additionResultState != value)
                {
                    _additionResultState = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<StateValue> _littleEndianState; public ObservableCollection<StateValue> LittleEndianState
        {
            get
            {
                if (_littleEndianState == null) _littleEndianState = new ObservableCollection<StateValue>();
                return _littleEndianState;
            }
            private set
            {
                if (_littleEndianState != value)
                {
                    _littleEndianState = value;
                    OnPropertyChanged();
                }
            }
        }

        private int? _currentKeystreamBlockIndex = 0; public int? CurrentKeystreamBlockIndex
        {
            get
            {
                return _currentKeystreamBlockIndex;
            }
            set
            {
                if (_currentKeystreamBlockIndex != value)
                {
                    _currentKeystreamBlockIndex = value;
                    CurrentUserKeystreamBlockIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Same purpose as property 'CurrentUserActionIndex'.
        /// See its documentation for further information.
        /// </summary>
        private int? _currentUserKeystreamBlockIndex = null; public int? CurrentUserKeystreamBlockIndex

        {
            get
            {
                return _currentUserKeystreamBlockIndex;
            }
            set
            {
                if (_currentUserKeystreamBlockIndex != value)
                {
                    _currentUserKeystreamBlockIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private int? _currentRoundIndex = null; public int? CurrentRoundIndex
        {
            get
            {
                return _currentRoundIndex;
            }
            set
            {
                if (_currentRoundIndex != value)
                {
                    _currentRoundIndex = value;
                    CurrentUserRoundIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Same purpose as property 'CurrentUserActionIndex'.
        /// See its documentation for further information.
        /// </summary>
        private int? _currentUserRoundIndex = null; public int? CurrentUserRoundIndex

        {
            get
            {
                return _currentUserRoundIndex;
            }
            set
            {
                if (_currentUserRoundIndex != value)
                {
                    _currentUserRoundIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private int? _currentQRIndex = null; public int? CurrentQRIndex
        {
            get
            {
                return _currentQRIndex;
            }
            set
            {
                if (_currentQRIndex != value)
                {
                    _currentQRIndex = value;
                    CurrentUserQRIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Same purpose as property 'CurrentUserActionIndex'.
        /// See its documentation for further information.
        /// </summary>
        private int? _currentUserQRIndex = null; public int? CurrentUserQRIndex

        {
            get
            {
                return _currentUserQRIndex;
            }
            set
            {
                if (_currentUserQRIndex != value)
                {
                    _currentUserQRIndex = value;
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

        /// <summary>
        /// Indicates if we are currently in the rounds step or in the addition / little-endian step.
        /// </summary>
        private bool _roundsStep; public bool RoundsStep

        {
            get
            {
                return _roundsStep;
            }
            set
            {
                if (_roundsStep != value)
                {
                    _roundsStep = value;
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

        #region IActionTag

        /// <summary>
        /// Tag the last added action as the start of the given round.
        /// </summary>
        /// <param name="keystreamBlock">Zero-based keystream block index.</param>
        private void TagKeystreamBlockStartAction(int keystreamBlock)
        {
            TagAction(KeystreamBlockStartTag(keystreamBlock), ActionIndex);
        }

        /// <summary>
        /// Tag the last added action as the start of the given round.
        /// </summary>
        /// <param name="keystreamBlock">Zero-based keystream block index.</param>
        /// <param name="round">Zero-based round index.</param>
        private void TagRoundStartAction(int keystreamBlock, int round)
        {
            TagAction(RoundStartTag(keystreamBlock, round), ActionIndex);
        }

        /// <summary>
        /// Tag the last added action as the end of the given round.
        /// </summary>
        /// <param name="keystreamBlock">Zero-based keystream block index.</param>
        /// <param name="round">Zero-based round index.</param>
        private void TagRoundEndStartAction(int keystreamBlock, int round)
        {
            TagAction(RoundEndTag(keystreamBlock, round), ActionIndex);
        }

        /// <summary>
        /// Tag the last added action as the start of the given quarterround of the given round.
        /// </summary>
        /// <param name="keystreamBlock">Zero-based keystream block index.</param>
        /// <param name="round">Zero-based round index.</param>
        /// <param name="qr">Zero-based qr index.</param>
        private void TagQRStartAction(int keystreamBlock, int round, int qr)
        {
            TagAction(QRStartTag(keystreamBlock, round, qr), ActionIndex);
        }

        /// <summary>
        /// Tag the last added action as the end of the given quarterround of the given round.
        /// </summary>
        /// <param name="keystreamBlock">Zero-based keystream block index.</param>
        /// <param name="round">Zero-based round index.</param>
        /// <param name="qr">Zero-based qr index.</param>
        private void TagQREndAction(int keystreamBlock, int round, int qr)
        {
            TagAction(QREndTag(keystreamBlock, round, qr), ActionIndex);
        }

        /// <summary>
        /// Extend the action at the given action index with the given action.
        /// </summary>
        private void ExtendAction(int actionIndex, Action toExtend)
        {
            Action action = Actions[actionIndex];
            Action updated = action.Extend(toExtend);
            Actions[actionIndex] = updated;
        }

        /// <summary>
        /// Extend the last added action with the given action.
        /// </summary>
        private void ExtendLastAction(Action toExtend)
        {
            ExtendAction(ActionIndex, toExtend);
        }

        /// <summary>
        /// Check if keystream block input for tag is in valid range.
        /// </summary>
        /// <param name="keystreamBlock">Zero-based keystream block index.</param>
        private void AssertKeystreamBlockTagInput(int keystreamBlock)
        {
            int maxKeystreamBlock = ChaChaVisualization.TotalKeystreamBlocks - 1;
            if (keystreamBlock < 0 || keystreamBlock > maxKeystreamBlock)
                throw new ArgumentOutOfRangeException("keystreamBlock", $"Keystream Block must be between 0 and {maxKeystreamBlock}. Received: {keystreamBlock}");
        }

        /// <summary>
        /// Check if round input for tag is in valid range.
        /// </summary>
        /// <param name="round">Zero-based round index.</param>
        private void AssertRoundTagInput(int round)
        {
            int maxRoundIndex = Settings.Rounds - 1;
            if (round < 0 || round > maxRoundIndex)
                throw new ArgumentOutOfRangeException("round", $"Round must be between 0 and {maxRoundIndex}. Received: {round}");
        }

        /// <summary>
        /// Check if qr input for tag is in valid range.
        /// </summary>
        /// <param name="qr">Zero-based qr index.</param>
        private void AssertQRTagInput(int qr)
        {
            if (qr < 0 || qr > 3)
            {
                throw new ArgumentOutOfRangeException("qr", $"Quarterround must be between 0 and 3. Received {qr}");
            }
        }

        /// <summary>
        /// Create the keystream block start tag.
        /// </summary>
        /// <param name="keystreamBlock">Zero-based keystream block index.</param>
        private string KeystreamBlockStartTag(int keystreamBlock)
        {
            AssertKeystreamBlockTagInput(keystreamBlock);
            return $"KEYSTREAM_BLOCK_START_{keystreamBlock}";
        }

        /// <summary>
        /// Create the round start tag.
        /// </summary>
        /// <param name="keystreamBlock">Zero-based keystream block index.</param>
        /// <param name="round">Zero-based round index.</param>
        private string RoundStartTag(int keystreamBlock, int round)
        {
            AssertKeystreamBlockTagInput(keystreamBlock);
            AssertRoundTagInput(round);
            return $"KEYSTREAM_BLOCK_{keystreamBlock}_ROUND_START_{round}";
        }

        /// <summary>
        /// Create the round end tag.
        /// </summary>
        /// <param name="keystreamBlock">Zero-based keystream block index.</param>
        /// <param name="round">Zero-based round index.</param>
        private string RoundEndTag(int keystreamBlock, int round)
        {
            AssertKeystreamBlockTagInput(keystreamBlock);
            AssertRoundTagInput(round);
            return $"KEYSTREAM_BLOCK_{keystreamBlock}_ROUND_END_{round}";
        }

        /// <summary>
        /// Create the quarterround start tag.
        /// </summary>
        /// <param name="keystreamBlock">Zero-based keystream block index.</param>
        /// <param name="round">Zero-based round index.</param>
        /// <param name="qr">Zero-based qr index.</param>
        private string QRStartTag(int keystreamBlock, int round, int qr)
        {
            AssertKeystreamBlockTagInput(keystreamBlock);
            AssertRoundTagInput(round);
            AssertQRTagInput(qr);
            return $"KEYSTREAM_BLOCK_{keystreamBlock}_ROUND_{round}_QR_START_{qr}";
        }

        /// <summary>
        /// Create the quarterround end tag.
        /// </summary>
        /// <param name="keystreamBlock">Zero-based keystream block index.</param>
        /// <param name="round">Zero-based round index.</param>
        /// <param name="qr">Zero-based qr index.</param>
        private string QREndTag(int keystreamBlock, int round, int qr)
        {
            AssertKeystreamBlockTagInput(keystreamBlock);
            AssertRoundTagInput(round);
            AssertQRTagInput(qr);
            return $"KEYSTREAM_BLOCK_{keystreamBlock}_ROUND_{round}_QR_END_{qr}";
        }

        #endregion IActionTag

        #region INavigation

        public override void Setup()
        {
            Debug.Assert(StateValues.Count == 0, "StateValues should be empty during ChaCha hash setup.");
            Debug.Assert(OriginalState.Count == 0, "OriginalState should be empty during ChaCha hash setup.");
            Debug.Assert(AdditionResultState.Count == 0, "AdditionState should be empty during ChaCha hash setup.");
            Debug.Assert(LittleEndianState.Count == 0, "LittleEndianState should be empty during ChaCha hash setup.");
            uint[] state = ChaChaVisualization.OriginalState[0];
            for (int i = 0; i < 16; ++i)
            {
                StateValues.Add(new StateValue(state[i]));
                OriginalState.Add(new StateValue());
                AdditionResultState.Add(new StateValue());
                LittleEndianState.Add(new StateValue());
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
            OriginalState.Clear();
            AdditionResultState.Clear();
            LittleEndianState.Clear();
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