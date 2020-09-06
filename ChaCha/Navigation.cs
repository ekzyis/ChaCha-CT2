using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cryptool.Plugins.ChaCha
{
    partial class ChaChaPresentation
    {

        ActionNavigation nav = new ActionNavigation();

        #region navigation bar

        private Button CreateNavigationButton()
        {
            Button b = new Button();
            b.Height = 18.75; b.Width = 32;
            b.Margin = new Thickness(1, 0, 1, 0);
            return b;
        }

        private TextBlock CreateNavigationTextBlock(int index, int activeIndex, string bindingElementName)
        {
            TextBlock tb = new TextBlock();
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            Binding binding = new Binding();
            binding.ElementName = bindingElementName;
            binding.Path = new PropertyPath("FontSize");
            tb.SetBinding(TextBlock.FontSizeProperty, binding);
            tb.Text = (index + 1).ToString();
            if(activeIndex == index)
            {
                tb.FontWeight = FontWeights.Bold;
            }
            return tb;
        }

        private TextBlock CreateNavBarLabel(string bindingElementName, string text)
        {
            TextBlock tb = new TextBlock();
            tb.Name = bindingElementName;
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            tb.FontSize = 10.0;
            tb.Margin = new Thickness(0, 0, 1, 0);
            tb.Text = text;
            return tb;
        }

        private string _PAGELABELNAME = "UINavBarPageLabel";
        private Button CreatePageNavigationButton(int index)
        {
            Button b = CreateNavigationButton();
            b.Click += new RoutedEventHandler(MoveToPageClickWrapper(index));
            TextBlock tb = CreateNavigationTextBlock(index, CurrentPageIndex, _PAGELABELNAME);
            b.Content = tb;
            return b;
        }

        private string _ACTIONLABELNAME = "UINavBarActionLabel";
        private Button CreateActionNavigationButton(int index)
        {
            Button b = CreateNavigationButton();
            b.Click += new RoutedEventHandler(MoveToActionClickWrapper(index));
            TextBlock tb = CreateNavigationTextBlock(index, CurrentActionIndex, _ACTIONLABELNAME);
            b.Content = tb;
            return b;
        }

        // this must be called after all pages have been added
        private void InitPageNavigationBar(Page p)
        {
            StackPanel pageNavBar = p.PageNavigationBar;
            pageNavBar.Children.Clear();
            pageNavBar.Children.Add(CreateNavBarLabel(_PAGELABELNAME, "Pages:"));
            for (int i = 0; i < _pages.Count; ++i)
            {
                pageNavBar.Children.Add(CreatePageNavigationButton(i));
            }
        }
        private void _InitActionButtonNavigationBar(StackPanel actionNavBar, int totalActions)
        {
            actionNavBar.Children.Clear();
            actionNavBar.Children.Add(CreateNavBarLabel(_ACTIONLABELNAME, "Actions:"));
            int startIndex = CurrentActionIntervalIndex * _ACTION_INTERVAL_SIZE;
            int endIndex = Math.Min(totalActions, (CurrentActionIntervalIndex + 1) * _ACTION_INTERVAL_SIZE);
            void PrevInterval_Click(object sender, RoutedEventArgs e)
            {
                CurrentActionIntervalIndex = Math.Max(0, CurrentActionIntervalIndex - 1);
            }
            void NextInterval_Click(object sender, RoutedEventArgs e)
            {
                CurrentActionIntervalIndex = Math.Min(CurrentPage.ActionFrames / _ACTION_INTERVAL_SIZE, CurrentActionIntervalIndex + 1);
            }
            for (int i = startIndex; i <= endIndex; ++i)
            {
                actionNavBar.Children.Add(CreateActionNavigationButton(i));
            }
            if (totalActions > _ACTION_INTERVAL_SIZE)
            {
                Button prevInterval = CreateNavigationButton();
                prevInterval.Click += PrevInterval_Click;
                prevInterval.Content = "<";
                prevInterval.IsEnabled = CurrentActionIntervalIndex != 0;
                Button nextInterval = CreateNavigationButton();
                nextInterval.Click += NextInterval_Click;
                nextInterval.Content = ">";
                nextInterval.IsEnabled = CurrentActionIntervalIndex < totalActions / _ACTION_INTERVAL_SIZE;
                actionNavBar.Children.Insert(1, prevInterval);
                actionNavBar.Children.Add(nextInterval);
            }
        }
        private void _InitActionSliderNavigationBar(StackPanel actionNavBar, int totalActions)
        {

            actionNavBar.Children.Clear();
            Slider s = new Slider();
            s.Minimum = 0;
            s.Maximum = totalActions;
            // TODO set width dynamically depending on total actions
            s.Width = 1000;
            s.TickFrequency = 1;
            s.TickPlacement = TickPlacement.TopLeft;
            s.IsSnapToTickEnabled = true;
            s.TickFrequency = 1;
            s.Value = CurrentActionIndex;
            s.AutoToolTipPlacement = AutoToolTipPlacement.BottomRight;
            void S_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
            {
                MoveToAction((int)s.Value);
            };
            s.ValueChanged += S_ValueChanged;
            TextBlock current = new TextBlock();
            Binding b = new Binding("Value");
            b.Source = s;
            current.SetBinding(TextBlock.TextProperty, b);
            TextBlock delimiter = new TextBlock();
            delimiter.Text = "/";
            TextBlock total = new TextBlock();
            total.Text = totalActions.ToString();
            actionNavBar.Children.Add(s);
            actionNavBar.Children.Add(current);
            actionNavBar.Children.Add(delimiter);
            actionNavBar.Children.Add(total);
        }
        private void InitActionNavigationBar(Page p)
        {
            int totalActions = p.ActionFrames;
            if (totalActions > 0)
            {
                //_InitActionButtonNavigationBar(p.ActionNavigationBar, totalActions);
                _InitActionSliderNavigationBar(p.ActionNavigationBar, totalActions);
            }
        }

        #endregion

        #region page navigation

        #region classes, structs and methods related page navigation

        public class PageAction
        {
            private List<Action> _exec = new List<Action>();
            private List<Action> _undo = new List<Action>();

            public PageAction(Action exec, Action undo, string label = "")
            {
                _exec.Add(exec);
                _undo.Add(undo);
                Label = label;
            }
            public void exec()
            {
                foreach (Action a in _exec)
                {
                    a();
                }
            }
            public void undo()
            {
                foreach (Action a in _undo)
                {
                    a();
                }
            }
            public string Label { get; set; }
            public void AddToExec(Action toAdd)
            {
                _exec.Add(toAdd);
            }
            public void AddToUndo(Action toAdd)
            {
                _undo.Add(toAdd);
            }

            public void Add(PageAction toAdd)
            {
                _exec.Add(toAdd.exec);
                _undo.Add(toAdd.undo);
            }
        }
        class Page
        {
            public Page(ContentControl pageElement)
            {
                _page = pageElement;
            }
            private readonly ContentControl _page; // the visual tree element which contains the page - the Visibility of this element will be set to Collapsed / Visible when going to next / previous page.
            private readonly List<PageAction> _pageActions = new List<PageAction>();
            private readonly List<PageAction> _pageInitActions = new List<PageAction>();
            public int ActionFrames
            {
                get
                {
                    return _pageActions.Count;
                }
            }
            public PageAction[] Actions
            {
                get
                {
                    return _pageActions.ToArray();
                }
            }
            public void AddAction(params PageAction[] pageActions)
            {
                foreach (PageAction pageAction in pageActions)
                {
                    _pageActions.Add(pageAction);
                }
            }
            public PageAction[] InitActions
            {
                get
                {
                    return _pageInitActions.ToArray();
                }
            }
            public void AddInitAction(PageAction pageAction)
            {
                _pageInitActions.Add(pageAction);
            }
            public Visibility Visibility
            {
                get
                {
                    return _page.Visibility;
                }
                set
                {
                    _page.Visibility = value;
                }
            }
            public StackPanel PageNavigationBar
            {
                get
                {
                    bool b = _page.ApplyTemplate();
                    return (StackPanel)_page.Template.FindName("PageNavBar", _page);
                }
            }
            public StackPanel ActionNavigationBar
            {
                get
                {
                    bool b = _page.ApplyTemplate();
                    return (StackPanel)_page.Template.FindName("ActionNavBar", _page);
                }
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
                WrapExecWithNavigation(pageAction);
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

        const int __START_VISUALIZATION_ON_PAGE_INDEX__ = 0;
        private void InitVisualization()
        {
            _pages.Clear();
            AddPage(LandingPage());
            AddPage(WorkflowPage());
            AddPage(StateMatrixPage());
            AddPage(KeystreamBlockGenPage());
            CollapseAllPagesExpect(__START_VISUALIZATION_ON_PAGE_INDEX__);
            InitPageNavigationBar(CurrentPage);
            InitActionNavigationBar(CurrentPage);
        }

        // useful for development: setting pages visible for development purposes does not infer with execution
        private void CollapseAllPagesExpect(int pageIndex)
        {
            for (int i = 0; i < _pages.Count; ++i)
            {
                if (i != pageIndex)
                {
                    _pages[i].Visibility = Visibility.Collapsed;
                }
            }
            _pages[pageIndex].Visibility = Visibility.Visible;
        }

        // Calls FinishPageAction if page action used navigation interface methods.
        private void WrapExecWithNavigation(PageAction pageAction)
        {
            pageAction.exec();
            if (nav.SaveStateHasBeenCalled)
            {
                nav.FinishPageAction();
            }
        }

        #endregion

        #region page navigation click handlers
        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage.Visibility = Visibility.Collapsed;
            ResetPageActions();
            CurrentPageIndex--;
            CurrentPage.Visibility = Visibility.Visible;
            InitPage(CurrentPage);
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage.Visibility = Visibility.Collapsed;
            ResetPageActions();
            CurrentPageIndex++;
            CurrentPage.Visibility = Visibility.Visible;
            InitPage(CurrentPage);
        }

        private Action<object, RoutedEventArgs> MoveToPageClickWrapper(int n)
        {
            Action<object, RoutedEventArgs> moveToPage_Click = (sender, e) => { MoveToPage(n); };
            return moveToPage_Click;
        }

        private Action<object, RoutedEventArgs> MoveToActionClickWrapper(int n)
        {
            Action<object, RoutedEventArgs> moveToAction_Click = (sender, e) => { MoveToAction(n); };
            return moveToAction_Click;
        }
        #endregion

        #region variables related to page navigation

        private int _currentPageIndex = 0;
        private int _currentActionIndex = 0;
        private int _currentActionIntervalIndex = 0;
        private int _ACTION_INTERVAL_SIZE = 20;

        private bool _executionFinished = false;
        private bool _inputValid = false;

        private int CurrentPageIndex
        {
            get
            {
                return _currentPageIndex;
            }
            set
            {
                if (value != _currentPageIndex)
                {
                    _currentPageIndex = value;
                    CurrentActionIndex = 0;
                    CurrentActionIntervalIndex = 0;
                    OnPropertyChanged("CurrentPageIndex");
                    OnPropertyChanged("CurrentPage");
                    OnPropertyChanged("NextPageIsEnabled");
                    OnPropertyChanged("PrevPageIsEnabled");
                    OnPropertyChanged("NextRoundIsEnabled");
                    OnPropertyChanged("PrevRoundIsEnabled");
                    InitPageNavigationBar(CurrentPage);
                    InitActionNavigationBar(CurrentPage);
                }
            }
        }
        private int CurrentActionIndex
        {
            get
            {
                return _currentActionIndex;
            }
            set
            {
                _currentActionIndex = value;
                OnPropertyChanged("CurrentActionIndex");
                OnPropertyChanged("CurrentActions");
                OnPropertyChanged("NextActionIsEnabled");
                OnPropertyChanged("PrevActionIsEnabled");
                OnPropertyChanged("NextRoundIsEnabled");
                OnPropertyChanged("PrevRoundIsEnabled");
                //InitActionNavigationBar(CurrentPage);
            }
        }
        private int CurrentActionIntervalIndex
        {
            get
            {
                return _currentActionIntervalIndex;
            }
            set
            {
                _currentActionIntervalIndex = value;
                InitActionNavigationBar(CurrentPage);
            }
        }
        private void UpdateCurrentActionIntervalIndex()
        {
            CurrentActionIntervalIndex = CurrentActionIndex / _ACTION_INTERVAL_SIZE;
        }

        private Page CurrentPage
        {
            get
            {
                if (_pages.Count == 0)
                {
                    return LandingPage();
                }
                return _pages[CurrentPageIndex];
            }
        }

        private PageAction[] CurrentActions
        {
            get
            {
                return CurrentPage.Actions;
            }
        }

        public int MaxPageIndex
        {
            get
            {
                return _pages.Count - 1;
            }
        }

        public bool ExecutionFinished
        {
            get
            {
                return _executionFinished;
            }
            set
            {
                // reload pages to use new variables
                InitVisualization();
                MovePages(__START_VISUALIZATION_ON_PAGE_INDEX__ - CurrentPageIndex);
                _executionFinished = value;
                OnPropertyChanged("NextPageIsEnabled");
                OnPropertyChanged("PrevPageIsEnabled");
                OnPropertyChanged("NextActionIsEnabled");
                OnPropertyChanged("PrevActionIsEnabled");
            }
        }

        public bool InputValid
        {
            get
            {
                return _inputValid;
            }
            set
            {
                _inputValid = value;
                OnPropertyChanged("NextPageIsEnabled");
                OnPropertyChanged("PrevPageIsEnabled");
                OnPropertyChanged("NextActionIsEnabled");
                OnPropertyChanged("PrevActionIsEnabled");
            }
        }

        public bool NextPageIsEnabled
        {
            get
            {
                return CurrentPageIndex != MaxPageIndex && ExecutionFinished && InputValid;
            }
        }
        public bool PrevPageIsEnabled
        {
            get
            {
                return CurrentPageIndex != 0 && ExecutionFinished && InputValid;
            }
        }

        #endregion

        #endregion

        #region action navigation

        #region variables related to action navigation

        public bool NextActionIsEnabled
        {
            get
            {
                // next action is only enabled if currentActionIndex is not already pointing outside of actionFrames array since we increase it after each action.
                // For example, if there are two action frames, we start with currentActionIndex = 0 and increase it each click _after_ we have processed the index
                // to retrieve the actions we need to take. After two clicks, we are at currentActionIndex = 2 which is the first invalid index.
                return CurrentPage.ActionFrames > 0 && CurrentActionIndex != CurrentPage.ActionFrames && ExecutionFinished;
            }
        }
        public bool PrevActionIsEnabled
        {
            get
            {
                return CurrentPage.ActionFrames > 0 && CurrentActionIndex != 0;
            }
        }
        public bool NextRoundIsEnabled
        {
            get
            {
                return CurrentRoundIndex != 20;
            }
        }
        public bool PrevRoundIsEnabled
        {
            get
            {
                return CurrentRoundIndex > 1;
            }
        }

        public readonly string _ROUND_ACTION_LABEL_1 = "_ROUND_ACTION_LABEL_1";
        public readonly string _ROUND_ACTION_LABEL_2 = "_ROUND_ACTION_LABEL_2";
        public readonly string _ROUND_ACTION_LABEL_3 = "_ROUND_ACTION_LABEL_3";
        public readonly string _ROUND_ACTION_LABEL_4 = "_ROUND_ACTION_LABEL_4";
        public readonly string _ROUND_ACTION_LABEL_5 = "_ROUND_ACTION_LABEL_5";
        public readonly string _ROUND_ACTION_LABEL_6 = "_ROUND_ACTION_LABEL_6";
        public readonly string _ROUND_ACTION_LABEL_7 = "_ROUND_ACTION_LABEL_7";
        public readonly string _ROUND_ACTION_LABEL_8 = "_ROUND_ACTION_LABEL_8";
        public readonly string _ROUND_ACTION_LABEL_9 = "_ROUND_ACTION_LABEL_9";
        public readonly string _ROUND_ACTION_LABEL_10 = "_ROUND_ACTION_LABEL_10";
        public readonly string _ROUND_ACTION_LABEL_11 = "_ROUND_ACTION_LABEL_11";
        public readonly string _ROUND_ACTION_LABEL_12 = "_ROUND_ACTION_LABEL_12";
        public readonly string _ROUND_ACTION_LABEL_13 = "_ROUND_ACTION_LABEL_13";
        public readonly string _ROUND_ACTION_LABEL_14 = "_ROUND_ACTION_LABEL_14";
        public readonly string _ROUND_ACTION_LABEL_15 = "_ROUND_ACTION_LABEL_15";
        public readonly string _ROUND_ACTION_LABEL_16 = "_ROUND_ACTION_LABEL_16";
        public readonly string _ROUND_ACTION_LABEL_17 = "_ROUND_ACTION_LABEL_17";
        public readonly string _ROUND_ACTION_LABEL_18 = "_ROUND_ACTION_LABEL_18";
        public readonly string _ROUND_ACTION_LABEL_19 = "_ROUND_ACTION_LABEL_19";
        public readonly string _ROUND_ACTION_LABEL_20 = "_ROUND_ACTION_LABEL_20";

        #endregion

        #region action navigation click handlers

        private void PrevAction_Click(object sender, RoutedEventArgs e)
        {
            CurrentActionIndex--;
            CurrentPage.Actions[CurrentActionIndex].undo();
        }
        private void NextAction_Click(object sender, RoutedEventArgs e)
        {
            WrapExecWithNavigation(CurrentPage.Actions[CurrentActionIndex]);
            CurrentActionIndex++;
        }
        private void ResetPageActions()
        {
            for (int i = CurrentActionIndex; i > 0; i--)
            {
                // this undo's the page actions on each action click
                PrevAction_Click(null, null);
            }
            Debug.Assert(CurrentActionIndex == 0);
            // Dont forget to also undo init actions.
            // Reverse because order (may) matter. Undoing should be done in a FIFO queue!
            foreach (PageAction pageAction in CurrentPage.InitActions.Reverse())
            {
                pageAction.undo();
            }
        }

        private void MoveActions(int n)
        {
            if (n < 0)
            {
                for (int i = 0; i < Math.Abs(n); ++i)
                {
                    PrevAction_Click(null, null);
                }
            }
            else
            {
                for (int i = 0; i < Math.Abs(n); ++i)
                {
                    NextAction_Click(null, null);
                }
            }
        }

        private void MoveToAction(int n)
        {
            MoveActions(n - CurrentActionIndex);
        }
        private void PrevRound_Click(object sender, RoutedEventArgs e)
        {
            int startIndex = CurrentActionIndex;
            int endIndex = Array.FindIndex(CurrentActions, pageAction => pageAction.Label == string.Format("_ROUND_ACTION_LABEL_{0}", CurrentRoundIndex)) + 1;
            if(startIndex == endIndex)
            {
                // We are currently at the start of a round. Go to previous round.
                endIndex = Array.FindIndex(CurrentActions, pageAction => pageAction.Label == string.Format("_ROUND_ACTION_LABEL_{0}", CurrentRoundIndex - 1)) + 1;
            }
            Debug.Assert(startIndex > endIndex);
            for (int i = startIndex; i > endIndex; --i)
            {
                PrevAction_Click(null, null);
            }
            UpdateCurrentActionIntervalIndex();
        }
        private void NextRound_Click(object sender, RoutedEventArgs e)
        {
            int startIndex = CurrentActionIndex;
            int endIndex = Array.FindIndex(CurrentActions, pageAction => pageAction.Label == string.Format("_ROUND_ACTION_LABEL_{0}", CurrentRoundIndex + 1));
            Debug.Assert(startIndex < endIndex);
            for (int i = startIndex; i <= endIndex; ++i)
            {
                NextAction_Click(null, null);
            }
            UpdateCurrentActionIntervalIndex();
        }

        private int CurrentRoundIndex { get; set; } = 0;

        #endregion

        #region action helper methods

        #region Page Action creators

        public PageAction MarkCopyFromAction(Border[] borders)
        {
            return new PageAction(() =>
            {
                foreach (Border b in borders)
                {
                    nav.SetCopyBackground(b);
                }
            }, nav.Undo);
        }

        public PageAction CopyAction(Border[] copyFrom, Border[] copyTo, bool replace = false)
        {
            return new PageAction(() =>
            {
                for (int i = 0; i < copyTo.Length; ++i)
                {
                    Border copyFromBorder = copyFrom[i];
                    Border copyToBorder = copyTo[i];
                    nav.SetCopyBackground(copyToBorder);
                    string text = "";
                    if (copyFromBorder.Child is TextBlock copyFromTextBlock)
                    {
                        text = ((Run)copyFromTextBlock.Inlines.LastInline).Text;

                    }
                    else if (copyFromBorder.Child is RichTextBox copyFromRichTextBox)
                    {
                        TextRange textRange = new TextRange(copyFromRichTextBox.Document.ContentStart, copyFromRichTextBox.Document.ContentEnd);
                        text = textRange.Text;
                    }
                    else if(copyFromBorder.Child is TextBox copyFromTextBox)
                    {
                        text = copyFromTextBox.Text;
                    }
                    else
                    {
                        Debug.Assert(false, "Input type for CopyAction not supported.");
                    }
                    if(copyToBorder.Child is TextBlock copyToTextBlock)
                    {
                        if (replace)
                        {
                            nav.ReplaceLast(copyToTextBlock, text);
                        }
                        else
                        {
                            nav.Add(copyToTextBlock, text);
                        }
                    }
                    else if(copyToBorder.Child is RichTextBox copyToRichTextBox)
                    {
                        if (replace)
                        {
                            nav.Replace(copyToRichTextBox, text);
                        }
                        else
                        {
                            nav.Add(copyToRichTextBox, text);
                        }
                    }
                    else if(copyToBorder.Child is TextBox copyToTextBox)
                    {
                        nav.Replace(copyToTextBox, text);
                    }
                    else
                    {
                        Debug.Assert(false, "Output type for CopyAction not supported.");
                    }
                }
            }, nav.Undo);
        }

        public PageAction UnmarkCopyAction(Border[] copyFrom, Border[] copyTo)
        {
            return new PageAction(() =>
            {
                foreach (Border b in copyFrom)
                {
                    nav.UnsetBackground(b);
                }
                foreach (Border b in copyTo)
                {
                    nav.UnsetBackground(b);
                }
            }, nav.Undo);
        }

        public PageAction[] CopyActions(Border[] copyFrom, Border[] copyTo, bool replace = false)
        {
            Debug.Assert(copyFrom.Length == copyTo.Length);
            PageAction mark = MarkCopyFromAction(copyFrom);
            PageAction copy = CopyAction(copyFrom, copyTo, replace);
            PageAction unmark = UnmarkCopyAction(copyFrom, copyTo);
            return new PageAction[] { mark, copy, unmark };
        }
        public PageAction[] CopyActions(Border[] copyFrom, Shape[] paths, Border[] copyTo, bool replace = false)
        {
            PageAction[] copyActions = CopyActions(copyFrom, copyTo, replace);
            Action markPaths = () =>
            {
                foreach (Shape s in paths)
                {
                    s.StrokeThickness = 5;
                }
            };
            Action undoMarkPaths = () =>
            {
                foreach (Shape s in paths)
                {
                    s.StrokeThickness = 1;
                }
            };
            PageAction mark = copyActions[0];
            mark.AddToExec(markPaths);
            mark.AddToUndo(undoMarkPaths);
            PageAction unmark = copyActions[2];
            unmark.AddToExec(undoMarkPaths);
            unmark.AddToUndo(markPaths);
            return copyActions;
        }
        #endregion

        #endregion

        #endregion
    }

    #region ActionNavigationInterface

    // I wish I had variadic templates in C# like in C++11 ...
    interface IActionNavigationService<T1, T2, T3, T4, T5>
    {
        // Save state of each element such that we can retrieve it later for undoing action.
        void SaveState(params T1[] t);

        void SaveState(params T2[] t);

        void SaveState(params T3[] t);

        void SaveState(params T4[] t);

        void SaveState(params T5[] t);

        // Tells that current page action is finished and thus next calls to save state are for a new page action.
        void FinishPageAction();

        // Get list of actions which completely reverts the page action of the given index.
        Dictionary<int, Action> GetUndoActions();

        // Execute automatic undoing of actions.
        void Undo();
    }

    #endregion

    #region NavigationInterface implementation

    class ActionNavigation : IActionNavigationService<TextBlock, Border, Shape, RichTextBox, TextBox>
    {
        #region interface methods
        /*
         * Stack with actions where the last dictionary contains undo actions which reverts changes from the last applied page action of an UI Element.
         */
        private Stack<Dictionary<int, Action>> _undoState = new Stack<Dictionary<int, Action>>();
        // temporary variable to collect undo actions before pushing into stack.
        private Dictionary<int, Action> _undoActions = new Dictionary<int, Action>();
        // bool to check if FinishPageAction should be called after a page action execution.
        private bool _saveStateHasBeenCalled = false;

        public bool SaveStateHasBeenCalled
        {
            get
            {
                return _saveStateHasBeenCalled;
            }
        }

        public void SaveState(params TextBlock[] textblocks)
        {
            _saveStateHasBeenCalled = true;
            foreach (TextBlock tb in textblocks)
            {
                int hash = tb.GetHashCode();
                // do not overwrite states since first added state was the "most original one"
                if (!_undoActions.ContainsKey(hash))
                {
                    // copy inline elements
                    Inline[] state = new Inline[tb.Inlines.Count];
                    tb.Inlines.CopyTo(state, 0);
                    _undoActions[hash] = () => {
                        tb.Inlines.Clear();
                        foreach (Inline i in state)
                        {
                            tb.Inlines.Add(i);
                        }
                    };
                }
            }
        }

        public void SaveState(params Border[] borders)
        {
            _saveStateHasBeenCalled = true;
            foreach (Border b in borders)
            {
                int hash = b.GetHashCode();
                if (!_undoActions.ContainsKey(hash))
                {
                    Brush background;
                    Brush borderBrush;
                    Thickness borderThickness;
                    if (b.Background != null)
                    {
                        background = b.Background.Clone();
                    }
                    else
                    {
                        background = Brushes.White;
                    }
                    if (b.BorderBrush != null)
                    {
                        borderBrush = b.BorderBrush.Clone();
                    }
                    else
                    {
                        borderBrush = Brushes.Black;
                    }
                    if (b.BorderThickness != null)
                    {
                        borderThickness = b.BorderThickness;
                    }
                    else
                    {
                        borderThickness = new Thickness(1);
                    }
                    _undoActions[hash] = () =>
                    {
                        b.Background = background;
                        b.BorderBrush = borderBrush;
                        b.BorderThickness = borderThickness;
                    };
                }
            }
        }

        public void SaveState(params Shape[] shapes)
        {
            _saveStateHasBeenCalled = true;
            foreach (Shape s in shapes)
            {
                int hash = s.GetHashCode();
                if (!_undoActions.ContainsKey(hash))
                {
                    Brush strokeBrush;
                    double strokeThickness = s.StrokeThickness;
                    if (s.Stroke != null)
                    {
                        strokeBrush = s.Stroke.Clone();
                    }
                    else
                    {
                        strokeBrush = Brushes.Black;
                    }
                    _undoActions[hash] = () =>
                    {
                        s.Stroke = strokeBrush;
                        s.StrokeThickness = strokeThickness;
                    };
                }
            }
        }

        public void SaveState(params RichTextBox[] textboxes)
        {
            _saveStateHasBeenCalled = true;
            foreach (RichTextBox rtb in textboxes)
            {
                int hash = rtb.GetHashCode();
                // copy block element
                Block[] state = new Block[rtb.Document.Blocks.Count];
                rtb.Document.Blocks.CopyTo(state, 0);
                _undoActions[hash] = () =>
                {
                    rtb.Document.Blocks.Clear();
                    foreach (Block b in state)
                    {
                        rtb.Document.Blocks.Add(b);
                    }
                };
            }
        }

        public void SaveState(params TextBox[] textboxes)
        {
            _saveStateHasBeenCalled = true;
            foreach (TextBox tb in textboxes)
            {
                int hash = tb.GetHashCode();
                // copy text element
                string state = tb.Text;
                _undoActions[hash] = () =>
                {
                    tb.Text = state;
                };
            }
        }

        public void FinishPageAction()
        {
            // copy dictionary using new
            _undoState.Push(new Dictionary<int, Action>(_undoActions));
            _undoActions.Clear();
            _saveStateHasBeenCalled = false;
        }

        public Dictionary<int, Action> GetUndoActions()
        {
            return _undoState.Pop();
        }

        public void Undo()
        {
            Dictionary<int, Action> undoActions = GetUndoActions();
            foreach (Action undo in undoActions.Values)
            {
                undo();
            }
        }
        #endregion

        #region API methods

        private readonly Brush copyBrush = Brushes.AliceBlue;
        private readonly Brush markBrush = Brushes.Purple;

        #region internals

        #region InlineCollection methods
        private void _RemoveLast(InlineCollection list)
        {
            list.Remove(list.LastInline);
        }
        private void _ReplaceLast(InlineCollection list, Inline element)
        {
            _RemoveLast(list);
            list.Add(element);
        }
        private void _ReplaceLast(InlineCollection list, string text)
        {
            _ReplaceLast(list, new Run(text));
        }
        private void _MakeBoldLast(InlineCollection list)
        {
            _ReplaceLast(list, MakeBold((Run)(list.LastInline)));
        }
        private void _UnboldLast(InlineCollection list)
        {
            _ReplaceLast(list, new Run { Text = ((Run)(list.LastInline)).Text });
        }
        private void _Add(InlineCollection list, Inline element)
        {
            list.Add(element);
        }
        private void _Clear(InlineCollection list)
        {
            list.Clear();
        }
        #endregion

        #region BlockCollection methods
        private void _RemoveLast(BlockCollection list)
        {
            list.Remove(list.LastBlock);
        }
        private void _ReplaceLast(BlockCollection list, Inline element)
        {
            _RemoveLast(list);
            list.Add(new Paragraph(element));
        }
        private void _UnboldLast(BlockCollection list)
        {
            if(list.LastBlock != null) _ReplaceLast(list, new Run { Text = (((Run)((Paragraph)(list.LastBlock)).Inlines.LastInline).Text) });
        }
        private void _Add(BlockCollection list, Inline element)
        {
            list.Add(new Paragraph(element));
        }
        private void _Clear(BlockCollection list)
        {
            list.Clear();
        }
        #endregion

        #endregion

        #region exposed API

        #region helper methods
        private Run MakeBold(Run r)
        {
            return new Run { Text = r.Text, FontWeight = FontWeights.Bold };
        }
        #endregion

        #region TextBlock

        public void RemoveLast(TextBlock tb)
        {
            SaveState(tb);
            _RemoveLast(tb.Inlines);
        }
        public void ReplaceLast(TextBlock tb, Inline element)
        {
            SaveState(tb);
            _ReplaceLast(tb.Inlines, element);
        }
        public void ReplaceLast(TextBlock tb, string text)
        {
            SaveState(tb);
            _ReplaceLast(tb.Inlines, new Run(text));
        }
        public void MakeBoldLast(TextBlock tb)
        {
            SaveState(tb);
            _MakeBoldLast(tb.Inlines);
        }
        public void UnboldLast(params TextBlock[] tbs)
        {
            foreach(TextBlock tb in tbs)
            {
                SaveState(tb);
                _UnboldLast(tb.Inlines);
            }
        }
        public void Add(TextBlock tb, Inline element)
        {
            SaveState(tb);
            _Add(tb.Inlines, element);
        }
        public void Add(TextBlock tb, string element)
        {
            Add(tb, new Run(element));
        }
        public void AddBold(TextBlock tb, string element)
        {
            Add(tb, MakeBold(new Run(element)));
        }
        public void Clear(params TextBlock[] textblocks)
        {
            foreach (TextBlock tb in textblocks)
            {
                SaveState(tb);
                _Clear(tb.Inlines);
            }
        }
        private void CopyLastText(TextBlock tbToCopyTo, TextBlock tbToCopyFrom)
        {
            SaveState(tbToCopyTo);
            tbToCopyTo.Inlines.Add(((Run)(tbToCopyFrom.Inlines.LastInline)).Text);
        }
        private void SetVisible(TextBlock tb)
        {
            SaveState(tb);
            tb.Visibility = Visibility.Visible;
        }
        private void SetInvisible(TextBlock tb)
        {
            SaveState(tb);
            tb.Visibility = Visibility.Hidden;
        }
        #endregion

        #region Border

        public void SetBackground(Border b, Brush background)
        {
            SaveState(b);
            b.Background = background;
        }
        public void SetCopyBackground(Border b)
        {
            SetBackground(b, copyBrush);
        }
        public void UnsetBackground(Border b)
        {
            SetBackground(b, Brushes.White);
        }
        public void SetBorderColor(Border b, Brush borderBrush)
        {
            SaveState(b);
            b.BorderBrush = borderBrush;
        }
        public void SetBorderStroke(Border b, double stroke)
        {
            SaveState(b);
            b.BorderThickness = new Thickness(stroke);
        }
        public void MarkBorder(Border b)
        {
            SetBorderColor(b, markBrush);
            SetBorderStroke(b, 2);
        }
        public void UnmarkBorder(Border b)
        {
            SetBorderColor(b, Brushes.Black);
            SetBorderStroke(b, 1);
        }

        #endregion

        #region Shape

        public void SetShapeStrokeColor(Shape s, Brush brush)
        {
            SaveState(s);
            s.Stroke = brush;
        }
        public void SetShapeStroke(Shape s, double stroke)
        {
            SaveState(s);
            s.StrokeThickness = stroke;
        }
        public void MarkShape(Shape s)
        {
            SetShapeStrokeColor(s, markBrush);
            SetShapeStroke(s, 2);
        }
        public void UnmarkShape(Shape s)
        {
            SetShapeStrokeColor(s, Brushes.Black);
            SetShapeStroke(s, 1);
        }

        #endregion

        #region RichTextBox
        public void UnboldLast(params RichTextBox[] rtbs)
        {
            foreach(RichTextBox rtb in rtbs)
            {
                SaveState(rtb);
                _UnboldLast(rtb.Document.Blocks);
            }
        }
        public void Add(RichTextBox tb, Inline element)
        {
            SaveState(tb);
            _Add(tb.Document.Blocks, element);
        }
        public void Add(RichTextBox tb, string text)
        {
            Add(tb, new Run(text));
        }
        public void AddBold(RichTextBox tb, string text)
        {
            Add(tb, MakeBold(new Run(text)));
        }
        public void Clear(params RichTextBox[] tbs)
        {
            foreach (RichTextBox tb in tbs)
            {
                SaveState(tb);
                _Clear(tb.Document.Blocks);
            }
        }
        public void Replace(RichTextBox rtb, string text)
        {
            _Clear(rtb.Document.Blocks);
            Add(rtb, text);
        }
        #endregion

        #region TextBox

        public void Replace(TextBox tb, string text)
        {
            SaveState(tb);
            tb.Text = text;
        }

        public void Clear(params TextBox[] tbs)
        {
            foreach(TextBox tb in tbs)
            {
                SaveState(tb);
                tb.Text = "";
            }
        }
        #endregion

        #endregion

        #endregion
    }

    #endregion
}
