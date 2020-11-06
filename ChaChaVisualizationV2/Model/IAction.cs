namespace Cryptool.Plugins.ChaChaVisualizationV2.Model
{
    /// <summary>
    /// Interface for page actions.
    /// </summary>
    internal interface IAction
    {
        /// <summary>
        /// Mark this action as an extension/base of the given action.
        /// </summary>
        void Extend(IAction action);

        /// <summary>
        /// Invoke the action; executing it.
        /// </summary>
        void Invoke();
    }
}