using System;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel.Components
{
    /// <summary>
    /// Base class for all QR action creators.
    /// Implements the MapIndex method and argument validation.
    /// </summary>
    abstract class QRActionCreator
    {
        protected QRActionCreator(ChaChaHashViewModel viewModel)
        {
            VM = viewModel;
        }

        protected ChaChaHashViewModel VM { get; private set; }

        protected void AssertRoundInput(int round)
        {
            int maxRoundIndex = VM.Settings.Rounds - 1;
            if (round < 0 || round > maxRoundIndex)
                throw new ArgumentOutOfRangeException("round", $"round must be between 0 and {maxRoundIndex}. Received {round}.");
        }

        protected void AssertQRInput(int qr)
        {
            if (qr < 0 || qr > 3) throw new ArgumentOutOfRangeException("qr", $"qr must be between 0 and 3. Received {qr}");
        }

        protected void AssertQRStepInput(int qrStep)
        {
            if (qrStep < 0 || qrStep > 3) throw new ArgumentOutOfRangeException("qrStep", $"qrStep must be between 0 and 3. Received {qrStep}");
        }

        /// <summary>
        /// The "big brain" method.
        /// Return the array index to access to correct QRStep instance.
        /// </summary>
        /// <param name="round">Zero-based round index.</param>
        /// <param name="qr">Zero-based quarterround index.</param>
        /// <param name="qrStep">Zero-based quarterround step index.</param>
        protected int MapIndex(int round, int qr, int qrStep)
        {
            // For every round, we need to skip 16 steps.
            // For every quarterround, we need to skip 4 steps.
            return round * 16 + qr * 4 + qrStep;
        }

        /// <summary>
        /// The "big brain" method.
        /// Return the array index to acess the correct QRInput/QROutput.
        /// </summary>
        /// <param name="round">Zero-based round index.</param>
        /// <param name="qr">Zero-based quarterround index.</param>
        protected int MapIndex(int round, int qr)
        {
            // For every round, we need to skip 4 quarterrounds.
            return round * 4 + qr;
        }
    }
}