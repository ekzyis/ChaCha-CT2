namespace Cryptool.Plugins.ChaCha.Visualization.ViewModel.Components
{
    /// <summary>
    /// Interface for page view models which need an instance of ChaCha, ChaChaVisualization or ChaChaSettings
    /// </summary>
    internal interface IChaCha
    {
        ChaChaPresentationViewModel PresentationViewModel { get; }

        ChaChaVisualization ChaChaVisualization { get; }

        ChaCha ChaCha { get; }

        ChaChaSettings Settings { get; }
    }
}