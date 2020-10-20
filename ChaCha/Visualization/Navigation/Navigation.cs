using System;
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
using System.Windows.Navigation;

namespace Cryptool.Plugins.ChaCha
{
    partial class ChaChaPresentation
    {
        public ActionNavigation Nav = new ActionNavigation();
        private CancellationTokenSource actionNavigationTokenSource;

        private const int NAVIGATION_BAR_HEIGHT = 24;
        private const int NAVIGATION_BAR_FONTSIZE = 16;
        private static FontStyle NAVIGATION_BAR_BUTTON_FONTSTYLE = FontStyles.Italic;
        private static Button CreateNavigationButton()
        {
            Button b = new Button { FontSize = NAVIGATION_BAR_FONTSIZE, FontStyle = NAVIGATION_BAR_BUTTON_FONTSTYLE, Height = NAVIGATION_BAR_HEIGHT, Width = 32, Margin = new Thickness(1, 0, 1, 0) };
            b.SetBinding(Button.IsEnabledProperty, new Binding("NavigationEnabled"));
            return b;
        }

        private static Button CreatePrevNavigationButton()
        {
            Button b = CreateNavigationButton();
            b.Content = "\uD83E\uDC60";
            b.FontStyle = FontStyles.Normal;
            return b;
        }

        private static Button CreateNextNavigationButton()
        {
            Button b = CreateNavigationButton();
            b.Content = "\uD83E\uDC62";
            b.FontStyle = FontStyles.Normal;
            return b;
        }

        private static TextBox CreateNavigationTextBox()
        {
            TextBox tb = new TextBox { FontSize = NAVIGATION_BAR_FONTSIZE, Height = NAVIGATION_BAR_HEIGHT, Width = 24, Margin = new Thickness(1, 0, 1, 0) };
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
                if (s.IsFocused)
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
            private ulong _maxActionIndex;
            private ulong _minActionIndex;
            public InputActionIndexRule(ulong maxActionIndex)
            {
                _maxActionIndex = maxActionIndex;
            }
            public InputActionIndexRule(int maxActionIndex) : this((ulong)maxActionIndex) { }

            public InputActionIndexRule(ulong minActionIndex, ulong maxActionIndex)
            {
                _minActionIndex = minActionIndex;
                _maxActionIndex = maxActionIndex;
            }

            public InputActionIndexRule(int minActionIndex, int maxActionIndex) : this((ulong)minActionIndex, (ulong)maxActionIndex) { }

            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                ulong input = 0;

                try
                {
                    input = ulong.Parse((String)value);
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

        private class BooleanToFontWeightConverter : IValueConverter
        {
            private FontWeight trueFontWeight;
            private FontWeight falseFontWeight;

            public BooleanToFontWeightConverter(FontWeight trueFontWeight, FontWeight falseFontWeight)
            {
                this.trueFontWeight = trueFontWeight;
                this.falseFontWeight = falseFontWeight;
            }

            public BooleanToFontWeightConverter(FontWeight trueFontWeight) : this(trueFontWeight, FontWeights.Normal) { }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (bool)value ? trueFontWeight : falseFontWeight;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private TextBox CreateCurrentActionIndexTextBox(int totalActions)
        {
            TextBox current = new TextBox { FontSize = NAVIGATION_BAR_FONTSIZE, Height = NAVIGATION_BAR_HEIGHT, VerticalAlignment = VerticalAlignment.Center, Width = 40 };
            Binding actionIndexBinding = new Binding("CurrentActionIndexTextBox")
            { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
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

        private Button CreatePageButton(string text, int currentPageIndex, int toPageIndex, int width)
        {
            Button b = CreateNavigationButton();
            b.Content = text;
            b.Width = width;
            b.Margin = new Thickness(0);
            b.Height = NAVIGATION_BAR_HEIGHT;
            // since each page has its own dedicated navigation bar, the button for the current page is always bold
            if (currentPageIndex == toPageIndex) b.FontWeight = FontWeights.Bold;
            b.Click += new RoutedEventHandler(MoveToPageClickWrapper(toPageIndex));
            return b;
        }

        private void InitPageNavigationBar(Page p, int pageIndex = -1)
        {
            pageIndex = pageIndex == -1 ? _pages.FindIndex(p_ => p_ == p) : pageIndex;
            StackPanel pageNavBar = p.PageNavigationBar;
            pageNavBar.Children.Clear();
            Button start = CreatePageButton("Start", pageIndex, 0, 64);
            Button overview = CreatePageButton("Overview", pageIndex, 1, 128);
            Button stateMatrixInit = CreatePageButton("State Matrix Initialization", pageIndex, 2, 320);
            Button keystream = CreatePageButton("Keystream Generation", pageIndex, 3, 320);
            if (pageIndex >= 3) keystream.FontWeight = FontWeights.Bold;
            pageNavBar.Children.Add(start);
            pageNavBar.Children.Add(overview);
            pageNavBar.Children.Add(stateMatrixInit);
            pageNavBar.Children.Add(keystream);

            StackPanel pageNavBar2 = p.PageNavigationBar2;
            pageNavBar2.Children.Clear();
            Button previousPage = CreatePrevNavigationButton();
            previousPage.SetBinding(Button.IsEnabledProperty, new Binding("PrevPageIsEnabled"));
            previousPage.Click += PrevPage_Click;
            Button nextPage = CreateNextNavigationButton();
            nextPage.SetBinding(Button.IsEnabledProperty, new Binding("NextPageIsEnabled"));
            nextPage.Click += NextPage_Click;
            pageNavBar2.Children.Add(previousPage);
            pageNavBar2.Children.Add(nextPage);
        }

        private TextBox CreateKeystreamBlockTextBox()
        {
            TextBox current = CreateNavigationTextBox();
            Binding actionIndexBinding = new Binding("CurrentKeystreamBlockTextBox")
            { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            ValidationRule inputActionIndexRule = new InputActionIndexRule(1, MaxKeystreamBlock);
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
                        MoveToKeystreamPage(ulong.Parse(value));
                    }
                }
            }

            current.KeyDown += HandleKeyDown;
            current.Width = 40;
            return current;
        }


        private TextBox CreateRoundTextBox(int totalRounds)
        {
            TextBox current = CreateNavigationTextBox();
            Binding actionIndexBinding = new Binding("CurrentRoundIndexTextBox")
            { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            ValidationRule inputActionIndexRule = new InputActionIndexRule(1, totalRounds);
            actionIndexBinding.ValidationRules.Add(inputActionIndexRule);
            current.SetBinding(TextBox.TextProperty, actionIndexBinding);

            void HandleKeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Return)
                {
                    string rawValue = ((TextBox)sender).Text;
                    ValidationResult result = inputActionIndexRule.Validate(rawValue, null);
                    if (result == ValidationResult.ValidResult)
                    {
                        MoveToRound(int.Parse(rawValue));
                    }
                }
            }

            current.KeyDown += HandleKeyDown;
            return current;
        }

        private TextBox CreateQuarterroundTextBox()
        {
            TextBox current = CreateNavigationTextBox();
            Binding actionIndexBinding = new Binding("CurrentQuarterroundIndexTextBox")
            { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            ValidationRule inputActionIndexRule = new InputActionIndexRule(1, 4);
            actionIndexBinding.ValidationRules.Add(inputActionIndexRule);
            current.SetBinding(TextBox.TextProperty, actionIndexBinding);

            void HandleKeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Return)
                {
                    string rawValue = ((TextBox)sender).Text;
                    ValidationResult result = inputActionIndexRule.Validate(rawValue, null);
                    if (result == ValidationResult.ValidResult)
                    {
                        MoveToQuarterround(int.Parse(rawValue));
                    }
                }
            }

            current.KeyDown += HandleKeyDown;
            return current;
        }

        private void InitKeystreamNavigation(Page p)
        {
            ulong totalKeystreamBlocks = KeystreamBlocksNeeded;
            int totalRounds = Rounds;
            // Assume that general page navigation bar has already been initialized
            StackPanel pageNavBar = p.PageNavigationBar;

            Grid keystreamBlockGrid = new Grid() { Margin = new Thickness(0, -18, 0, 0) };
            keystreamBlockGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(NAVIGATION_BAR_HEIGHT - 5) });
            keystreamBlockGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(NAVIGATION_BAR_HEIGHT) });
            TextBlock keystreamLabel = new TextBlock() { FontSize = NAVIGATION_BAR_FONTSIZE - 2, Height = NAVIGATION_BAR_HEIGHT - 5, Text = "Keystream Block", HorizontalAlignment = HorizontalAlignment.Center };
            StackPanel keystreamBlockBottomRow = new StackPanel() { Orientation = Orientation.Horizontal };
            Grid.SetRow(keystreamLabel, 0);
            Grid.SetRow(keystreamBlockBottomRow, 1);
            Button previousKeystreamBlock = CreatePrevNavigationButton();
            previousKeystreamBlock.Click += PrevKeystreamBlock_Click;
            previousKeystreamBlock.SetBinding(Button.IsEnabledProperty, new Binding("PrevKeystreamBlockIsEnabled"));
            TextBox currentKeystreamBlock = CreateKeystreamBlockTextBox();
            TextBlock keystreamDelimiter = new TextBlock() { FontSize = NAVIGATION_BAR_FONTSIZE, Height = NAVIGATION_BAR_HEIGHT, Text = "/" };
            TextBlock totalKeystreamBlockLabel = new TextBlock() { FontSize = NAVIGATION_BAR_FONTSIZE, Height = NAVIGATION_BAR_HEIGHT, Text = totalKeystreamBlocks.ToString() };
            Button nextKeystreamBlock = CreateNextNavigationButton();
            nextKeystreamBlock.Click += NextKeystreamBlock_Click;
            nextKeystreamBlock.SetBinding(Button.IsEnabledProperty, new Binding("NextKeystreamBlockIsEnabled"));
            keystreamBlockBottomRow.Children.Add(previousKeystreamBlock);
            keystreamBlockBottomRow.Children.Add(currentKeystreamBlock);
            keystreamBlockBottomRow.Children.Add(keystreamDelimiter);
            keystreamBlockBottomRow.Children.Add(totalKeystreamBlockLabel);
            keystreamBlockBottomRow.Children.Add(nextKeystreamBlock);
            keystreamBlockGrid.Children.Add(keystreamLabel);
            keystreamBlockGrid.Children.Add(keystreamBlockBottomRow);
            pageNavBar.Children.Add(keystreamBlockGrid);


            Grid roundGrid = new Grid() { Margin = new Thickness(0, -18, 0, 0) };
            roundGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(NAVIGATION_BAR_HEIGHT - 5) });
            roundGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(NAVIGATION_BAR_HEIGHT) });
            TextBlock roundLabel = new TextBlock() { FontSize = NAVIGATION_BAR_FONTSIZE - 2, Height = NAVIGATION_BAR_HEIGHT - 5, Text = "Round", HorizontalAlignment = HorizontalAlignment.Center };
            StackPanel roundBottomRow = new StackPanel() { Orientation = Orientation.Horizontal };
            Grid.SetRow(roundLabel, 0);
            Grid.SetRow(roundBottomRow, 1);
            Button previousRound = CreatePrevNavigationButton();
            previousRound.Click += PrevRound_Click;
            previousRound.SetBinding(Button.IsEnabledProperty, new Binding("PrevRoundIsEnabled"));
            TextBox currentRound = CreateRoundTextBox(totalRounds);
            TextBlock delimiterRound = new TextBlock() { FontSize = NAVIGATION_BAR_FONTSIZE, Height = NAVIGATION_BAR_HEIGHT, Text = "/" };
            TextBlock totalRoundsLabel = new TextBlock() { FontSize = NAVIGATION_BAR_FONTSIZE, Height = NAVIGATION_BAR_HEIGHT, Text = totalRounds.ToString() };
            Button nextRound = CreateNextNavigationButton();
            nextRound.Click += NextRound_Click;
            nextRound.SetBinding(Button.IsEnabledProperty, new Binding("NextRoundIsEnabled"));
            roundBottomRow.Children.Add(previousRound);
            roundBottomRow.Children.Add(currentRound);
            roundBottomRow.Children.Add(delimiterRound);
            roundBottomRow.Children.Add(totalRoundsLabel);
            roundBottomRow.Children.Add(nextRound);
            roundGrid.Children.Add(roundLabel);
            roundGrid.Children.Add(roundBottomRow);
            pageNavBar.Children.Add(roundGrid);


            Grid quarterroundGrid = new Grid() { Margin = new Thickness(0, -18, 0, 0) };
            quarterroundGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(NAVIGATION_BAR_HEIGHT - 5) });
            quarterroundGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(NAVIGATION_BAR_HEIGHT) });
            TextBlock quarterroundLabel = new TextBlock() { FontSize = NAVIGATION_BAR_FONTSIZE - 2, Height = NAVIGATION_BAR_HEIGHT - 5, Text = "Quarterround", HorizontalAlignment = HorizontalAlignment.Center };
            StackPanel quarterroundBottomRow = new StackPanel() { Orientation = Orientation.Horizontal };
            Grid.SetRow(quarterroundLabel, 0);
            Grid.SetRow(quarterroundBottomRow, 1);
            Button previousQuarterround = CreatePrevNavigationButton();
            previousQuarterround.Click += PrevQuarterround_Click;
            previousQuarterround.SetBinding(Button.IsEnabledProperty, new Binding("PrevQuarterroundIsEnabled"));
            TextBox currentQuarterround = CreateQuarterroundTextBox();
            TextBlock delimiterQuarterround = new TextBlock() { FontSize = NAVIGATION_BAR_FONTSIZE, Height = NAVIGATION_BAR_HEIGHT, Text = "/" };
            TextBlock totalQuarterRoundsLabel = new TextBlock() { FontSize = NAVIGATION_BAR_FONTSIZE, Height = NAVIGATION_BAR_HEIGHT, Text = "4" };
            Button nextQuarterround = CreateNextNavigationButton();
            nextQuarterround.Click += NextQuarterround_Click;
            nextQuarterround.SetBinding(Button.IsEnabledProperty, new Binding("NextQuarterroundIsEnabled"));
            quarterroundBottomRow.Children.Add(previousQuarterround);
            quarterroundBottomRow.Children.Add(currentQuarterround);
            quarterroundBottomRow.Children.Add(delimiterQuarterround);
            quarterroundBottomRow.Children.Add(totalQuarterRoundsLabel);
            quarterroundBottomRow.Children.Add(nextQuarterround);
            quarterroundGrid.Children.Add(quarterroundLabel);
            quarterroundGrid.Children.Add(quarterroundBottomRow);
            pageNavBar.Children.Add(quarterroundGrid);

            Button goToAddition = CreateNavigationButton();
            goToAddition.Content = "Original State Addition";
            goToAddition.Width = 196;
            goToAddition.Click += GoToAddition;
            pageNavBar.Children.Add(goToAddition);
            goToAddition.SetBinding(Button.FontWeightProperty, new Binding("AtAdditionStep") { Converter = new BooleanToFontWeightConverter(FontWeights.Bold) });

            Button goToLittleEndian = CreateNavigationButton();
            goToLittleEndian.Content = "Little-endian";
            goToLittleEndian.Width = 148;
            goToLittleEndian.Click += GoToLittleEndian;
            goToLittleEndian.SetBinding(Button.FontWeightProperty, new Binding("AtLittleEndianStep") { Converter = new BooleanToFontWeightConverter(FontWeights.Bold) });
            pageNavBar.Children.Add(goToLittleEndian);

            Button keystreamGenEnd = CreateNavigationButton();
            keystreamGenEnd.Content = "End";
            keystreamGenEnd.Width = 128;
            keystreamGenEnd.Click += GoToKeystreamGenEnd;
            keystreamGenEnd.SetBinding(Button.FontWeightProperty, new Binding("AtKeystreamGenerationEnd") { Converter = new BooleanToFontWeightConverter(FontWeights.Bold) });
            pageNavBar.Children.Add(keystreamGenEnd);
        }

        private bool _atAdditionStep = false;
        public bool AtAdditionStep
        {
            get
            {
                return _atAdditionStep;
            }
            set
            {
                _atAdditionStep = value;
                OnPropertyChanged("AtAdditionStep");
            }
        }

        private bool _atLittleEndianStep = false;
        public bool AtLittleEndianStep
        {
            get
            {
                return _atLittleEndianStep;
            }
            set
            {
                _atLittleEndianStep = value;
                OnPropertyChanged("AtLittleEndianStep");
            }
        }

        public bool _atKeystreamGenerationEnd = false;
        public bool AtKeystreamGenerationEnd
        {
            get
            {
                return _atKeystreamGenerationEnd;
            }
            set
            {
                _atKeystreamGenerationEnd = value;
                OnPropertyChanged("AtKeystreamGenerationEnd");
            }
        }

        private void InitActionSliderNavigationBar(StackPanel actionNavBar, int totalActions)
        {
            actionNavBar.Children.Clear();
            Slider actionSlider = CreateActionNavigationSlider(totalActions);
            TextBox current = CreateCurrentActionIndexTextBox(totalActions);
            TextBlock delimiter = new TextBlock() { FontSize = NAVIGATION_BAR_FONTSIZE, VerticalAlignment = VerticalAlignment.Center, Text = "/" };
            TextBlock total = new TextBlock() { FontSize = NAVIGATION_BAR_FONTSIZE, VerticalAlignment = VerticalAlignment.Center, Text = totalActions.ToString() };
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

        private void ReplaceUserPage(Page p)
        {
            Debug.Assert(UserKeystreamBlockGenPageAdded, $"ReplaceUserPage called but no user page found. (page count: {TotalPages})");
            _pages.Remove(_pages.Last());
            AddPage(p);
        }

        private void SetupPage(Page p)
        {
            p.Setup();
            if (p.ActionFrames > 0)
            {
                StartActionBufferHandler(50);
            }
            InitActionNavigationBar(p);
            p.Visibility = Visibility.Visible;
        }

        private void TearDownPreviousUserPage()
        {
            Debug.Assert(UserKeystreamBlockGenPageAdded, $"ReplaceUserPage called but no user page found. (page count: {TotalPages})");
            TearDownPage(_pages.Last());
        }

        private void TearDownPage(Page p)
        {
            StopActionBufferHandler();
            // undo all actions
            MoveToAction(0);
            p.TearDown();
            p.Visibility = Visibility.Collapsed;
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
            else if(n > 0)
            {
                for (int i = 0; i < Math.Abs(n); ++i)
                {
                    NextPage_Click(null, null);
                }
            }
        }

        private void MoveToKeystreamPage(ulong n)
        {
            bool moveToUserPage = n > KeystreamBlocksNeeded;
            bool moveFromUserPage = CurrentPageIndex == TotalPagesWithoutUserKeystreamPages;
            bool userPageExists = UserKeystreamBlockGenPageAdded;
            if (moveToUserPage)
            {
                /*
                 * We want to move to a user page (keystream page with own set counter).
                 * There are three cases to consider:
                 * 1. Moving from a normal page to a user page
                 *   CurrentPageIndex does change to 5      => The navigation system will tear down and setup the pages.
                 * 2. Moving from user page to a user page
                 *   The CurrentPageIndex does not change   => The navigation system will *not* tear down and setup the pages.
                 * 3. Moving from user page to normal page
                 *   The CurrentPageIndex will change       => The navigation system will tear down and setup the pages.
                 *
                 * In this branch, we only have to consider case 1 and 2 since we are currently moving to a user page.
                 */
                bool moveFromNormalPage = CurrentPageIndex < TotalPagesWithoutUserKeystreamPages;
                // at least one case of these two should be true.
                Debug.Assert(moveFromUserPage || moveFromNormalPage);

                // create new keystream block page with given keyblock number
                UserKeystreamBlockGenPage p = Page.UserKeystreamBlockGenPage(this, n);
                InitPageNavigationBar(p, pageIndex: 5);
                InitKeystreamNavigation(p);

                if (moveFromNormalPage)
                {
                    // if there is already a user page, replace it.
                    if (userPageExists) ReplaceUserPage(p);
                    else AddPage(p);
                    // let the navigation system handle page teardown + setup
                    MoveToLastPage();
                }
                else if (moveFromUserPage)
                {
                    // there should already be a user page.
                    Debug.Assert(userPageExists, "Assumption that when moving from user page to user page, there is already a user page, was wrong.");
                    // manually tear down previous user page and setup new user page.
                    TearDownPreviousUserPage();
                    ReplaceUserPage(p);
                    SetupPage(p);
                }
            }
            else
            {
                // case 3 (Moving from user page to normal page) should be handled by the navigation system as if we are moving between normal pages.
                MoveToPage((int)(2 + n));
            }
        }

        private void MoveToLastPage()
        {
            MoveToPage(TotalPages - 1);
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
            InitPageNavigationBar(_pages[0]);
        }

        private void InitExecutableVisualization()
        {

            if (!InputValid)
            {
                ErrorText.Visibility = Visibility.Visible;
                return;
            }
            ErrorText.Visibility = Visibility.Hidden;
            _pages.Clear();
            AddPage(Page.LandingPage(this));
            AddPage(Page.WorkflowPage(this));
            AddPage(Page.StateMatrixPage(this));
            for (ulong i = 0; i < KeystreamBlocksNeeded; ++i)
            {
                AddPage(Page.KeystreamBlockGenPage(this, i + 1));
            }
            CollapseAllPagesExpect(START_VISUALIZATION_ON_PAGE_INDEX);
            for (int i = 0; i < _pages.Count; ++i)
            {
                Page p = _pages[i];
                InitPageNavigationBar(p);
                if (i >= 3) InitKeystreamNavigation(p);
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
            TearDownPage(CurrentPage);
            CurrentPageIndex--;
            SetupPage(CurrentPage);
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            TearDownPage(CurrentPage);
            CurrentPageIndex++;
            SetupPage(CurrentPage);
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
        private ulong _currentKeystreamBlockTextBox = 1;
        private int _currentRoundIndexTextBox = 0;
        private int _currentQuarterroundIndexTextBox = 0;

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

        public ulong CurrentKeystreamBlockTextBox
        {
            get => _currentKeystreamBlockTextBox;
            set
            {
                _currentKeystreamBlockTextBox = value;
                OnPropertyChanged("CurrentKeystreamBlockTextBox");
                OnPropertyChanged("PrevKeystreamBlockIsEnabled");
                OnPropertyChanged("NextKeystreamBlockIsEnabled");
            }
        }

        public int CurrentRoundIndexTextBox
        {
            get => _currentRoundIndexTextBox;
            set
            {
                _currentRoundIndexTextBox = value;
                OnPropertyChanged("CurrentRoundIndexTextBox");
            }
        }

        public int CurrentQuarterroundIndexTextBox
        {
            get => _currentQuarterroundIndexTextBox;
            set
            {
                _currentQuarterroundIndexTextBox = value;
                OnPropertyChanged("CurrentQuarterroundIndexTextBox");
                OnPropertyChanged("NextQuarterroundIsEnabled");
                OnPropertyChanged("PrevQuarterroundIsEnabled");
            }
        }

        private Page CurrentPage => _pages.Count == 0 ? Page.LandingPage(this) : _pages[CurrentPageIndex];

        private int TotalPages => _pages.Count;

        private int TotalPagesWithoutUserKeystreamPages => UserKeystreamBlockGenPageAdded ? TotalPages - 1 : TotalPages;

        private bool UserKeystreamBlockGenPageAdded => TotalPages > (int)KeystreamBlocksNeeded + 3;

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
                                           CurrentActionIndex != CurrentPage.ActionFrames && NavigationEnabled;

        public bool PrevActionIsEnabled => CurrentPage.ActionFrames > 0 && CurrentActionIndex != 0 && NavigationEnabled;

        public bool NextRoundIsEnabled => CurrentRoundIndex != 20 && NavigationEnabled;

        public bool PrevRoundIsEnabled => CurrentRoundIndex >= 1 && NavigationEnabled;

        public ulong MaxKeystreamBlock => Version.BitsCounter == 64 ? ulong.MaxValue : uint.MaxValue;

        public bool NextKeystreamBlockIsEnabled => CurrentKeystreamBlockTextBox != MaxKeystreamBlock;

        public bool PrevKeystreamBlockIsEnabled => CurrentKeystreamBlockTextBox > 1 && NavigationEnabled;

        public bool NextQuarterroundIsEnabled => (NextRoundIsEnabled || CurrentQuarterroundIndexTextBox != 4) && NavigationEnabled;

        public bool PrevQuarterroundIsEnabled => (PrevRoundIsEnabled || CurrentQuarterroundIndexTextBox > 1) && NavigationEnabled;

        public bool NextPageIsEnabled => CurrentPageIndex < TotalPagesWithoutUserKeystreamPages - 1;

        public bool PrevPageIsEnabled => CurrentPageIndex != 0;

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
                CurrentRoundIndexTextBox = value;
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
            b.Content = "\uD83E\uDC60";
            b.FontStyle = FontStyles.Normal;
            return b;
        }

        private Button NextButton()
        {
            Button b = CreateNavigationButton();
            b.SetBinding(Button.IsEnabledProperty, new Binding("NextActionIsEnabled"));
            b.Click += (sender, e) => MoveActionsAsync(1);
            b.Content = "\uD83E\uDC62";
            b.FontStyle = FontStyles.Normal;
            return b;
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

        private void GoToLabeledAction(string actionLabel)
        {
            int additionActionIndex = GetLabeledPageActionIndex(actionLabel, CurrentActions) + 1;
            MoveToActionAsync(additionActionIndex);
        }

        private void GoToAddition(object sender, RoutedEventArgs e)
        {
            GoToLabeledAction(KeystreamBlockGenPage.ACTIONLABEL_ADDITION_START);
        }

        private void GoToLittleEndian(object sender, RoutedEventArgs e)
        {
            GoToLabeledAction(KeystreamBlockGenPage.ACTIONLABEL_LITTLE_ENDIAN_START);
        }

        private void GoToKeystreamGenEnd(object sender, RoutedEventArgs e)
        {
            GoToLastAction();
        }

        private void GoToLastAction()
        {
            int lastActionIndex = CurrentActions.Length;
            MoveToAction(lastActionIndex);
        }

        private ActionCache Cache
        {
            get
            {
                return CurrentPage.Cache;
            }
        }


        private void MoveToQuarterround(int n)
        {
            string searchLabel = KeystreamBlockGenPage.QuarterroundStartLabelWithRound(n, CurrentRoundIndex);
            int qrActionIndex = GetLabeledPageActionIndex(searchLabel, CurrentActions) + 1;
            CurrentQuarterroundIndexTextBox = n;
            MoveToActionAsync(qrActionIndex);
        }

        private void MoveToRound(int n)
        {
            int destination = GetLabeledPageActionIndex(KeystreamBlockGenPage.RoundStartLabel(n), CurrentActions) + 1;
            CurrentRoundIndex = n;
            MoveToActionAsync(destination);
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
                if (Cache.NotEmpty)
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
                catch (TaskCanceledException)
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
            if (startIndex == endIndex)
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

        private void NextKeystreamBlock_Click(object sender, RoutedEventArgs e)
        {
            ulong pageIndex = (ulong)CurrentKeystreamBlockTextBox + 1;
            MoveToKeystreamPage(pageIndex);
        }

        private void PrevKeystreamBlock_Click(object sender, RoutedEventArgs e)
        {
            ulong pageIndex = CurrentKeystreamBlockTextBox - 1;
            MoveToKeystreamPage(pageIndex);
        }

        private void PrevQuarterround_Click(object sender, RoutedEventArgs e)
        {
            int qrIndex = CurrentQuarterroundIndexTextBox - 1;
            if (qrIndex == 0)
            {
                string searchLabel = KeystreamBlockGenPage.QuarterroundStartLabelWithRound(4, CurrentRoundIndex - 1);
                GoToLabeledAction(searchLabel);
                return;
            }
            MoveToQuarterround(qrIndex);
        }

        private void NextQuarterround_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentQuarterroundIndexTextBox == 0)
            {
                // we are in no round yet. Behave as if clicked on next round.
                NextRound_Click(null, null);
                return;
            }
            int qrIndex = CurrentQuarterroundIndexTextBox + 1;
            if(qrIndex == 5)
            {
                NextRound_Click(null, null);
                return;
            }
            MoveToQuarterround(qrIndex);
        }

        private (int, int) GetQRSearchIndices(int qrLabelIndex)
        {
            /*
             * We need to map the QR Label Index which is between 1 and 8 and is corresponds to the row of the pressed Quarterround button to
             * the appropriate page action label.
             * The page action labels are in this format: _QR_ACTION_LABEL_{QR_ROUND}_{ROUND} where QR_ROUND is between 1 - 4 and ROUND from 1 - 20
             * This means, for example, for the first quarterround of round 3, the label is _QR_ACTION_LABEL_1_3.
             */
            int qrLabelSearchIndex = -1;
            int roundLabelSearchIndex = -1;
            if (qrLabelIndex <= 4)
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
            if (qrLabelIndex > 4)
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
            return (qrLabelSearchIndex, roundLabelSearchIndex);
        }

        private void QR_Start_Click(int qrLabelIndex)
        {
            int qrLabelSearchIndex, roundLabelSearchIndex;
            (qrLabelSearchIndex, roundLabelSearchIndex) = GetQRSearchIndices(qrLabelIndex);
            string searchLabel = KeystreamBlockGenPage.QuarterroundStartLabelWithRound(qrLabelSearchIndex, roundLabelSearchIndex);
            int qrActionIndex = GetLabeledPageActionIndex(searchLabel, CurrentActions) + 1;
            MoveToActionAsync(qrActionIndex);
        }
        private void QR1_Start_Click(object sender, RoutedEventArgs e)
        {
            QR_Start_Click(1);
        }
        private void QR2_Start_Click(object sender, RoutedEventArgs e)
        {
            QR_Start_Click(2);
        }
        private void QR3_Start_Click(object sender, RoutedEventArgs e)
        {
            QR_Start_Click(3);
        }
        private void QR4_Start_Click(object sender, RoutedEventArgs e)
        {
            QR_Start_Click(4);
        }
        private void QR5_Start_Click(object sender, RoutedEventArgs e)
        {
            QR_Start_Click(5);
        }
        private void QR6_Start_Click(object sender, RoutedEventArgs e)
        {
            QR_Start_Click(6);
        }
        private void QR7_Start_Click(object sender, RoutedEventArgs e)
        {
            QR_Start_Click(7);
        }
        private void QR8_Start_Click(object sender, RoutedEventArgs e)
        {
            QR_Start_Click(8);
        }

        private void QR_End_Click(int qrLabelIndex)
        {
            int qrLabelSearchIndex, roundLabelSearchIndex;
            (qrLabelSearchIndex, roundLabelSearchIndex) = GetQRSearchIndices(qrLabelIndex);
            string searchLabel = KeystreamBlockGenPage.QuarterroundEndLabelWithRound(qrLabelSearchIndex, roundLabelSearchIndex);
            int qrActionIndex = GetLabeledPageActionIndex(searchLabel, CurrentActions) + 1;
            MoveToActionAsync(qrActionIndex);
        }

        private void QR1_End_Click(object sender, RoutedEventArgs e)
        {
            QR_End_Click(1);
        }
        private void QR2_End_Click(object sender, RoutedEventArgs e)
        {
            QR_End_Click(2);
        }
        private void QR3_End_Click(object sender, RoutedEventArgs e)
        {
            QR_End_Click(3);
        }
        private void QR4_End_Click(object sender, RoutedEventArgs e)
        {
            QR_End_Click(4);
        }
        private void QR5_End_Click(object sender, RoutedEventArgs e)
        {
            QR_End_Click(5);
        }
        private void QR6_End_Click(object sender, RoutedEventArgs e)
        {
            QR_End_Click(6);
        }
        private void QR7_End_Click(object sender, RoutedEventArgs e)
        {
            QR_End_Click(7);
        }
        private void QR8_End_Click(object sender, RoutedEventArgs e)
        {
            QR_End_Click(8);
        }
    }
}
