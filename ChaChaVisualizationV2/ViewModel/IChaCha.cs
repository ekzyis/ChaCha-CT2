using Cryptool.Plugins.ChaCha;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    /// <summary>
    /// Interface for page view models which need an instance of ChaCha, ChaChaVisualization or ChaChaSettings
    /// </summary>
    internal interface IChaCha
    {
        ChaChaVisualizationV2 ChaChaVisualization { get; }

        ChaCha.ChaCha ChaCha { get; }

        ChaChaSettings Settings { get; }
    }
}