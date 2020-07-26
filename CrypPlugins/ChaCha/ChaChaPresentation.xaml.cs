using Cryptool.PluginBase.Miscellaneous;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Security.Policy;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using System.Data;

namespace Cryptool.Plugins.ChaCha
{
    /// <summary>
    /// Interaction logic for ChaChaPresentation.xaml
    /// </summary>
    [PluginBase.Attributes.Localization("Cryptool.Plugins.ChaCha.Properties.Resources")]
    public partial class ChaChaPresentation : UserControl, INotifyPropertyChanged, INavigationService<TextBlock>
    {

        public ChaChaPresentation()
        {
            InitializeComponent();
            InitPages();
            DataContext = this;
        }

        private void InitPages()
        {
            const int __START_VISUALIZATION_ON_PAGE_INDEX__ = 2;
            AddPage(LandingPage());
            AddPage(WorkflowPage());
            AddPage(StateMatrixPage());
            for(int i = 0; i < __START_VISUALIZATION_ON_PAGE_INDEX__; ++i)
            {
                NextPage_Click(null, null);
            }
            AddPage(KeystreamBlockGenPage());
        }

        #region Navigation

        #region interface methods
        /*
         * Stack with actions where the last dictionary contains undo actions which reverts changes from the last applied page action of an UI Element.
         */
        private Stack<Dictionary<int, Action>> _undoState = new Stack<Dictionary<int, Action>>();
        // temporary variable to collect undo actions before pushing into stack.
        private Dictionary<int, Action> _undoActions = new Dictionary<int, Action> ();
        public void SaveState(params TextBlock[] textblocks)
        {
            foreach(TextBlock tb in textblocks)
            {
                // copy inline elements
                Inline[] state = new Inline[tb.Inlines.Count];
                tb.Inlines.CopyTo(state, 0);
                // do not overwrite states since first added state was the "most original one"
                if(!_undoActions.ContainsKey(tb.GetHashCode()))
                {
                    _undoActions[tb.GetHashCode()] = () => {
                        tb.Inlines.Clear();
                        foreach (Inline i in state)
                        {
                            tb.Inlines.Add(i);
                        }
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

        #region properties

        struct PageAction
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
            public void AddAction(PageAction pageAction)
            {
                _pageActions.Add(pageAction);
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

        public bool ExecutionFinished
        {
            get
            {
                return _executionFinished;
            }
            set
            {
                _executionFinished = value;
                OnPropertyChanged("NextPageIsEnabled");
                OnPropertyChanged("PrevPageIsEnabled");
                OnPropertyChanged("NextActionIsEnabled");
                OnPropertyChanged("PrevActionIsEnabled");
            }
        }

        public int MaxPageIndex
        {
            get
            {
                return _pages.Count - 1;
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

        #region Click handlers
        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage.Visibility = Visibility.Collapsed;
            UndoActions();
            CurrentPageIndex--;
            CurrentPage.Visibility = Visibility.Visible;
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage.Visibility = Visibility.Collapsed;
            UndoActions();
            CurrentPageIndex++;
            CurrentPage.Visibility = Visibility.Visible;
            foreach(PageAction pageAction in CurrentPage.InitActions)
            {
                pageAction.exec();
                FinishPageAction();
            }
        }
        private void PrevAction_Click(object sender, RoutedEventArgs e)
        {
            CurrentActionIndex--;
            CurrentPage.Actions[CurrentActionIndex].undo();
        }
        private void NextAction_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage.Actions[CurrentActionIndex].exec();
            FinishPageAction();
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
            foreach(PageAction pageAction in CurrentPage.InitActions.Reverse())
            {
                pageAction.undo();
            }
        }
        #endregion

        #region action helper methods

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
        private void Clear(InlineCollection list)
        {
            list.Clear();
        }
        private void Clear(TextBlock tb)
        {
            SaveState(tb);
            Clear(tb.Inlines);
        }

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

        #region Input
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

        #region ValueConversion


        /* insert a space after every n characters */
        private string Chunkify(string text, int n)
        {
            string pattern = string.Format(".{{{0}}}", n);
            return Regex.Replace(text, pattern, "$0 ");
        }

        /* print a hex presentation of the byte array*/
        public string HexString(byte[] bytes, int offset, int length)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = offset; i < offset + length; ++i)
            {
                sb.Append(bytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public string HexString(byte[] bytes)
        {
            return HexString(bytes, 0, bytes.Length);
        }

        public string HexString(uint u)
        {
            return HexString(ChaCha.GetBytes(u));
        }
        /* Write bytes as hex string with each 4 byte written in little-endian */
        public string HexStringLittleEndian(byte[] bytes)
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

    interface INavigationService<T>
    {
        // Save state of each element such that we can retrieve it later for undoing action.
        void SaveState(params T[] t);

        // Tells that current page action is finished and thus next calls to save state are for a new page action.
        void FinishPageAction();

        // Get list of actions which completely reverts the page action of the given index.
        Dictionary<int, Action> GetUndoActions();

        // Execute automatic undoing of actions.
        void Undo();
    }
}
