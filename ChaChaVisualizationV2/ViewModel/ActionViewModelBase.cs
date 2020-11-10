using Cryptool.Plugins.ChaCha;
using Cryptool.Plugins.ChaChaVisualizationV2.Helper;
using Cryptool.Plugins.ChaChaVisualizationV2.Helper.Validation;
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
    internal abstract class ActionViewModelBase : ViewModelBase, IActionNavigation, IChaCha, INavigation, IActionTag
    {
        public ActionViewModelBase(ChaChaPresentationViewModel chachaPresentationViewModel)
        {
            PresentationViewModel = chachaPresentationViewModel;
            CurrentActionIndex = 0;
            // Make sure that the action at index 0 is the initial page state.
            Actions.Add(() => Reset());
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

        public void HandleActionSliderValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            if (slider.IsFocused)
            {
                int actionIndex = (int)e.NewValue;
                QueueMoveToAction(actionIndex);
            }
        }

        private ValidationRule _actionInputRule; private ValidationRule ActionInputRule
        {
            get
            {
                if (_actionInputRule == null) _actionInputRule = new UserInputValidationRule(TotalActions - 1);
                return _actionInputRule;
            }
        }

        private KeyEventHandler _actionInputHandler; private KeyEventHandler ActionInputHandler
        {
            get
            {
                if (_actionInputHandler == null) _actionInputHandler = UserInputHandler(ActionInputRule, MoveToAction);
                return _actionInputHandler;
            }
        }

        /// <summary>
        /// Handles the event 'user enters value into action textbox next to slider'.
        /// </summary>
        public void HandleUserActionInput(object sender, KeyEventArgs e)
        {
            ActionInputHandler(sender, e);
        }

        /// <summary>
        /// Factory function to create user input handlers.
        /// </summary>
        protected KeyEventHandler UserInputHandler(ValidationRule validationRule, Action<int> HandleEvent)
        {
            return (object sender, KeyEventArgs e) =>
            {
                if (e.Key == Key.Return)
                {
                    string value = ((TextBox)sender).Text;
                    ValidationResult result = validationRule.Validate(value, null);
                    if (result == ValidationResult.ValidResult)
                    {
                        HandleEvent((int.Parse(value)));
                    }
                }
            };
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
                    CurrentUserActionIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Wrapper around internal current action index.
        ///
        /// We use it because we don't want to directly bind the text property
        /// to the internal current action index since we never want to change
        /// the internal current action index ourself. Only the action move handlers should do this.
        /// But we need two-way data binding to enable validation.
        ///
        /// This property will get updated when the internal
        /// current action index changes thus changes to it are reflected
        /// in the user action index.
        /// If the user enters a valid result, the internal current action index
        /// gets updated which as mentioned updates this property.
        /// </summary>
        private int _currentUserActionIndex; public int CurrentUserActionIndex

        {
            get
            {
                return _currentUserActionIndex;
            }
            set
            {
                if (_currentUserActionIndex != value)
                {
                    _currentUserActionIndex = value;
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
            if (CurrentActionIndex != n)
            {
                Reset();
                Actions[n].Invoke();
                CurrentActionIndex = n;
            }
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
                        // Console.WriteLine($"'MoveToAction({n})' took {elapsedMs} ms");
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

        #region IActionTag

        private Dictionary<string, int> ActionTags { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Saves action indices under a "tag" for later retrieval.
        /// This implements "action tagging". We can mark actions with a string
        /// and then retrieve their action index using that string.
        /// One must use this function during action creation and call
        /// it with the index of the action we want to tag.
        /// </summary>
        public void TagAction(string tag, int actionIndex)
        {
            ActionTags.Add(tag, actionIndex);
        }

        /// <summary>
        /// Return the action index of the given action tag.
        /// </summary>
        public int GetTaggedActionIndex(string tag)
        {
            return ActionTags[tag];
        }

        #endregion IActionTag

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
            // Setup actions when navigating to page to have the newest inputs.
            // Fix for TotalKeystreamBlocks = 0 when initializing actions in ctor.
            // TODO(Performance) check if actions have already been initialized or if they need to be reinitialized.
            InitActions();
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