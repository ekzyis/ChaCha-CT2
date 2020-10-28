namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    /// <summary>
    /// Interface for page view models which are navigable to by the page navigation.
    /// </summary>
    internal interface INavigation
    {
        /// <summary>
        /// Page name. Will be used as content in page navigation button.
        /// </summary>
        string Name { get; set; }
    }
}