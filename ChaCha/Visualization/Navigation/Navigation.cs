using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Cryptool.Plugins.ChaCha
{
    partial class ChaChaPresentation
    {
        public ActionNavigation Nav = new ActionNavigation();
        private CancellationTokenSource actionNavigationTokenSource;
        private static Button CreateNavigationButton()
        {
            Button b = new Button {Height = 18.75, Width = 32, Margin = new Thickness(1, 0, 1, 0)};
            return b;
        }

        private static Button CreatePrevNavigationButton()
        {
            Button b = CreateNavigationButton();
            b.Content = "<";
            return b;
        }

        private static Button CreateNextNavigationButton()
        {
            Button b = CreateNavigationButton();
            b.Content = ">";
            return b;
        }

        private static TextBox CreateNavigationTextBox()
        {
            TextBox tb = new TextBox { Height = 18.75, Width = 24, Margin = new Thickness(1, 0, 1, 0) };
            return tb;
        }

        private Slider CreateActionNavigationSlider(int totalActions)
        {
            Slider s = new Slider
            {
                Minimum = 0,
                Maximum = totalActions,
                // TODO set width dynamically depending on total actions
                Width = 1000,
                TickFrequency = 1,
                TickPlacement = TickPlacement.None,
                IsSnapToTickEnabled = true,
                VerticalAlignment = VerticalAlignment.Center,
                AutoToolTipPlacement = AutoToolTipPlacement.BottomRight
            };
            
            s.SetBinding(Slider.ValueProperty, new Binding("CurrentActionIndex") { Mode = BindingMode.OneWay });
            void S_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
            {
                // Only execute listener logic if value changed by user
                if(s.IsFocused)
                {
                    int value = (int)s.Value;
                    // Console.WriteLine($"Slider value changed to {value}");
                    if (CurrentActionIndex != value) MoveToActionAsync(value);
                }
            };
            s.ValueChanged += S_ValueChanged;
            return s;
        }
        private class InputActionIndexRule : ValidationRule
        {
            private int _maxActionIndex;
            private int _minActionIndex;
            public InputActionIndexRule(int maxActionIndex)
            {
                _maxActionIndex = maxActionIndex;
            }

            public InputActionIndexRule(int minActionIndex, int maxActionIndex)
            {
                _minActionIndex = minActionIndex;
                _maxActionIndex = maxActionIndex;
            }

            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                int input = 0;

                try
                {
                    input = int.Parse((String)value);
                }
                catch (Exception e)
                {
                    return new ValidationResult(false, $"Illegal characters or {e.Message}");
                }

                if ((input < _minActionIndex) || (input > _maxActionIndex))
                {
                    return new ValidationResult(false,
                        $"Please enter an age in the range: {0}-{_maxActionIndex}.");
                }
                return ValidationResult.ValidResult;
            }
        }
        private TextBox CreateCurrentActionIndexTextBox(int totalActions)
        {
            TextBox current = new TextBox { VerticalAlignment = VerticalAlignment.Center, Width = 30 };
            Binding actionIndexBinding = new Binding("CurrentActionIndexTextBox")
                {Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged};
            ValidationRule inputActionIndexRule = new InputActionIndexRule(totalActions);
            actionIndexBinding.ValidationRules.Add(inputActionIndexRule);
            current.SetBinding(TextBox.TextProperty, actionIndexBinding);

            void HandleKeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Return)
                {
                    string value = ((TextBox)sender).Text;
                    ValidationResult result = inputActionIndexRule.Validate(value, null);
                    if (result == ValidationResult.ValidResult)
                    {
                        MoveToActionAsync(int.Parse(value));
                    }
                }
            }

            current.KeyDown += HandleKeyDown;
            return current;
        }

        private List<Button> _pageButtons = new List<Button>();
        private Button CreatePageButton(string text, int currentPageIndex, int toPageIndex, int width)
        {
            Button b = new Button()
            {
                Content = text,
                Width = width,
                Height = 18.75,
            };
            // since each page has its own dedicated navigation bar, the button for the current page is always bold
            if (currentPageIndex == toPageIndex) b.FontWeight = FontWeights.Bold;
            b.Click += new RoutedEventHandler(MoveToPageClickWrapper(toPageIndex));
            _pageButtons.Add(b);
            return b;
        }

        private void InitPageNavigationBar(Page p)
        {
            int pageIndex = _pages.FindIndex(p_ => p_ == p);
            StackPanel pageNavBar = p.PageNavigationBar;
            pageNavBar.Children.Clear();
            _pageButtons.Clear();
            Button start = CreatePageButton("Start", pageIndex, 0, 32);
            Button overview = CreatePageButton("Overview", pageIndex, 1, 64);
            Button stateMatrixInit = CreatePageButton("State Matrix Initialization", pageIndex, 2, 160);
            Button keystream = CreatePageButton("Keystream Generation", pageIndex, 3, 160);
            pageNavBar.Children.Add(start);
            pageNavBar.Children.Add(overview);
            pageNavBar.Children.Add(stateMatrixInit);
            pageNavBar.Children.Add(keystream);
        }

        private TextBox CreateKeystreamBlockTextBox(int totalKeystreamBlocks)
        {
            TextBox current = CreateNavigationTextBox();
            Binding actionIndexBinding = new Binding("CurrentKeystreamBlock")
            { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            ValidationRule inputActionIndexRule = new InputActionIndexRule(1, totalKeystreamBlocks);
            actionIndexBinding.ValidationRules.Add(inputActionIndexRule);
            current.SetBinding(TextBox.TextProperty, actionIndexBinding);

            void HandleKeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Return)
                {
                    string value = ((TextBox)sender).Text;
                    ValidationResult result = inputActionIndexRule.Validate(value, null);
                    if (result == ValidationResult.ValidResult)
                    {
                        // +2 because keystream pages start at index 3, so if we get the value 1
                        // we want to go to page index 3 etc.
                        MoveToPage(2 + int.Parse(value));
                    }
                }
            }

            current.KeyDown += HandleKeyDown;
            return current;
        }

        public int CurrentKeystreamBlock { get; set; } = 1;

        private void InitKeystreamNavigation(Page p, int totalKeystreamBlocks, int totalRounds)
        {
            // Assume that general page navigation bar has already been initialized
            StackPanel pageNavBar = p.PageNavigationBar;

            Grid keystreamBlockGrid = new Grid() { Margin = new Thickness(0, -15, 0, 0) };
            keystreamBlockGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(18.75) });
            keystreamBlockGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(18.75) });
            TextBlock keystreamLabel = new TextBlock() { Text = "Keystream Block", HorizontalAlignment = HorizontalAlignment.Center };
            StackPanel keystreamBlockBottomRow = new StackPanel() { Orientation = Orientation.Horizontal };
            Grid.SetRow(keystreamLabel, 0);
            Grid.SetRow(keystreamBlockBottomRow, 1);
            Button previousKeystreamBlock = CreatePrevNavigationButton();
            TextBox currentKeystreamBlock = CreateKeystreamBlockTextBox(totalKeystreamBlocks);
            TextBlock keystreamDelimiter = new TextBlock() { Text = "/" };
            TextBlock totalKeystreamBlockLabel = new TextBlock() { Text = totalKeystreamBlocks.ToString() };
            Button nextKeystreamBlock = CreateNextNavigationButton();
            keystreamBlockBottomRow.Children.Add(previousKeystreamBlock);
            keystreamBlockBottomRow.Children.Add(currentKeystreamBlock);
            keystreamBlockBottomRow.Children.Add(keystreamDelimiter);
            keystreamBlockBottomRow.Children.Add(totalKeystreamBlockLabel);
            keystreamBlockBottomRow.Children.Add(nextKeystreamBlock);
            keystreamBlockGrid.Children.Add(keystreamLabel);
            keystreamBlockGrid.Children.Add(keystreamBlockBottomRow);
            pageNavBar.Children.Add(keystreamBlockGrid);


            Grid roundGrid = new Grid() { Margin = new Thickness(0, -15, 0, 0) };
            roundGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(18.75) });
            roundGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(18.75) });
            TextBlock roundLabel = new TextBlock() { Text = "Round", HorizontalAlignment = HorizontalAlignment.Center };
            StackPanel roundBottomRow = new StackPanel() { Orientation = Orientation.Horizontal };
            Grid.SetRow(roundLabel, 0);
            Grid.SetRow(roundBottomRow, 1);
            Button previousRound = CreatePrevNavigationButton();
            TextBox currentRound = CreateNavigationTextBox();
            TextBlock delimiterRound = new TextBlock() { Text = "/" };
            TextBlock totalRoundsLabel = new TextBlock() { Text = totalRounds.ToString() };
            Button nextRound = CreateNextNavigationButton();
            roundBottomRow.Children.Add(previousRound);
            roundBottomRow.Children.Add(currentRound);
            roundBottomRow.Children.Add(delimiterRound);
            roundBottomRow.Children.Add(totalRoundsLabel);
            roundBottomRow.Children.Add(nextRound);
            roundGrid.Children.Add(roundLabel);
            roundGrid.Children.Add(roundBottomRow);
            pageNavBar.Children.Add(roundGrid);


            Grid quarterroundGrid = new Grid() { Margin = new Thickness(0, -15, 0, 0) };
            quarterroundGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(18.75) });
            quarterroundGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(18.75) });
            TextBlock quarterroundLabel = new TextBlock() { Text = "Quarterround", HorizontalAlignment = HorizontalAlignment.Center };
            StackPanel quarterroundBottomRow = new StackPanel() { Orientation = Orientation.Horizontal };
            Grid.SetRow(quarterroundLabel, 0);
            Grid.SetRow(quarterroundBottomRow, 1);
            Button previousQuarterround = CreatePrevNavigationButton();
            TextBox currentQuarterround = CreateNavigationTextBox();
            TextBlock delimiterQuarterround = new TextBlock() { Text = "/" };
            TextBlock totalQuarterRoundsLabel = new TextBlock() { Text = "4" };
            Button nextQuarterround = CreateNextNavigationButton();
            quarterroundBottomRow.Children.Add(previousQuarterround);
            quarterroundBottomRow.Children.Add(currentQuarterround);
            quarterroundBottomRow.Children.Add(delimiterQuarterround);
            quarterroundBottomRow.Children.Add(totalQuarterRoundsLabel);
            quarterroundBottomRow.Children.Add(nextQuarterround);
            quarterroundGrid.Children.Add(quarterroundLabel);
            quarterroundGrid.Children.Add(quarterroundBottomRow);
            pageNavBar.Children.Add(quarterroundGrid);
        }

        private void InitActionSliderNavigationBar(StackPanel actionNavBar, int totalActions)
        {
            actionNavBar.Children.Clear();
            Slider actionSlider = CreateActionNavigationSlider(totalActions);
            TextBox current = CreateCurrentActionIndexTextBox(totalActions);
            TextBlock delimiter = new TextBlock() { VerticalAlignment = VerticalAlignment.Center, Text = "/" };
            TextBlock total = new TextBlock() { VerticalAlignment = VerticalAlignment.Center, Text = totalActions.ToString() };
            actionNavBar.Children.Add(PrevButton());
            actionNavBar.Children.Add(actionSlider);
            Button next = NextButton();
            next.Margin = new Thickness(1, 0, 3, 0);
            actionNavBar.Children.Add(next);
            actionNavBar.Children.Add(current);
            actionNavBar.Children.Add(delimiter);
            actionNavBar.Children.Add(total);
        }

        private void InitActionNavigationBar(Page p)
        {
            int totalActions = p.ActionFrames;
            if (totalActions > 0)
            {
                //InitActionButtonNavigationBar(p.ActionNavigationBar, totalActions);
                InitActionSliderNavigationBar(p.ActionNavigationBar, totalActions);
            }
        }
       
        // List with pages in particular order to implement page navigation + their page actions
        private readonly List<Page> _pages = new List<Page>();
        private void AddPage(Page page)
        {
            _pages.Add(page);
        }

        // Initializes page by wrapping the init action functions with the navigation interface methods and calling them.
        private void InitPage(Page p)
        {
            foreach (PageAction pageAction in p.InitActions)
            {
                pageAction.Exec();
            }

            if (p.ActionFrames > 0)
            {
                StartActionBufferHandler(50);
            }
        }

        private void MovePages(int n)
        {
            if (n < 0)
            {
                for (int i = 0; i < Math.Abs(n); ++i)
                {
                    PrevPage_Click(null, null);
                }
            }
            else
            {
                for (int i = 0; i < Math.Abs(n); ++i)
                {
                    NextPage_Click(null, null);
                }
            }
        }

        private void MoveToPage(int n)
        {
            MovePages(n - CurrentPageIndex);
        }

        const int START_VISUALIZATION_ON_PAGE_INDEX = 0;
        private void InitStaticVisualization()
        {
            // Static visualization means that only the first page is added.
            // Other pages are added when execution has finished and thus all variables needed exist.
            _pages.Clear();
            AddPage(Page.LandingPage(this));
            CollapseAllPagesExpect(0);
        }

        private void InitExecutableVisualization()
        {
            _pages.Clear();
            AddPage(Page.LandingPage(this));
            AddPage(Page.WorkflowPage(this));
            AddPage(Page.StateMatrixPage(this));
            for (int i = 0; i < KeystreamBlocksNeeded; ++i)
            {
                AddPage(Page.KeystreamBlockGenPage(this, (ulong)i + 1));
            }
            CollapseAllPagesExpect(START_VISUALIZATION_ON_PAGE_INDEX);
            for (int i = 0; i < _pages.Count; ++i)
            {
                Page p = _pages[i];
                InitPageNavigationBar(p);
                if (i >= 3) InitKeystreamNavigation(p, KeystreamBlocksNeeded, Rounds);
            }
            InitActionNavigationBar(CurrentPage);
        }

        private UIElement[] GetRawPages()
        {
            return new UIElement[] { UILandingPage, UIWorkflowPage, UIStateMatrixPage, UIKeystreamBlockGenPage };
        }

        // useful for development: setting pages visible for development purposes does not infer with execution
        private void CollapseAllPagesExpect(int pageIndex)
        {
            UIElement[] pages = GetRawPages();
            for (int i = 0; i < pages.Length; ++i)
            {
                if (i != pageIndex)
                {
                    pages[i].Visibility = Visibility.Collapsed;
                }
            }
            pages[pageIndex].Visibility = Visibility.Visible;
        }

        // Calls FinishPageAction if page action used navigation interface methods.
        [Obsolete("This method has been deprecated because it was incompatible with caching.", true)]
        private void WrapExecWithNavigation(PageAction pageAction)
        {
            pageAction.Exec();
            if (Nav.SaveStateHasBeenCalled)
            {
                Nav.FinishPageAction();
            }
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            StopActionBufferHandler();
            CurrentPage.Visibility = Visibility.Collapsed;
            ResetPageActions();
            CurrentPageIndex--;
            CurrentPage.Visibility = Visibility.Visible;
            InitPage(CurrentPage);
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            StopActionBufferHandler();
            CurrentPage.Visibility = Visibility.Collapsed;
            ResetPageActions();
            CurrentPageIndex++;
            CurrentPage.Visibility = Visibility.Visible;
            InitPage(CurrentPage);

        }

        private Action<object, RoutedEventArgs> MoveToPageClickWrapper(int n)
        {
            void MoveToPageClick(object sender, RoutedEventArgs e)
            {
                MoveToPage(n);
            }

            return MoveToPageClick;
        }

        private int _currentPageIndex = 0;
        private int _currentActionIndex = 0;
        // action index value for TextBox.
        // Prevents direct write-access to actual current action index value while still being able to read from it.
        private int _currentActionIndexTextBox = 0;

        private bool _executionFinished = false;
        private bool _inputValid = false;

        private int CurrentPageIndex
        {
            get => _currentPageIndex;
            set
            {
                if (value == _currentPageIndex) return;
                _currentPageIndex = value;
                CurrentActionIndex = 0;
                OnPropertyChanged("CurrentPageIndex");
                OnPropertyChanged("CurrentPage");
                OnPropertyChanged("NextPageIsEnabled");
                OnPropertyChanged("PrevPageIsEnabled");
                OnPropertyChanged("NextRoundIsEnabled");
                OnPropertyChanged("PrevRoundIsEnabled");
                InitActionNavigationBar(CurrentPage);
            }
        }
        public int CurrentActionIndex
        {
            get => _currentActionIndex;
            set
            {
                _currentActionIndex = value;
                // Console.WriteLine($"CurrentActionIndex = {value}");
                CurrentActionIndexTextBox = value;
                OnPropertyChanged("CurrentActionIndex");
                OnPropertyChanged("CurrentActions");
                OnPropertyChanged("NextActionIsEnabled");
                OnPropertyChanged("PrevActionIsEnabled");
                OnPropertyChanged("NextRoundIsEnabled");
                OnPropertyChanged("PrevRoundIsEnabled");
            }
        }

        public int CurrentActionIndexTextBox
        {
            get => _currentActionIndexTextBox;
            set
            {
                _currentActionIndexTextBox = value;
                OnPropertyChanged("CurrentActionIndexTextBox");

            }
        }

        private Page CurrentPage => _pages.Count == 0 ? Page.LandingPage(this) : _pages[CurrentPageIndex];

        private PageAction[] CurrentActions => CurrentPage.Actions;

        public bool ExecutionFinished
        {
            get => _executionFinished;
            set
            {
                InitExecutableVisualization();
                MovePages(START_VISUALIZATION_ON_PAGE_INDEX - CurrentPageIndex);
                _executionFinished = value;
                OnPropertyChanged("NextPageIsEnabled");
                OnPropertyChanged("PrevPageIsEnabled");
                OnPropertyChanged("NextActionIsEnabled");
                OnPropertyChanged("PrevActionIsEnabled");
                OnPropertyChanged("NavigationEnabled");
            }
        }

        public bool InputValid
        {
            get => _inputValid;
            set
            {
                _inputValid = value;
                OnPropertyChanged("NextPageIsEnabled");
                OnPropertyChanged("PrevPageIsEnabled");
                OnPropertyChanged("NextActionIsEnabled");
                OnPropertyChanged("PrevActionIsEnabled");
                OnPropertyChanged("NavigationEnabled");
            }
        }

        // next action is only enabled if currentActionIndex is not already pointing outside of actionFrames array since we increase it after each action.
        // For example, if there are two action frames, we start with currentActionIndex = 0 and increase it each click _after_ we have processed the index
        // to retrieve the actions we need to take. After two clicks, we are at currentActionIndex = 2 which is the first invalid index.
        public bool NextActionIsEnabled => CurrentPage.ActionFrames > 0 &&
                                           CurrentActionIndex != CurrentPage.ActionFrames && ExecutionFinished;

        public bool PrevActionIsEnabled => CurrentPage.ActionFrames > 0 && CurrentActionIndex != 0;

        public bool NextRoundIsEnabled => CurrentRoundIndex != 20;

        public bool PrevRoundIsEnabled => CurrentRoundIndex >= 1;

        private int _currentRoundIndex = 0;
        public int CurrentRoundIndex
        {
            get
            {
                return _currentRoundIndex;
            }
            set
            {
                _currentRoundIndex = value;
                OnPropertyChanged("CurrentRoundIndex");
                OnPropertyChanged("NextRoundIsEnabled");
                OnPropertyChanged("PrevRoundIsEnabled");
            }
        }

        public bool NavigationEnabled => InputValid && ExecutionFinished;

        private Button PrevButton()
        {
            Button b = CreateNavigationButton();
            b.SetBinding(Button.IsEnabledProperty, new Binding("PrevActionIsEnabled"));
            b.Click += (sender, e) => MoveActionsAsync(-1);
            b.Content = "<";
            return b;
        }

        private Button NextButton()
        {
            Button b = CreateNavigationButton();
            b.SetBinding(Button.IsEnabledProperty, new Binding("NextActionIsEnabled"));
            b.Click += (sender, e) => MoveActionsAsync(1);
            b.Content = ">";
            return b;
        }

        private void ResetPageActions()
        {
            MoveToAction(0);
            Debug.Assert(CurrentActionIndex == 0);
            // Also undo init actions.
            // Reverse because order (may) matter. Undoing should be done in a FIFO queue!
            foreach (PageAction pageAction in CurrentPage.InitActions.Reverse())
            {
                pageAction.Undo();
            }
        }

        /**
         * Move n actions back / forward.
         *
         * Implements action navigation with relative value.
         * Action is not immediately executed but buffered and regularly executed by a different thread.
         */
        private void MoveActionsAsync(int n)
        {
            MoveToActionAsync(n + CurrentActionIndex);
        }

        /**
         * Move to Action with given index.
         *
         * Implements action navigation with absolute value.
         * Action is not immediately executed but buffered and regularly executed by a different thread.
         */
        private readonly Stack<int> _moveToActionIndicesStack = new Stack<int>();
        private void MoveToActionAsync(int n)
        {
            // Console.WriteLine($"MoveToActionAsync({n})");
            lock (_moveToActionIndicesStack)
            {
                _moveToActionIndicesStack.Push(n);
                // Console.WriteLine($"Pushed {n} onto action stack.");
            }
        }

        private void PrevActionClick(object sender, RoutedEventArgs e)
        {
            CurrentActionIndex--;
            CurrentPage.Actions[CurrentActionIndex].Undo();
        }
        private void NextActionClick(object sender, RoutedEventArgs e)
        {
            CurrentPage.Actions[CurrentActionIndex].Exec();
            CurrentActionIndex++;
        }
        private void ExecuteCache(int n)
        {
            // Console.WriteLine($"ExecuteCache({n})");
            Cache.Get(n).Exec();
            CurrentActionIndex = n;
            _moveToActionIndicesStack.Clear();
            // Console.WriteLine("Cleared action stack");
        }

        private ActionCache Cache
        {
            get
            {
                return CurrentPage.Cache;
            }
        }

        /**
         * Move to Action with given index.
         *
         * Implements action navigation with absolute value.
         * Action is immediately executed.
         */
        private void MoveToAction(int n)
        {
            int relative = n - CurrentActionIndex;
            // if (relative != 0) Console.WriteLine($"MoveToAction({n}) - CurrentActionIndex: {CurrentActionIndex}, relative: {relative}");
            // TODO Implement "relative caching"
            //   (go first to nearest cache entry in constant time and then from there to destination index)
            if (relative != 0)
            {
                if(Cache.NotEmpty)
                {
                    int closestCache = Cache.GetClosestCache(n);
                    int distanceCacheToDestination = Math.Abs(closestCache - n);
                    // Console.WriteLine($"closestCache: {closestCache}, distanceCacheToDestination: {distanceCacheToDestination}");
                    if (distanceCacheToDestination < Math.Abs(relative))
                    {
                        ExecuteCache(closestCache);
                        relative = n - closestCache;
                        // Console.WriteLine($"new relative: {relative}");
                    }
                    // Console.WriteLine($"closestCache: {closestCache}, n: {n}, relative: {relative}");
                }
            }
            if (relative < 0)
            {
                for (int i = 0; i < Math.Abs(relative); ++i)
                {
                    PrevActionClick(null, null);
                }
            }
            else
            {
                for (int i = 0; i < Math.Abs(relative); ++i)
                {
                    NextActionClick(null, null);
                }
            }
        }

        /**
         * Starts the handler which periodically runs all queued up actions to increase performance by using a cache. 
         *
         * Implemented as follows:
         *   1. Sum up all relative action indices to get the resulting relative action index.
         *        For example, if we have [-1, -2, 5] queued, we would first move one action back, then 5 and then 3 forward.
         *        This is the same as just moving 2 actions forward.
         *   2. Calculate nearest cached action index.
         *   3. move to nearest cached action index in constant time from anywhere.
         *   4. Move to destination.
         */
        private async void StartActionBufferHandler(int millisecondsPeriod)
        {
            // first stop action thread if one exists
            StopActionBufferHandler();
            actionNavigationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = actionNavigationTokenSource.Token;
            Task ClearActionBuffer()
            {
                return Task.Run(() =>
                {
                    int n = CurrentActionIndex;
                    lock (_moveToActionIndicesStack)
                    {
                        if (_moveToActionIndicesStack.Count != 0)
                        {
                            n = _moveToActionIndicesStack.Pop();
                            _moveToActionIndicesStack.Clear();
                            // Console.WriteLine($"Popped {n} from action stack and cleared stack");
                        }
                    }
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    this.Dispatcher.Invoke(() => MoveToAction(n));
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
                catch (TaskCanceledException e)
                {
                    break;
                }
            }
        }

        private void StopActionBufferHandler()
        {
            actionNavigationTokenSource?.Cancel();
        }

        public static int GetLabeledPageActionIndex(string label, PageAction[] actions)
        {
            return Array.FindIndex(actions, pageAction => Array.FindIndex(pageAction.Labels, actionlabel => actionlabel == label) != -1);
        }

        private void PrevRound_Click(object sender, RoutedEventArgs e)
        {
            int startIndex = CurrentActionIndex;
            int endIndex = GetLabeledPageActionIndex(KeystreamBlockGenPage.RoundStartLabel(CurrentRoundIndex), CurrentActions) + 1;
            if(startIndex == endIndex)
            {
                // We are currently at the start of a round. Go to previous round.
                endIndex = GetLabeledPageActionIndex(KeystreamBlockGenPage.RoundStartLabel(CurrentRoundIndex - 1), CurrentActions) + 1;
            }
            Debug.Assert(startIndex > endIndex, $"startIndex ({startIndex}) should be higher than endIndex ({endIndex}) when clicking on previous round");
            MoveToActionAsync(endIndex);
        }

        private void NextRound_Click(object sender, RoutedEventArgs e)
        {
            int startIndex = CurrentActionIndex;
            int endIndex = GetLabeledPageActionIndex(KeystreamBlockGenPage.RoundStartLabel(CurrentRoundIndex + 1), CurrentActions) + 1;
            Debug.Assert(startIndex < endIndex, $"startIndex ({startIndex}) should be lower than endIndex ({endIndex}) when clicking on next round");
            MoveToActionAsync(endIndex);
        }

        private void QR_Click(int qrLabelIndex)
        {
            /*
             * We need to map the QR Label Index which is between 1 and 8 and is corresponds to the row of the pressed Quarterround button to
             * the appropriate page action label.
             * The page action labels are in this format: _QR_ACTION_LABEL_{QR_ROUND}_{ROUND} where QR_ROUND is between 1 - 4 and ROUND from 1 - 20
             * This means, for example, for the first quarterround of round 3, the label is _QR_ACTION_LABEL_1_3.
             */
            int qrLabelSearchIndex = -1;
            int roundLabelSearchIndex = -1;
            if(qrLabelIndex <= 4)
            {
                qrLabelSearchIndex = qrLabelIndex;
                if (CurrentRoundIndex % 2 == 1 || CurrentRoundIndex == 0)
                {
                    roundLabelSearchIndex = CurrentRoundIndex != 0 ? CurrentRoundIndex : 1;
                }
                else
                {
                    roundLabelSearchIndex = CurrentRoundIndex - 1;
                }
            }
            if(qrLabelIndex > 4)
            {
                qrLabelSearchIndex = (qrLabelIndex - 1) % 4 + 1;
                if (CurrentRoundIndex % 2 == 1 || CurrentRoundIndex == 0)
                {
                    roundLabelSearchIndex = CurrentRoundIndex + 1;
                }
                else
                {
                    roundLabelSearchIndex = CurrentRoundIndex;
                }
            }
            string searchLabel = KeystreamBlockGenPage.QuarterroundStartLabelWithRound(qrLabelSearchIndex, roundLabelSearchIndex);
            int qrActionIndex = GetLabeledPageActionIndex(searchLabel, CurrentActions) + 1;
            MoveToActionAsync(qrActionIndex);
        }
        private void QR1_Click(object sender, RoutedEventArgs e)
        {
            QR_Click(1);
        }
        private void QR2_Click(object sender, RoutedEventArgs e)
        {
            QR_Click(2);
        }
        private void QR3_Click(object sender, RoutedEventArgs e)
        {
            QR_Click(3);
        }
        private void QR4_Click(object sender, RoutedEventArgs e)
        {
            QR_Click(4);
        }
        private void QR5_Click(object sender, RoutedEventArgs e)
        {
            QR_Click(5);
        }
        private void QR6_Click(object sender, RoutedEventArgs e)
        {
            QR_Click(6);
        }
        private void QR7_Click(object sender, RoutedEventArgs e)
        {
            QR_Click(7);
        }
        private void QR8_Click(object sender, RoutedEventArgs e)
        {
            QR_Click(8);
        }
    }
}
