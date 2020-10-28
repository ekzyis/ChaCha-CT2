﻿using System;
using System.Collections.Generic;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    internal abstract class ActionViewModelBase : ViewModelBase, IActionNavigation
    {
        public ActionViewModelBase()
        {
            // Set action index to -1 such that NextAction will increment it to 0 and execute the first action in the list of actions.
            CurrentActionIndex = -1;
        }

        private List<Action> _actions; public List<Action> Actions
        {
            get
            {
                if (_actions == null) _actions = new List<Action>();
                return _actions;
            }
            set
            {
                if (_actions != value)
                {
                    _actions = value;
                    OnPropertyChanged(); OnPropertyChanged("TotalActions"); OnPropertyChanged("HasActions");
                }
            }
        }

        /// <summary>
        /// Reset the page, undoing any action that was applied to it.
        /// </summary>
        public abstract void Reset();

        #region IActionNavigation

        public int CurrentActionIndex { get; private set; }

        public int TotalActions { get => Actions.Count; }

        public bool HasActions { get => TotalActions != 0; }

        public void MoveActions(int n)
        {
            MoveToAction(CurrentActionIndex + n);
        }

        public void MoveToAction(int n)
        {
            if (n > TotalActions - 1 || n < 0)
            {
                throw new ArgumentOutOfRangeException("n", n, $"Action index out of range. Total actions: {TotalActions}");
            }
            Reset();
            CurrentActionIndex = n;
            Actions[CurrentActionIndex]();
        }

        public void MoveToFirstAction()
        {
            MoveToAction(0);
        }

        public void MoveToLastAction()
        {
            MoveToAction(TotalActions - 1);
        }

        public void NextAction()
        {
            Reset();
            CurrentActionIndex++;
            Actions[CurrentActionIndex]();
        }

        public void PrevAction()
        {
            Reset();
            CurrentActionIndex--;
            Actions[CurrentActionIndex]();
        }

        #endregion IActionNavigation
    }
}