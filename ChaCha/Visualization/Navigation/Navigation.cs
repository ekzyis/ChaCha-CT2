using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Cryptool.Plugins.ChaCha
{
    partial class ChaChaPresentation
    {
        public ActionNavigation Nav = new ActionNavigation();

        private static Button CreateNavigationButton()
        {
            Button b = new Button {Height = 18.75, Width = 32, Margin = new Thickness(1, 0, 1, 0)};
            return b;
        }

        private static TextBlock CreateNavigationTextBlock(int index, int activeIndex, string bindingElementName)
        {
            TextBlock tb = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center
            };
            Binding binding = new Binding {ElementName = bindingElementName, Path = new PropertyPath("FontSize")};
            tb.SetBinding(TextBlock.FontSizeProperty, binding);
            tb.Text = (index + 1).ToString();
            if(activeIndex == index)
            {
                tb.FontWeight = FontWeights.Bold;
            }
            return tb;
        }

        private static TextBlock CreateNavBarLabel(string bindingElementName, string text)
        {
            TextBlock tb = new TextBlock
            {
                Name = bindingElementName,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 10.0,
                Margin = new Thickness(0, 0, 1, 0),
                Text = text
            };
            return tb;
        }

        private readonly string _PAGELABELNAME = "UINavBarPageLabel";
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
        private void InitActionSliderNavigationBar(StackPanel actionNavBar, int totalActions)
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

        const int START_VISUALIZATION_ON_PAGE_INDEX = 0;
        private void InitVisualization()
        {
            _pages.Clear();
            AddPage(Page.LandingPage(this));
            AddPage(Page.WorkflowPage(this));
            AddPage(Page.StateMatrixPage(this));
            AddPage(Page.KeystreamBlockGenPage(this));
            CollapseAllPagesExpect(START_VISUALIZATION_ON_PAGE_INDEX);
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
            pageAction.Exec();
            if (Nav.SaveStateHasBeenCalled)
            {
                Nav.FinishPageAction();
            }
        }

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
            void MoveToPageClick(object sender, RoutedEventArgs e)
            {
                MoveToPage(n);
            }

            return MoveToPageClick;
        }

        private int _currentPageIndex = 0;
        private int _currentActionIndex = 0;

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
                InitPageNavigationBar(CurrentPage);
                InitActionNavigationBar(CurrentPage);
            }
        }
        public int CurrentActionIndex
        {
            get => _currentActionIndex;
            set
            {
                _currentActionIndex = value;
                OnPropertyChanged("CurrentActionIndex");
                OnPropertyChanged("CurrentActions");
                OnPropertyChanged("NextActionIsEnabled");
                OnPropertyChanged("PrevActionIsEnabled");
                OnPropertyChanged("NextRoundIsEnabled");
                OnPropertyChanged("PrevRoundIsEnabled");
            }
        }

        private Page CurrentPage => _pages.Count == 0 ? Page.LandingPage(this) : _pages[CurrentPageIndex];

        private PageAction[] CurrentActions => CurrentPage.Actions;

        public bool ExecutionFinished
        {
            get => _executionFinished;
            set
            {
                // reload pages to use new variables
                InitVisualization();
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

        public bool NavigationEnabled => InputValid && ExecutionFinished;

        private Button PrevButton()
        {
            Button b = CreateNavigationButton();
            b.SetBinding(Button.IsEnabledProperty, new Binding("PrevActionIsEnabled"));
            b.Click += (sender, e) => MoveActions(-1);
            b.Content = "<";
            return b;
        }

        private Button NextButton()
        {
            Button b = CreateNavigationButton();
            b.SetBinding(Button.IsEnabledProperty, new Binding("NextActionIsEnabled"));
            b.Click += (sender, e) => MoveActions(1);
            b.Content = ">";
            return b;
        }

        private void ResetPageActions()
        {
            MoveToAction(0);
            Debug.Assert(CurrentActionIndex == 0);
            // Dont forget to also undo init actions.
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
         */
        private void MoveActions(int n)
        {
            void PrevActionClick(object sender, RoutedEventArgs e)
            {
                CurrentActionIndex--;
                CurrentPage.Actions[CurrentActionIndex].Undo();
            }

            void NextActionClick(object sender, RoutedEventArgs e)
            {
                WrapExecWithNavigation(CurrentPage.Actions[CurrentActionIndex]);
                CurrentActionIndex++;
            }
            if (n < 0)
            {
                for (int i = 0; i < Math.Abs(n); ++i)
                {
                    PrevActionClick(null, null);
                }
            }
            else
            {
                for (int i = 0; i < Math.Abs(n); ++i)
                {
                    NextActionClick(null, null);
                }
            }
        }

        /**
         * Move to Action with given index.
         *
         * Implements action navigation with absolute value.
         */
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
            MoveToAction(endIndex);
        }

        private void NextRound_Click(object sender, RoutedEventArgs e)
        {
            int startIndex = CurrentActionIndex;
            int endIndex = GetLabeledPageActionIndex(string.Format("_ROUND_ACTION_LABEL_{0}", CurrentRoundIndex + 1)) + 1;
            Debug.Assert(startIndex < endIndex);
            MoveToAction(endIndex);
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
    }
}
