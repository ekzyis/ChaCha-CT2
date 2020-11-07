using System;
using System.Collections.Generic;
using System.Linq;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel.Components
{
    internal class ActionCreator : IActionCreator
    {
        private Stack<Action> Baseline { get; set; } = new Stack<Action>();

        private Action AggregatedBaseline { get => Baseline.Aggregate((acc, curr) => curr.Extend(acc)); }

        private int SequenceCount { get; set; }

        public void PushBaseline(Action baseline)
        {
            Baseline.Push(baseline);
        }

        public void PopBaseline()
        {
            Baseline.Pop();
        }

        public void ResetBaseline()
        {
            Baseline.Clear();
            SequenceCount = 0;
        }

        public void ResetBaseline(Action newBaseline)
        {
            ResetBaseline();
            PushBaseline(newBaseline);
        }

        public Action ExtendBaseline(Action action)
        {
            return action.Extend(AggregatedBaseline);
        }

        public Action Sequential(Action action)
        {
            Action extended = action.Extend(AggregatedBaseline);
            PushBaseline(action);
            SequenceCount++;
            return extended;
        }

        public void ResetSequence()
        {
            for (int i = 0; i < SequenceCount; ++i)
            {
                PopBaseline();
            }
            SequenceCount = 0;
        }

        public void ResetSequence(Action newBaseline)
        {
            ResetSequence();
            PushBaseline(newBaseline);
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