using Cryptool.Plugins.ChaCha;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    /// <summary>
    /// Interface for page view models which need an instance of ChaCha or ChaChaSettings
    /// </summary>
    internal interface IChaCha
    {
        ChaCha.ChaCha ChaCha { get; }

        ChaChaSettings Settings { get; }
    }
}