using System;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    /// <summary>
    /// Interface to streamline process of action creation.
    ///
    /// Since we are using a centralized navigation system, we need to define the complete page state in every action.
    /// This interface documents and defines helper methods for a high-level action creation API.
    /// </summary>
    internal interface IActionCreator
    {
        /// <summary>
        /// Add the given action to the baseline.
        ///
        /// The baseline is the action which all future actions will extend.
        /// This function can be called multiple times to extend the baseline itself.
        /// </summary>
        void PushBaseline(Action baseline);

        /// <summary>
        /// Remove the latest added baseline action from the baseline.
        /// </summary>
        void PopBaseline();

        /// <summary>
        /// Remove all baseline actions.
        /// </summary>
        void ResetBaseline();

        /// <summary>
        /// Extends the given action with the current baseline and adding it to the baseline.
        /// This enables creation of actions which reuse the code of all previous actions without having
        /// to rewrite their action code.
        ///
        /// <example>
        /// > Action a = Consecutive(() => Console.WriteLine("1"));
        /// > Action b = Consecutive(() => Console.WriteLine("2"));
        /// > Action c = Consecutive(() => Console.WriteLine("3"));
        /// > a.Invoke();
        ///   1
        /// > b.Invoke();
        ///   1
        ///   2
        /// > c.Invoke();
        ///   1
        ///   2
        ///   3
        /// </example>
        /// </summary>
        Action Consecutive(Action action);
    }
}