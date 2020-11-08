using System;
using System.Collections.Generic;
using System.Linq;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel.Components
{
    internal class ActionCreator : IActionCreator
    {
        /// <summary>
        /// Data structure to store sequences.
        /// </summary>
        private class Sequence : Stack<Action>, IEnumerable<Action>
        {
            public Action AggregatedAction
            {
                get
                {
                    if (Count == 0) return () => { };
                    return this.Aggregate((Action acc, Action curr) => curr.Extend(acc));
                }
            }
        }

        private Stack<Sequence> Sequences { get; set; } = new Stack<Sequence>();

        public void StartSequence()
        {
            Sequences.Push(new Sequence());
        }

        public void EndSequence()
        {
            if (Sequences.Count == 0)
                throw new InvalidOperationException("Cannot end sequence because there is none.");
            Sequences.Pop();
        }

        public Action Sequential(Action action)
        {
            if (Sequences.Count == 0)
                throw new InvalidOperationException("Cannot create sequential action because there has no sequence been started. Please call `StartSequence` first.");
            Action baseAction = Sequences
                .Select(s => s.AggregatedAction)
                .Aggregate((Action acc, Action curr) => curr.Extend(acc));
            Action extendedAction = action.Extend(baseAction);
            Sequences.Peek().Push(action);
            return extendedAction;
        }
    }

    internal static class ActionExtensions
    {
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

        /// <summary>
        /// Syntactic sugar for Extend chaining.
        /// In other words: Action.Extend(x, y) is equivalent to Action.Extend(x).Extend(y)
        /// </summary>
        public static Action Extend(this Action action, params Action[] toExtend)
        {
            return toExtend.Aggregate(action, (acc, curr) => acc.Extend(curr));
        }
    }
}