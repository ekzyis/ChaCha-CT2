using System;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel.Components
{
    /// <summary>
    /// A helper class which creates the step operation actions for the quarterround visualization.
    /// This class is responsible for correctly retrieving the results
    /// by mapping the given round, quarterround and step index to the array index
    /// for a particular step operation (addition, XOR or shift).
    ///
    /// For example, if we want to have the addition result of round 2,
    /// quarterround 2, step 2 we need to access the index 6 of the QRStep array
    /// and get the addition result from that QRStep instance.
    /// This is so because the array is in this format:
    ///
    /// QR Step Array: [ Step0, Step1, Step2, Step3, Step0, Step1, Step2, Step3, ... ]
    /// Index:           0      1      2      3      4      5      6      7
    /// Quarterround:    QR 1                        QR 2
    /// Round:           Round 1
    /// </summary>
    internal class QRStepOperationActionCreator
    {
        protected enum QRStepOperation
        {
            ADD, XOR, SHIFT
        }

        /// <summary>
        /// Create an quarterround action creator instance
        /// for the given round.
        /// </summary>
        /// <param name="round">Zero-based round index.</param>
        protected QRStepOperationActionCreator(ChaChaHashViewModel viewModel, QRStepOperation operation)
        {
            VM = viewModel;
            Operation = operation;
        }

        private QRStepOperation Operation { get; set; }

        public ChaChaHashViewModel VM { get; private set; }

        private void AssertRoundInput(int round)
        {
            int maxRoundIndex = VM.Settings.Rounds - 1;
            if (round < 0 || round > maxRoundIndex)
                throw new ArgumentOutOfRangeException("round", $"round must be between 0 and {maxRoundIndex}. Received {round}.");
        }

        private void AssertQRInput(int qr)
        {
            if (0 < qr || qr > 3) throw new ArgumentOutOfRangeException("qr", $"qr must be between 0 and 3. Received {qr}");
        }

        private void AssertQRStepInput(int qrStep)
        {
            if (0 < qrStep || qrStep > 3) throw new ArgumentOutOfRangeException("qrStep", $"qrStep must be between 0 and 3. Received {qrStep}");
        }

        /// <summary>
        /// The "big brain" method.
        /// Return the array index to access to correct QRStep instance.
        /// </summary>
        /// <param name="round">Zero-based round index.</param>
        /// <param name="qr">Zero-based quarterround index.</param>
        /// <param name="qrStep">Zero-based quarterround step index.</param>
        private int MapIndex(int round, int qr, int qrStep)
        {
            // For every round, we need to skip 16 steps.
            // For every quarterround, we need to skip 4 steps.
            return round * 16 + qr * 4 + qrStep;
        }

        /// <summary>
        /// Action which marks the input paths and boxes for the step operation.
        /// </summary>
        /// <param name="round">Zero-based round index.</param>
        /// <param name="qr">Zero-based quarterround index.</param>
        /// <param name="qrStep">Zero-based quarterround step index.</param>
        public Action MarkInputs(int round, int qr, int qrStep)
        {
            AssertRoundInput(round);
            AssertQRInput(qr);
            AssertQRStepInput(qrStep);
            int arrayIndex = MapIndex(round, qr, qrStep);
            if (Operation == QRStepOperation.ADD)
                return () => VM.QRStep[arrayIndex].Add.MarkInput = true;
            else if (Operation == QRStepOperation.XOR)
                return () => VM.QRStep[arrayIndex].XOR.MarkInput = true;
            else if (Operation == QRStepOperation.SHIFT)
                return () => VM.QRStep[arrayIndex].Shift.MarkInput = true;
            throw new InvalidOperationException("Could not find a matching QRStepOperation.");
        }

        /// <summary>
        /// Action which marks the box for the step operation result.
        /// </summary>
        /// <param name="round">Zero-based round index.</param>
        /// <param name="qr">Zero-based quarterround index.</param>
        /// <param name="qrStep">Zero-based quarterround step index.</param>
        public Action Mark(int round, int qr, int qrStep)
        {
            AssertRoundInput(round);
            AssertQRInput(qr);
            AssertQRStepInput(qrStep);
            int arrayIndex = MapIndex(round, qr, qrStep);
            if (Operation == QRStepOperation.ADD)
                return () => VM.QRStep[arrayIndex].Add.Mark = true;
            else if (Operation == QRStepOperation.XOR)
                return () => VM.QRStep[arrayIndex].XOR.Mark = true;
            else if (Operation == QRStepOperation.SHIFT)
                return () => VM.QRStep[arrayIndex].Shift.Mark = true;
            throw new InvalidOperationException("Could not find a matching QRStepOperation.");
        }

        /// <summary>
        /// Action which inserts the result for the step operation.
        /// </summary>
        /// <param name="round">Zero-based round index.</param>
        /// <param name="qr">Zero-based quarterround index.</param>
        /// <param name="qrStep">Zero-based quarterround step index.</param>
        public Action Insert(int round, int qr, int qrStep)
        {
            AssertRoundInput(round);
            AssertQRInput(qr);
            AssertQRStepInput(qrStep);
            int arrayIndex = MapIndex(round, qr, qrStep);
            if (Operation == QRStepOperation.ADD)
                return () => VM.QRStep[arrayIndex].Add.Value = VM.ChaChaVisualization.QRStep[arrayIndex].Add;
            else if (Operation == QRStepOperation.XOR)
                return () => VM.QRStep[arrayIndex].XOR.Value = VM.ChaChaVisualization.QRStep[arrayIndex].XOR;
            else if (Operation == QRStepOperation.SHIFT)
                return () => VM.QRStep[arrayIndex].Shift.Value = VM.ChaChaVisualization.QRStep[arrayIndex].Shift;
            throw new InvalidOperationException("Could not find a matching QRStepOperation.");
        }
    }

    /// <summary>
    /// A helper class which creates the actions for the addition operation during the quarterround visualization.
    /// </summary>
    internal class QRAdditionActionCreator : QRStepOperationActionCreator
    {
        public QRAdditionActionCreator(ChaChaHashViewModel viewModel) : base(viewModel, QRStepOperation.ADD)
        {
        }
    }

    /// <summary>
    /// A helper class which creates the actions for the XOR operation during the quarterround visualization.
    /// </summary>
    internal class QRXORActionCreator : QRStepOperationActionCreator
    {
        public QRXORActionCreator(ChaChaHashViewModel viewModel) : base(viewModel, QRStepOperation.XOR)
        {
        }
    }

    /// <summary>
    /// A helper class which creates the actions for the shift operation during the quarterround visualization.
    /// </summary>
    internal class QRShiftActionCreator : QRStepOperationActionCreator
    {
        public QRShiftActionCreator(ChaChaHashViewModel viewModel) : base(viewModel, QRStepOperation.SHIFT)
        {
        }
    }
}