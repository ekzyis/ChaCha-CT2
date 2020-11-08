using System;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel.Components
{
    /// <summary>
    /// A helper class which creates the input and output actions for the quarterround visualization
    /// for the given round.
    /// </summary>
    internal class QRIOActionCreator
    {
        /// <summary>
        /// Creates an instance to help with quarterround input action creation for the given round.
        /// </summary>
        /// <param name="round">Zero-based round index.</param>
        public QRIOActionCreator(ChaChaHashViewModel viewModel)
        {
            VM = viewModel;
        }

        public ChaChaHashViewModel VM { get; private set; }

        private void AssertRoundInput(int round)
        {
            int maxRoundIndex = VM.Settings.Rounds - 1;
            if (round < 0 || round > maxRoundIndex)
                throw new ArgumentOutOfRangeException("round", $"round must be between 0 and {maxRoundIndex}. Received {round}.");
        }

        private void AssertQRInput(int qr)
        {
            if (qr < 0 || qr > 3) throw new ArgumentOutOfRangeException("qr", $"qr must be between 0 and 3. Received {qr}");
        }

        /// <summary>
        /// Mark the state boxes which will be the qr inputs of the given round.
        /// </summary>
        /// <param name="round">Zero-based round index.</param>
        /// <param name="qr">Zero-based quarterround index.</param>
        public Action MarkState(int round, int qr)
        {
            return () =>
            {
                foreach (int i in GetStateIndices(round, qr))
                {
                    VM.StateValues[i].Mark = true;
                }
            };
        }

        /// <summary>
        /// Get the state indices which depend on the round and quarterround.
        /// </summary>
        /// <param name="round">Zero-based round index.</param>
        /// <param name="qr">Zero-based quarterround index.</param>
        private int[] GetStateIndices(int round, int qr)
        {
            AssertRoundInput(round);
            AssertQRInput(qr);
            if (round % 2 == 0)
            {
                // Column rounds
                switch (qr)
                {
                    case 0: return new int[] { 0, 4, 8, 12 };
                    case 1: return new int[] { 1, 5, 9, 13 };
                    case 2: return new int[] { 2, 6, 10, 14 };
                    case 3: return new int[] { 3, 7, 11, 15 };
                }
            }
            else
            {
                // Diagonal rounds
                switch (qr)
                {
                    case 0: return new int[] { 0, 5, 10, 15 };
                    case 1: return new int[] { 1, 6, 11, 12 };
                    case 2: return new int[] { 2, 7, 8, 13 };
                    case 3: return new int[] { 3, 4, 9, 14 };
                }
            }
            throw new InvalidOperationException($"No matching state indices found for given quarterround index {qr}");
        }

        /// <summary>
        /// Action which marks the qr input boxes.
        /// </summary>
        public Action MarkQRInputs
        {
            get
            {
                return () =>
                {
                    VM.QRInA.Mark = true;
                    VM.QRInB.Mark = true;
                    VM.QRInC.Mark = true;
                    VM.QRInD.Mark = true;
                };
            }
        }

        /// <summary>
        /// Action which inserts the QR input values into the QR input boxes.
        /// </summary>
        /// <param name="qr">Zero-based quarterround index.</param>
        public Action InsertQRInputs(int qr)
        {
            AssertQRInput(qr);
            return () => (VM.QRInA.Value, VM.QRInB.Value, VM.QRInC.Value, VM.QRInD.Value) = VM.ChaChaVisualization.QRInput[qr];
        }

        /// <summary>
        /// Action which marks the qr output paths.
        /// </summary>
        public Action MarkQROutputPaths
        {
            get
            {
                return () =>
                {
                    VM.QROutA.MarkInput = true;
                    VM.QROutB.MarkInput = true;
                    VM.QROutC.MarkInput = true;
                    VM.QROutD.MarkInput = true;
                };
            }
        }

        /// <summary>
        /// Action which marks the qr output boxes.
        /// </summary>
        public Action MarkQROutputs
        {
            get
            {
                return () =>
                {
                    VM.QROutA.Mark = true;
                    VM.QROutB.Mark = true;
                    VM.QROutC.Mark = true;
                    VM.QROutD.Mark = true;
                };
            }
        }

        /// <summary>
        /// Action which inserts the QR output values into the QR output boxes.
        /// </summary>
        /// <param name="qr">Zero-based quarterround index.</param>
        public Action InsertQROutputs(int qr)
        {
            AssertQRInput(qr);
            return () => (VM.QROutA.Value, VM.QROutB.Value, VM.QROutC.Value, VM.QROutD.Value) = VM.ChaChaVisualization.QROutput[qr];
        }
    }
}