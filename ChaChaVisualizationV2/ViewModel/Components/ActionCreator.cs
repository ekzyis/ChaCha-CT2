using Cryptool.Plugins.ChaChaVisualizationV2.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel.Components
{
    internal class ActionCreator : IActionCreator
    {
        private Stack<Action> Baseline { get; set; } = new Stack<Action>();

        private Action AggregatedBaseline { get => Baseline.Aggregate((acc, curr) => curr.Extend(acc)); }

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
        }

        public Action ExtendBaseline(Action action)
        {
            return action.Extend(AggregatedBaseline);
        }

        public Action Sequential(Action action)
        {
            Action extended = action.Extend(AggregatedBaseline);
            PushBaseline(action);
            return extended;
        }
    }
}