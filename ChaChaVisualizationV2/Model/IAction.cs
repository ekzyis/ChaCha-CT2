using System;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Model
{
    /// <summary>
    /// Interface for page actions.
    /// </summary>
    internal interface IAction
    {
        /// <summary>
        /// Return a new action which extends the current action with the given action.
        /// </summary>
        Action Extend(Action action);

        /// <summary>
        /// Invoke the action; executing it.
        /// </summary>
        void Invoke();
    }
}