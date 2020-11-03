using Cryptool.Plugins.ChaChaVisualizationV2.Helper;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    /// <summary>
    /// View model base for all view models which implement action navigation
    /// </summary>
    internal abstract class ActionViewModelBase : ViewModelBase, IActionNavigation
    {
        public ActionViewModelBase()
        {
            CurrentActionIndex = 0;
            // Make sure that the action at index 0 is the initial page state.
            Actions.Add(() => Reset());
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

        public void ActionSliderValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int actionIndex = (int)e.NewValue;
            MoveToAction(actionIndex);
        }

        /// <summary>
        /// Reset the page, undoing any action that was applied to it.
        /// </summary>
        public abstract void Reset();

        #region IActionNavigation

        private int _currentActionIndex; public int CurrentActionIndex
        {
            get
            {
                return _currentActionIndex;
            }
            set
            {
                if (_currentActionIndex != value)
                {
                    _currentActionIndex = value;
                    OnPropertyChanged();
                }
            }
        }

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
            Actions[n]();
            CurrentActionIndex = n;
        }

        public void MoveToFirstAction()
        {
            MoveToAction(0);
        }

        public void MoveToLastAction()
        {
            MoveToAction(TotalActions - 1);
        }

        private ICommand _nextActionCommand; public ICommand NextActionCommand
        {
            get
            {
                if (_nextActionCommand == null) _nextActionCommand = new RelayCommand((arg) => NextAction(), (arg) => CanNextAction);
                return _nextActionCommand;
            }
        }

        public void NextAction()
        {
            MoveActions(1);
        }

        public bool CanNextAction
        {
            get
            {
                return CurrentActionIndex < TotalActions - 1;
            }
        }

        private ICommand _prevActionCommand; public ICommand PrevActionCommand
        {
            get
            {
                if (_prevActionCommand == null) _prevActionCommand = new RelayCommand((arg) => PrevAction(), (arg) => CanPrevAction);
                return _prevActionCommand;
            }
        }

        public void PrevAction()
        {
            MoveActions(-1);
        }

        public bool CanPrevAction
        {
            get
            {
                return CurrentActionIndex != 0;
            }
        }

        #endregion IActionNavigation
    }
}