using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Shapes;

namespace Cryptool.Plugins.ChaCha
{
    /// <summary>
    /// Interaction logic for ChaChaPresentation.xaml
    /// </summary>
    [PluginBase.Attributes.Localization("Cryptool.Plugins.ChaCha.Properties.Resources")]
    public partial class ChaChaPresentation : UserControl, INotifyPropertyChanged, INavigationService<TextBlock, Border, Shape>
    {
        #region private variables
        private Brush copyBrush = Brushes.AliceBlue;
        private Brush markBrush = Brushes.Purple;
        #endregion
        public ChaChaPresentation()
        {
            InitializeComponent();
            InitPages();
            DataContext = this;
        }

        #region Navigation

        #region interface methods
        /*
         * Stack with actions where the last dictionary contains undo actions which reverts changes from the last applied page action of an UI Element.
         */
        private Stack<Dictionary<int, Action>> _undoState = new Stack<Dictionary<int, Action>>();
        // temporary variable to collect undo actions before pushing into stack.
        private Dictionary<int, Action> _undoActions = new Dictionary<int, Action> ();
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
                    if(b.Background != null)
                    {
                        background = b.Background.Clone();
                    }
                    else
                    {
                        background = Brushes.White;
                    }
                    if(b.BorderBrush != null)
                    {
                        borderBrush = b.BorderBrush.Clone();
                    }
                    else
                    {
                        borderBrush = Brushes.Black;
                    }
                    if(b.BorderThickness != null)
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
            foreach(Action undo in undoActions.Values)
            {
                undo();
            }
        }
        #endregion

        #region page navigation

        #region classes, structs and methods related page navigation

        public struct PageAction
        {
            public Action exec;
            public Action undo;
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
            public PageAction[] Actions { 
                get
                {
                    return _pageActions.ToArray();
                }
            }
            public void AddAction(params PageAction[] pageActions)
            {
                foreach(PageAction pageAction in pageActions)
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
                get {
                    return _page.Visibility;
                }
                set {
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

        const int __START_VISUALIZATION_ON_PAGE_INDEX__ = 4;
        private void InitPages()
        {
            AddPage(LandingPage());
            AddPage(WorkflowPage());
            AddPage(StateMatrixPage());
            AddPage(KeystreamBlockGenPage());
            //AddPage(QuarterroundPage());
            CollapseAllPagesExpect(__START_VISUALIZATION_ON_PAGE_INDEX__);
        }

        // useful for development: setting pages visible for development purposes does not infer with execution
        private void CollapseAllPagesExpect(int pageIndex)
        {
            for(int i = 0; i < _pages.Count; ++i)
            {
                if (i != pageIndex)
                {
                    _pages[i].Visibility = Visibility.Collapsed;
                }
            }
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
                if(value != _currentPageIndex)
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
        private void ReplaceLast(TextBlock tb, Inline element)
        {
            SaveState(tb);
            ReplaceLast(tb.Inlines, element);
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
        private void UnboldLast(TextBlock tb)
        {
            SaveState(tb);
            UnboldLast(tb.Inlines);
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
        private void Add(TextBlock tb, string element)
        {
            Add(tb, new Run(element));
        }
        private void Clear(InlineCollection list)
        {
            list.Clear();
        }
        private void Clear(TextBlock tb)
        {
            SaveState(tb);
            Clear(tb.Inlines);
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
        #endregion

        #region Page Action creators

        public PageAction[] CreateCopyActions(Border[] copyFrom, Border[] copyTo)
        {
            Debug.Assert(copyFrom.Length == copyTo.Length);
            PageAction mark = new PageAction()
            {
                exec = () =>
                {
                    foreach (Border b in copyFrom)
                    {
                        SetBackground(b, copyBrush);
                    }
                },
                undo = Undo
            };
            PageAction copy = new PageAction()
            {
                exec = () =>
                {
                    for(int i = 0; i < copyTo.Length; ++i)
                    {
                        Border copyFromBorder = copyFrom[i];
                        TextBlock copyFromTextBlock = (TextBlock)copyFromBorder.Child;
                        Border copyToBorder = copyTo[i];
                        TextBlock copyToTextBlock = (TextBlock)copyToBorder.Child;
                        SetBackground(copyToBorder, copyBrush);
                        Add(copyToTextBlock, ((Run)copyFromTextBlock.Inlines.LastInline).Text);
                    }
                },
                undo = Undo
            };
            PageAction unmark = new PageAction()
            {
                exec = () =>
                {
                    foreach(Border b in copyFrom)
                    {
                        UnsetBackground(b);
                    }
                    foreach (Border b in copyTo)
                    {
                        UnsetBackground(b);
                    }
                },
                undo = Undo
            };
            return new PageAction[] { mark, copy, unmark };
        }
        #endregion

        #endregion

        #endregion

        #endregion

        #region Data binding notification
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if(PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion

        #region Visualization variables

        #region Input variables

        private byte[] _constants = new byte[0];
        private byte[] _inputKey = new byte[0];
        private byte[] _inputIV = new byte[0];
        private byte[] _initialCounter = new byte[0];
        private byte[] _inputData = new byte[0];
        
        public byte[] Constants
        {
            get
            {
                return _constants;
            }
            set
            {
                _constants = value;
                OnPropertyChanged("Constants");
                OnPropertyChanged("HexConstants");
                OnPropertyChanged("ConstantsChunks");
                OnPropertyChanged("ConstantsLittleEndian");
            }
        }
        public string HexConstants
        {
            get
            {
                return HexString(_constants);
            }
        }
        /* Constants splitted into 4 byte chunks */
        public string ConstantsChunks
        {
            get
            {
                // insert space after every 8 characters
                return Chunkify(HexString(_constants), 8);
            }
        }
        /* Constants with each 4 byte in little endian format*/
        public string ConstantsLittleEndian
        {
            get
            {
                return Chunkify(HexStringLittleEndian(_constants), 8);
            }
        }

        public byte[] InputKey
        {
            get
            {
                return _inputKey;
            }
            set
            {
                _inputKey = value;
                OnPropertyChanged("InputKey");
                OnPropertyChanged("HexInputKey");
                OnPropertyChanged("KeyChunks");
                OnPropertyChanged("KeyLittleEndian");
            }
        }
        public string HexInputKey
        {
            get
            {
                return HexString(_inputKey);
            }
        }
        public string KeyChunks
        {
            get
            {
                return Chunkify(HexString(_inputKey), 8);
            }
        }
        public string KeyLittleEndian
        {
            get
            {
                return Chunkify(HexStringLittleEndian(_inputKey), 8);
            }
        }
        public byte[] InputIV
        {
            get
            {
                return _inputIV;
            }
            set
            {
                _inputIV = value;
                OnPropertyChanged("InputIV");
                OnPropertyChanged("HexInputIV");
                OnPropertyChanged("IVChunks");
                OnPropertyChanged("IVLittleEndian");
            }
        }
        public string HexInputIV
        {
            get
            {
                return HexString(_inputIV);
            }
        }
        public string IVChunks
        {
            get
            {
                return Chunkify(HexString(_inputIV), 8);
            }
        }
        public string IVLittleEndian
        {
            get
            {
                return Chunkify(HexStringLittleEndian(_inputIV), 8);
            }
        }
        public byte[] InputData
        {
            get
            {
                return _inputData;
            }
            set
            {
                _inputData = value;
                OnPropertyChanged("InputData");
            }
        }
        public byte[] InitialCounter
        {
            get
            {
                return _initialCounter;
            }
            set
            {
                _initialCounter = value;
                OnPropertyChanged("InitialCounter");
                OnPropertyChanged("HexInitialCounter");
                OnPropertyChanged("InitialCounterChunks");
                OnPropertyChanged("InitialCounterLittleEndian");
            }
        }
        public string HexInitialCounter
        {
            get
            {
                return HexString(_initialCounter);
            }
        }
        public string InitialCounterChunks
        {
            get
            {
                return Chunkify(HexString(_initialCounter), 8);
            }
        }
        public string InitialCounterLittleEndian
        {
            get
            {
                return Chunkify(HexStringLittleEndian(_initialCounter), 8);
            }
        }

        private ChaCha.Version _version;
        public ChaCha.Version Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }

        #endregion

        #region interim results

        public enum ResultType
        {
            QR_INPUT_A, QR_INPUT_B, QR_INPUT_C, QR_INPUT_D,
            QR_INPUT_X1, QR_INPUT_X2, QR_INPUT_X3,
            QR_OUTPUT_X1, QR_OUTPUT_X2, QR_OUTPUT_X3,
            QR_ADD_X1_X2, QR_XOR
        }

        #region interim results manager class
        static class InterimResultsManager
        {
            private class InterimResultList
            {
                private List<uint> _results;
                private ResultType _type;
                public InterimResultList(ResultType type)
                {
                    _results = new List<uint>();
                    _type = type;
                }
                public ResultType Type
                {
                    get
                    {
                        return _type;
                    }
                }
                public string Hex(int index)
                {
                    return HexString(_results[index]);
                }
                public void Add(uint result)
                {
                    _results.Add(result);
                }
                public void Clear()
                {
                    _results.Clear();
                }
            }
            private static List<InterimResultList> _interimResultsList = new List<InterimResultList>();
            private static bool TypeExists(ResultType type)
            {
                return _interimResultsList.Exists(list => list.Type == type);
            }
            private static InterimResultList GetList(ResultType type)
            {
                if(!TypeExists(type))
                {
                    return null;
                }
                return _interimResultsList.Find(list => list.Type == type);
            }
            public static void Clear()
            {
                foreach(InterimResultList r in _interimResultsList)
                {
                    r.Clear();
                }
                _interimResultsList.Clear();
            }
            public static void AddResult(ResultType type, uint result)
            {
                if(!TypeExists(type))
                {
                    _interimResultsList.Add(new InterimResultList(type));
                }
                GetList(type).Add(result);
            }
            public static string Hex(ResultType type, int index)
            {
                InterimResultList list = GetList(type);
                if (list == null)
                {
                    throw new ArgumentException("InterimResultList of type {0} does not exist", type.ToString());
                }
                return list.Hex(index);
            }
        }
        public void AddResult(ResultType type, object result)
        {
            InterimResultsManager.AddResult(type, (uint)result);
        }
        public string GetHexResult(ResultType type, int index)
        {
            return InterimResultsManager.Hex(type, index);
        }
        public void clearResults()
        {
            InterimResultsManager.Clear();
        }

        #endregion

        #endregion

        #endregion

        #region Visualization helper methods


        /* insert a space after every n characters */
        private static string Chunkify(string text, int n)
        {
            string pattern = string.Format(".{{{0}}}", n);
            return Regex.Replace(text, pattern, "$0 ");
        }

        /* print a hex presentation of the byte array*/
        public static string HexString(byte[] bytes, int offset, int length)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = offset; i < offset + length; ++i)
            {
                sb.Append(bytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string HexString(byte[] bytes)
        {
            return HexString(bytes, 0, bytes.Length);
        }

        public static string HexString(uint u)
        {
            return HexString(ChaCha.GetBytes(u));
        }
        /* Write bytes as hex string with each 4 byte written in little-endian */
        public static string HexStringLittleEndian(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 4)
            {
                sb.Append(HexString(ChaCha.To4ByteLE(bytes, i)));
            }
            return sb.ToString();
        }
        #endregion
    }

    #region NavigationInterface

    // I wish I had variadic templates in C# like in C++11 ...
    interface INavigationService<T1, T2, T3>
    {
        // Save state of each element such that we can retrieve it later for undoing action.
        void SaveState(params T1[] t);

        void SaveState(params T2[] t);

        void SaveState(params T3[] t);

        // Tells that current page action is finished and thus next calls to save state are for a new page action.
        void FinishPageAction();

        // Get list of actions which completely reverts the page action of the given index.
        Dictionary<int, Action> GetUndoActions();

        // Execute automatic undoing of actions.
        void Undo();
    }

    #endregion
}
