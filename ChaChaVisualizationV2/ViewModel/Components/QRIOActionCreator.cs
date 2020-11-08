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
        public QRIOActionCreator(ChaChaHashViewModel viewModel, int round)
        {
            int maxRoundIndex = viewModel.Settings.Rounds - 1;
            if (round < 0 || round > maxRoundIndex)
                throw new ArgumentOutOfRangeException("round", $"round must be between 0 and {maxRoundIndex}. Received {round}.");
            VM = viewModel;
            Round = round;
        }

        public ChaChaHashViewModel VM { get; private set; }

        public int Round { get; private set; }

        /// <summary>
        /// Mark the state boxes at the given indices.
        /// </summary>
        public Action MarkState(params int[] stateIndices)
        {
            return () =>
            {
                foreach (int i in stateIndices)
                {
                    VM.StateValues[i].Mark = true;
                }
            };
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
        public Action InsertQRInputs
        {
            get
            {
                return () => (VM.QRInA.Value, VM.QRInB.Value, VM.QRInC.Value, VM.QRInD.Value) = VM.ChaChaVisualization.QRInput[Round];
            }
        }
    }
}