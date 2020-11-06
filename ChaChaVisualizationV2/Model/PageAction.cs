using System;
using System.Collections.Generic;
using System.Linq;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Model
{
    /// <summary>
    /// Class for page actions.
    /// Wraps execution logic for a single page action.
    ///
    /// Due to the centralized navigation design, which means that every page action needs to fully define the state of the page,
    /// we wrap page actions around other page actions to still be able to easily reuse the state of the previous page action.
    /// This makes writing consecutive page action which are based on each other easier by preventing code duplication.
    ///
    /// Furthermore, this should not have any performance impact since we still only execute the logic needed to visualize the state
    /// of the current action.
    ///
    /// </summary>
    internal class PageAction : IAction
    {
        /// <summary>
        /// The action which will perform the UI changes for this page action.
        /// </summary>
        private Action Action { get; set; }

        public PageAction(Action action)
        {
            Action = action;
        }

        public IAction Extend(IAction action)
        {
            return new PageAction(Action.Extend(Action));
        }

        /// <summary>
        /// Syntactic sugar for Extend chaining.
        /// In other words: action.Extend(x, y) is equivalent to action.Extend(x).Extend(y)
        /// </summary>
        public IAction Extend(params IAction[] actions)
        {
            return actions.Aggregate((acc, curr) => acc.Extend(curr));
        }

        public void Invoke()
        {
            Action.Invoke();
        }

        public static implicit operator PageAction(Action action) => new PageAction(action);

        public static implicit operator Action(PageAction pageAction) => () => pageAction.Invoke();
    }

    internal static class PageActionExtensions
    {
        /// <summary>
        /// Extension method to automatically wrap actions into a PageAction.
        /// </summary>
        public static void Add(this List<PageAction> list, Action action)
        {
            PageAction wrappedAction = new PageAction(action);
            list.Add(wrappedAction);
        }

        /// <summary>
        /// Extension method to extend actions.
        /// The last parameter is the action from which we want to derive a new, extended action.
        /// This is why we call it first in the new returned action.
        /// </summary>
        public static Action Extend(this Action action, Action toExtend)
        {
            return () =>
            {
                toExtend.Invoke();
                action.Invoke();
            };
        }
    }
}