using Cryptool.Plugins.ChaCha;
using Cryptool.Plugins.ChaChaVisualizationV2.Helper;
using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel.Components;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    /// <summary>
    /// View model base for all view models which implement action navigation
    /// </summary>
    internal abstract class ActionViewModelBase : ViewModelBase, IActionNavigation, IChaCha, INavigation
    {
        public ActionViewModelBase(ChaChaPresentationViewModel chachaPresentationViewModel)
        {
            PresentationViewModel = chachaPresentationViewModel;
            CurrentActionIndex = 0;
            // Make sure that the action at index 0 is the initial page state.
            Actions.Add(() => Reset());
            InitActions();
        }

        public ActionCreator ActionCreator { get; private set; } = new ActionCreator();

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
            Slider slider = (Slider)sender;
            if (slider.IsFocused)
            {
                int actionIndex = (int)e.NewValue;
                QueueMoveToAction(actionIndex);
            }
        }

        /// <summary>
        /// Initialize the page actions.
        /// </summary>
        protected abstract void InitActions();

        /// <summary>
        /// Reset the page, undoing any action that was applied to it.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Convenience method for Actions.Add(ActionCreator.Sequential(action))
        /// </summary>
        protected void Seq(Action action)
        {
            Actions.Add(ActionCreator.Sequential(action));
        }

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
            Actions[n].Invoke();
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

        public void NextAction()
        {
            MoveActions(1);
        }

        public void PrevAction()
        {
            MoveActions(-1);
        }

        #region Asynchronous action navigation

        private readonly Stack<int> AsyncMoveCommandsStack = new Stack<int>();

        private CancellationTokenSource ActionNavigationTokenSource;

        public void QueueMoveActions(int n)
        {
            QueueMoveToAction(n + CurrentActionIndex);
        }

        public void QueueMoveToAction(int n)
        {
            lock (AsyncMoveCommandsStack)
            {
                AsyncMoveCommandsStack.Push(n);
            }
        }

        public async void StartActionBufferHandler(int millisecondsPeriod)
        {
            // first stop action thread if one exists
            StopActionBufferHandler();
            ActionNavigationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = ActionNavigationTokenSource.Token;
            Task ClearActionBuffer()
            {
                return Task.Run(() =>
                {
                    int n = CurrentActionIndex;
                    lock (AsyncMoveCommandsStack)
                    {
                        if (AsyncMoveCommandsStack.Count != 0)
                        {
                            n = AsyncMoveCommandsStack.Pop();
                            AsyncMoveCommandsStack.Clear();
                        }
                    }
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    MoveToAction(n);
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    if (elapsedMs != 0)
                    {
                        Console.WriteLine($"'MoveToAction({n})' took {elapsedMs} ms");
                    }
                }, cancellationToken);
            }
            while (true)
            {
                try
                {
                    var delayTask = Task.Delay(millisecondsPeriod, cancellationToken);
                    await ClearActionBuffer();
                    await delayTask;
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        public void StopActionBufferHandler()
        {
            ActionNavigationTokenSource?.Cancel();
        }

        #endregion Asynchronous action navigation

        #endregion IActionNavigation

        #region ICommand

        private ICommand _nextActionCommand; public ICommand NextActionCommand
        {
            get
            {
                if (_nextActionCommand == null) _nextActionCommand = new RelayCommand((arg) => NextAction(), (arg) => CanNextAction);
                return _nextActionCommand;
            }
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

        public bool CanPrevAction
        {
            get
            {
                return CurrentActionIndex != 0;
            }
        }

        #endregion ICommand

        #region INavigation

        private string _name; public string Name
        {
            get
            {
                if (_name == null) _name = "";
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual void Setup()
        {
            StartActionBufferHandler(50);
        }

        public virtual void Teardown()
        {
            StopActionBufferHandler();
            MoveToFirstAction();
        }

        #endregion INavigation

        #region IChaCha

        public ChaChaPresentationViewModel PresentationViewModel { get; private set; }
        public ChaChaVisualizationV2 ChaChaVisualization { get => PresentationViewModel.ChaChaVisualization; }
        public ChaCha.ChaCha ChaCha { get => ChaChaVisualization; }
        public ChaCha.ChaChaSettings Settings { get => (ChaChaSettings)ChaChaVisualization.Settings; }

        #endregion IChaCha
    }
}