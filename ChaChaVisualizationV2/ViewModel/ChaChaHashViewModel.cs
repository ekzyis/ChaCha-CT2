﻿using Cryptool.Plugins.ChaChaVisualizationV2.Model;
using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel.Components;
using System;
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

        /// <summary>
        /// Method which wraps ActionCreator.ResetSequence.
        /// Calls ActionCreator.ResetSequence and adds action to show new baseline by adding an empty action.
        /// </summary>
        private void ResetSequence(Action newBaseline)
        {
            ActionCreator.ResetSequence(newBaseline);
            Seq(() => { });
        }

        protected override void InitActions()
        {
            QRIOActionCreator qrIO = new QRIOActionCreator(this);

            QRAdditionActionCreator qrAdd = new QRAdditionActionCreator(this);
            QRXORActionCreator qrXOR = new QRXORActionCreator(this);
            QRShiftActionCreator qrShift = new QRShiftActionCreator(this);

            for (int round = 0; round < Settings.Rounds; ++round)
            {
                for (int qr = 0; qr < 4; ++qr)
                {
                    // Copy from state into quarterround input
                    Seq(qrIO.MarkState(round, qr));
                    Seq(qrIO.InsertQRInputs(round, qr).Extend(qrIO.MarkQRInputs));

                    ResetSequence(qrIO.InsertQRInputs(round, qr));

                    // Run quarterround
                    for (int qrStep = 0; qrStep < 4; ++qrStep)
                    {
                        // Execute addition
                        Seq(qrAdd.MarkInputs(qrStep));
                        Seq(qrAdd.Insert(round, qr, qrStep).Extend(qrAdd.Mark(qrStep)));

                        ResetSequence(qrAdd.Insert(round, qr, qrStep));

                        // Execute XOR
                        Seq(qrXOR.MarkInputs(qrStep));
                        Seq(qrXOR.Insert(round, qr, qrStep).Extend(qrXOR.Mark(qrStep)));

                        ResetSequence(qrXOR.Insert(round, qr, qrStep));

                        // Execute shift
                        Seq(qrShift.MarkInputs(qrStep));
                        Seq(qrShift.Insert(round, qr, qrStep).Extend(qrShift.Mark(qrStep)));

                        ResetSequence(qrShift.Insert(round, qr, qrStep));
                    }

                    Seq(qrIO.MarkQROutputPaths);
                    Seq(qrIO.InsertQROutputs(round, qr).Extend(qrIO.MarkQROutputs));

                    ResetSequence(qrIO.InsertQROutputs(round, qr));

                    Seq(qrIO.MarkQROutputs);
                    Seq(qrIO.UpdateState(round, qr).Extend(qrIO.MarkState(round, qr)));

                    ResetSequence(qrIO.UpdateState(round, qr).Extend(() => ResetQuarterroundValues()));
                }
            }
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
            Debug.Assert(QRStep.Count == 0, "QRStep should be empty during ChaCha hash setup.");
            // For each round, there are four quarterround and each quarterround has four steps.
            for (int i = 0; i < Settings.Rounds * 4 * 4; ++i)
            {
                QRStep.Add(new VisualQRStep());
            }
        }

        public void Teardown()
        {
            MoveToFirstAction();
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