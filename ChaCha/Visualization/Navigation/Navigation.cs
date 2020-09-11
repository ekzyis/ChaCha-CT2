﻿using System;
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
        public ActionNavigation nav = new ActionNavigation();

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
            Binding binding = new Binding("NavigationEnabled");
            b.SetBinding(Button.IsEnabledProperty, binding);
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
            Binding currentActionIndexBinding = new Binding("CurrentActionIndex") { Mode = BindingMode.OneWay };
            s.Minimum = 0;
            s.Maximum = totalActions;
            // TODO set width dynamically depending on total actions
            s.Width = 1000;
            s.TickFrequency = 1;
            s.TickPlacement = TickPlacement.None;
            s.IsSnapToTickEnabled = true;
            s.SetBinding(Slider.ValueProperty, currentActionIndexBinding);
            s.AutoToolTipPlacement = AutoToolTipPlacement.BottomRight;
            void S_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
            {
                MoveToAction((int)s.Value);
            };
            s.ValueChanged += S_ValueChanged;
            s.VerticalAlignment = VerticalAlignment.Center;
            TextBlock current = new TextBlock() { VerticalAlignment = VerticalAlignment.Center };
            current.SetBinding(TextBlock.TextProperty, currentActionIndexBinding);
            TextBlock delimiter = new TextBlock() { VerticalAlignment = VerticalAlignment.Center, Text = "/" };
            TextBlock total = new TextBlock() { VerticalAlignment = VerticalAlignment.Center, Text = totalActions.ToString() };
            actionNavBar.Children.Add(PrevButton());
            actionNavBar.Children.Add(s);
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
                //_InitActionButtonNavigationBar(p.ActionNavigationBar, totalActions);
                _InitActionSliderNavigationBar(p.ActionNavigationBar, totalActions);
            }
        }

        #region page navigation

        #region classes, structs and methods related page navigation

       

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
            AddPage(Page.LandingPage(this));
            AddPage(Page.WorkflowPage(this));
            AddPage(Page.StateMatrixPage(this));
            AddPage(Page.KeystreamBlockGenPage(this));
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
        public int CurrentActionIndex
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
                    return Page.LandingPage(this);
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
                OnPropertyChanged("NavigationEnabled");
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
                OnPropertyChanged("NavigationEnabled");
            }
        }

        public bool NextPageIsEnabled
        {
            get
            {
                return CurrentPageIndex != MaxPageIndex && NavigationEnabled;
            }
        }
        public bool PrevPageIsEnabled
        {
            get
            {
                return CurrentPageIndex != 0 && NavigationEnabled;
            }
        }
        public bool NavigationEnabled
        {
            get
            {
                return ExecutionFinished && InputValid;
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
                return CurrentRoundIndex >= 1;
            }
        }

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
        private Button PrevButton()
        {
            Button b = CreateNavigationButton();
            b.SetBinding(Button.IsEnabledProperty, new Binding("PrevActionIsEnabled"));
            b.Click += PrevAction_Click;
            b.Content = "<";
            return b;
        }
        private Button NextButton()
        {
            Button b = CreateNavigationButton();
            b.SetBinding(Button.IsEnabledProperty, new Binding("NextActionIsEnabled"));
            b.Click += NextAction_Click;
            b.Content = ">";
            return b;
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
        private int GetLabeledPageActionIndex(string label)
        {
            return Array.FindIndex(CurrentActions, pageAction => Array.FindIndex(pageAction.Labels, actionlabel => actionlabel == label) != -1);
        }
        private void PrevRound_Click(object sender, RoutedEventArgs e)
        {
            int startIndex = CurrentActionIndex;
            int endIndex = GetLabeledPageActionIndex(string.Format("_ROUND_ACTION_LABEL_{0}", CurrentRoundIndex)) + 1;
            if(startIndex == endIndex)
            {
                // We are currently at the start of a round. Go to previous round.
                endIndex = GetLabeledPageActionIndex(string.Format("_ROUND_ACTION_LABEL_{0}", CurrentRoundIndex - 1)) + 1;
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
            int endIndex = GetLabeledPageActionIndex(string.Format("_ROUND_ACTION_LABEL_{0}", CurrentRoundIndex + 1));
            Debug.Assert(startIndex < endIndex);
            for (int i = startIndex; i <= endIndex; ++i)
            {
                NextAction_Click(null, null);
            }
            UpdateCurrentActionIntervalIndex();
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
            string searchLabel = string.Format("_QR_ACTION_LABEL_{0}_{1}", qrLabelSearchIndex, roundLabelSearchIndex);
            int qrActionIndex = GetLabeledPageActionIndex(searchLabel) + 1;
            MoveToAction(qrActionIndex);
            InitActionNavigationBar(CurrentPage);
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

        public int CurrentRoundIndex { get; set; } = 0;

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
}
