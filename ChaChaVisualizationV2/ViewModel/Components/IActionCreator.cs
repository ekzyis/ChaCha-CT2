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
        /// Pop the latest n added baseline actions from the baseline.
        /// </summary>
        /// <param name="n"></param>
        void PopBaseline(int n);

        /// <summary>
        /// Remove all baseline actions.
        /// </summary>
        void ResetBaseline();

        /// <summary>
        /// Remove all baseline actions and add given action as new baseline.
        /// </summary>
        /// <param name="newBaseline"></param>
        void ResetBaseline(Action newBaseline);

        /// <summary>
        /// Extends the given action with the current baseline.
        /// </summary>
        Action ExtendBaseline(Action action);

        /// <summary>
        /// Extends the given action with the current baseline and adding it to the baseline.
        /// This enables creation of actions which reuse the code of previous actions without having
        /// to rewrite their action code.
        ///
        /// <example>
        ///   <para>
        ///   Example:
        ///   <code>
        ///     > Action a = Sequential(() => Console.WriteLine("1")); <br/>
        ///     > Action b = Sequential(() => Console.WriteLine("2")); <br/>
        ///     > Action c = Sequential(() => Console.WriteLine("3")); <br/>
        ///     > a.Invoke(); <br/>
        ///       1 <br/>
        ///     > b.Invoke(); <br/>
        ///       1 <br/>
        ///       2 <br/>
        ///     > c.Invoke(); <br/>
        ///       1 <br/>
        ///       2 <br/>
        ///       3 <br/>
        ///     </code>
        ///   </para>
        /// </example>
        /// </summary>
        Action Sequential(Action action);

        /// <summary>
        /// Reset the sequence; removing all sequential actions from the baseline
        /// thus resetting it to the baseline before the first sequential action was created.
        /// </summary>
        /// <returns></returns>
        void ResetSequence();
    }
}