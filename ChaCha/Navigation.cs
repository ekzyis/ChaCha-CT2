﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Cryptool.Plugins.ChaCha
{
    partial class ChaChaPresentation : INavigationService<TextBlock, Border, Shape, RichTextBox>
    {
        #region Navigation

        #region interface methods
        /*
         * Stack with actions where the last dictionary contains undo actions which reverts changes from the last applied page action of an UI Element.
         */
        private Stack<Dictionary<int, Action>> _undoState = new Stack<Dictionary<int, Action>>();
        // temporary variable to collect undo actions before pushing into stack.
        private Dictionary<int, Action> _undoActions = new Dictionary<int, Action>();
        // bool to check if FinishPageAction should be called after a page action execution.
        private bool _saveStateHasBeenCalled = false;
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
            foreach(RichTextBox rtb in textboxes)
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

        public void FinishPageAction()
        {
            // copy dictionary using new
            _undoState.Push(new Dictionary<int, Action>(_undoActions));
            _undoActions.Clear();
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

        #region page navigation

        #region classes, structs and methods related page navigation

        public class PageAction
        {
            private List<Action> _exec = new List<Action>();
            private List<Action> _undo = new List<Action>();
            public PageAction(Action exec, Action undo)
            {
                _exec.Add(exec);
                _undo.Add(undo);
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
            public Page(UIElement UIPageElement)
            {
                _page = UIPageElement;
            }
            private readonly UIElement _page; // the UI element which contains the page - the Visibility of this element will be set to Collapsed / Visible when going to next / previous page.
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
        }

        // List with pages in particular order to implement page navigation + their page actions
        private readonly List<Page> _pages = new List<Page>();
        private void AddPage(Page page)
        {
            _pages.Add(page);
        }

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

        const int __START_VISUALIZATION_ON_PAGE_INDEX__ = 0;
        private void InitPages()
        {
            _pages.Clear();
            AddPage(LandingPage());
            AddPage(WorkflowPage());
            AddPage(StateMatrixPage());
            AddPage(KeystreamBlockGenPage());
            CollapseAllPagesExpect(__START_VISUALIZATION_ON_PAGE_INDEX__);
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

        private void WrapExecWithNavigation(PageAction pageAction)
        {
            _saveStateHasBeenCalled = false;
            pageAction.exec();
            if (_saveStateHasBeenCalled)
            {
                FinishPageAction();
            }
        }

        #endregion

        #region page navigation click handlers
        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage.Visibility = Visibility.Collapsed;
            UndoActions();
            CurrentPageIndex--;
            CurrentPage.Visibility = Visibility.Visible;
            InitPage(CurrentPage);
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage.Visibility = Visibility.Collapsed;
            UndoActions();
            CurrentPageIndex++;
            CurrentPage.Visibility = Visibility.Visible;
            InitPage(CurrentPage);
        }

        #endregion

        #region variables related to page navigation

        private int _currentPageIndex = 0;
        private int _currentActionIndex = 0;

        private bool _executionFinished = false;

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
                    OnPropertyChanged("CurrentPageIndex");
                    OnPropertyChanged("CurrentPage");
                    OnPropertyChanged("NextPageIsEnabled");
                    OnPropertyChanged("PrevPageIsEnabled");
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
            }
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
                InitPages();
                MovePages(__START_VISUALIZATION_ON_PAGE_INDEX__ - CurrentPageIndex);
                _executionFinished = value;
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
                return CurrentPageIndex != MaxPageIndex && ExecutionFinished;
            }
        }
        public bool PrevPageIsEnabled
        {
            get
            {
                return CurrentPageIndex != 0 && ExecutionFinished;
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
        private void UndoActions()
        {
            for (int i = CurrentActionIndex; i > 0; i--)
            {
                PrevAction_Click(null, null);
            }
            Debug.Assert(CurrentActionIndex == 0);
            // Reverse because order (may) matter. Undoing should be done in a FIFO queue!
            foreach (PageAction pageAction in CurrentPage.InitActions.Reverse())
            {
                pageAction.undo();
            }
        }


        #endregion

        #region action helper methods

        #region WPF Wrapper
        private Run MakeBold(Run r)
        {
            return new Run { Text = r.Text, FontWeight = FontWeights.Bold };
        }

        private void RemoveLast(InlineCollection list)
        {
            list.Remove(list.LastInline);
        }
        private void RemoveLast(BlockCollection list)
        {
            list.Remove(list.LastBlock);
        }
        private void RemoveLast(TextBlock tb)
        {
            SaveState(tb);
            RemoveLast(tb.Inlines);
        }
        private void ReplaceLast(InlineCollection list, Inline element)
        {
            RemoveLast(list);
            list.Add(element);
        }
        private void ReplaceLast(BlockCollection list, Inline element)
        {
            RemoveLast(list);
            list.Add(new Paragraph(element));
        }
        private void ReplaceLast(InlineCollection list, string text)
        {
            RemoveLast(list);
            list.Add(new Run(text));
        }
        private void ReplaceLast(TextBlock tb, Inline element)
        {
            SaveState(tb);
            ReplaceLast(tb.Inlines, element);
        }
        private void ReplaceLast(TextBlock tb, string text)
        {
            SaveState(tb);
            ReplaceLast(tb.Inlines, text);
        }
        private void MakeBoldLast(InlineCollection list)
        {
            ReplaceLast(list, MakeBold((Run)(list.LastInline)));
        }
        private void MakeBoldLast(TextBlock tb)
        {
            SaveState(tb);
            MakeBoldLast(tb.Inlines);
        }
        private void UnboldLast(InlineCollection list)
        {
            ReplaceLast(list, new Run { Text = ((Run)(list.LastInline)).Text });
        }
        private void UnboldLast(BlockCollection list)
        {
            ReplaceLast(list, new Run { Text = (((Run)((Paragraph)(list.LastBlock)).Inlines.LastInline).Text) });
        }
        private void UnboldLast(TextBlock tb)
        {
            SaveState(tb);
            UnboldLast(tb.Inlines);
        }
        private void UnboldLast(RichTextBox tb)
        {
            SaveState(tb);
            UnboldLast(tb.Document.Blocks);
        }
        private void Add(InlineCollection list, Inline element)
        {
            list.Add(element);
        }
        private void Add(TextBlock tb, Inline element)
        {
            SaveState(tb);
            Add(tb.Inlines, element);
        }
        private void Add(RichTextBox tb, Inline element)
        {
            SaveState(tb);
            tb.Document.Blocks.Add(new Paragraph(element));
        }
        private void Add(TextBlock tb, string element)
        {
            Add(tb, new Run(element));
        }
        private void Clear(InlineCollection list)
        {
            list.Clear();
        }
        private void Clear(params TextBlock[] textblocks)
        {
            foreach (TextBlock tb in textblocks)
            {
                SaveState(tb);
                Clear(tb.Inlines);
            }
        }
        private void SetBackground(Border b, Brush background)
        {
            SaveState(b);
            b.Background = background;
        }
        private void UnsetBackground(Border b)
        {
            SetBackground(b, Brushes.White);
        }
        private void SetBorderColor(Border b, Brush borderBrush)
        {
            SaveState(b);
            b.BorderBrush = borderBrush;
        }
        private void SetBorderStroke(Border b, double stroke)
        {
            SaveState(b);
            b.BorderThickness = new Thickness(stroke);
        }
        private void MarkBorder(Border b)
        {
            SetBorderColor(b, markBrush);
            SetBorderStroke(b, 2);
        }
        private void UnmarkBorder(Border b)
        {
            SetBorderColor(b, Brushes.Black);
            SetBorderStroke(b, 1);
        }
        private void SetShapeStrokeColor(Shape s, Brush brush)
        {
            SaveState(s);
            s.Stroke = brush;
        }
        private void SetShapeStroke(Shape s, double stroke)
        {
            SaveState(s);
            s.StrokeThickness = stroke;
        }
        private void MarkShape(Shape s)
        {
            SetShapeStrokeColor(s, markBrush);
            SetShapeStroke(s, 2);
        }
        private void UnmarkShape(Shape s)
        {
            SetShapeStrokeColor(s, Brushes.Black);
            SetShapeStroke(s, 1);
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

        #region Page Action creators

        public PageAction MarkCopyFromAction(Border[] borders)
        {
            return new PageAction(() =>
            {
                foreach (Border b in borders)
                {
                    SetBackground(b, copyBrush);
                }
            }, Undo);
        }

        public PageAction CopyAction(Border[] copyFrom, Border[] copyTo, bool replace = false)
        {
            return new PageAction(() =>
            {
                for (int i = 0; i < copyTo.Length; ++i)
                {
                    Border copyFromBorder = copyFrom[i];
                    TextBlock copyFromTextBlock = (TextBlock)copyFromBorder.Child;
                    Border copyToBorder = copyTo[i];
                    TextBlock copyToTextBlock = (TextBlock)copyToBorder.Child;
                    SetBackground(copyToBorder, copyBrush);
                    string text = ((Run)copyFromTextBlock.Inlines.LastInline).Text;
                    if (replace)
                    {
                        ReplaceLast(copyToTextBlock, text);
                    }
                    else
                    {
                        Add(copyToTextBlock, text);
                    }
                }
            }, Undo);
        }

        public PageAction UnmarkCopyAction(Border[] copyFrom, Border[] copyTo)
        {
            return new PageAction(() =>
            {
                foreach (Border b in copyFrom)
                {
                    UnsetBackground(b);
                }
                foreach (Border b in copyTo)
                {
                    UnsetBackground(b);
                }
            }, Undo);
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

        #endregion
    }

    #region NavigationInterface

    // I wish I had variadic templates in C# like in C++11 ...
    interface INavigationService<T1, T2, T3, T4>
    {
        // Save state of each element such that we can retrieve it later for undoing action.
        void SaveState(params T1[] t);

        void SaveState(params T2[] t);

        void SaveState(params T3[] t);

        void SaveState(params T4[] t);

        // Tells that current page action is finished and thus next calls to save state are for a new page action.
        void FinishPageAction();

        // Get list of actions which completely reverts the page action of the given index.
        Dictionary<int, Action> GetUndoActions();

        // Execute automatic undoing of actions.
        void Undo();
    }

    #endregion
}
