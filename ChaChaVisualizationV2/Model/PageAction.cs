using System;
using System.Collections.Generic;

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
        /// The list of actions this action extends.
        /// Named `baseActions` because this action can be called to be "based" on them.
        /// </summary>
        private List<IAction> BaseActions { get; set; } = new List<IAction>();

        /// <summary>
        /// The list of actions of this page action.
        /// </summary>
        private List<Action> Actions { get; set; } = new List<Action>();

        public PageAction(Action action)
        {
            Actions.Add(action);
        }

        public void Extend(IAction action)
        {
            BaseActions.Add(action);
        }

        public void Extend(Action action)
        {
            BaseActions.Add(new PageAction(action));
        }

        public void Invoke()
        {
            foreach (IAction baseAction in BaseActions)
            {
                baseAction.Invoke();
            }
            foreach (Action action in Actions)
            {
                action.Invoke();
            }
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

        public static PageAction Extend(this Action a, Action action)
        {
            PageAction pageAction = new PageAction(a);
            pageAction.Extend(action);
            return pageAction;
        }
    }
}