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

namespace Cryptool.Plugins.ChaCha
{
    /// <summary>
    /// Interaction logic for ChaChaPresentation.xaml
    /// </summary>
    [PluginBase.Attributes.Localization("Cryptool.Plugins.ChaCha.Properties.Resources")]
    public partial class ChaChaPresentation : UserControl, INotifyPropertyChanged
    {

        public ChaChaPresentation()
        {
            InitializeComponent();
            DataContext = this;
            PageAction UIStateMatrixPageKey16Action = new PageAction()
            {
                elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UIState4, content = () => KeyLittleEndian.Replace(" ", "").Substring(0, 8) },
                        new UIElementAction() { element = UIState5, content = () => KeyLittleEndian.Replace(" ", "").Substring(8, 8) },
                        new UIElementAction() { element = UIState6, content = () => KeyLittleEndian.Replace(" ", "").Substring(16, 8) },
                        new UIElementAction() { element = UIState7, content = () => KeyLittleEndian.Replace(" ", "").Substring(24, 8) },
                        new UIElementAction() { element = UIState8, content = () => KeyLittleEndian.Replace(" ", "").Substring(0, 8) },
                        new UIElementAction() { element = UIState9, content = () => KeyLittleEndian.Replace(" ", "").Substring(8, 8) },
                        new UIElementAction() { element = UIState10, content = () => KeyLittleEndian.Replace(" ", "").Substring(16, 8) },
                        new UIElementAction() { element = UIState11, content = () => KeyLittleEndian.Replace(" ", "").Substring(24, 8) },
                    }
            };
            PageAction UIStateMatrixPageKey32Action = new PageAction()
            {
                elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UIState4, content = () => KeyLittleEndian.Replace(" ", "").Substring(0, 8) },
                        new UIElementAction() { element = UIState5, content = () => KeyLittleEndian.Replace(" ", "").Substring(8, 8) },
                        new UIElementAction() { element = UIState6, content = () => KeyLittleEndian.Replace(" ", "").Substring(16, 8) },
                        new UIElementAction() { element = UIState7, content = () => KeyLittleEndian.Replace(" ", "").Substring(24, 8) },
                        new UIElementAction() { element = UIState8, content = () => KeyLittleEndian.Replace(" ", "").Substring(32, 8) },
                        new UIElementAction() { element = UIState9, content = () => KeyLittleEndian.Replace(" ", "").Substring(40, 8) },
                        new UIElementAction() { element = UIState10, content = () => KeyLittleEndian.Replace(" ", "").Substring(48, 8) },
                        new UIElementAction() { element = UIState11, content = () => KeyLittleEndian.Replace(" ", "").Substring(56, 8) },
                    }
            };
            PageAction UIStateMatrixPageIV8Action = new PageAction()
            {
                elementActions = new UIElementAction[]
                {
                    new UIElementAction() { element = UIState14, content = () => IVLittleEndian.Replace(" ", "").Substring(0, 8) },
                    new UIElementAction() { element = UIState14, content = () => IVLittleEndian.Replace(" ", "").Substring(8, 8) }
                }
            };
            PageAction UIStateMatrixPageIV12Action = new PageAction()
            {
                elementActions = new UIElementAction[]
                {
                    new UIElementAction() { element = UIState13, content = () => IVLittleEndian.Replace(" ", "").Substring(0, 8) },
                    new UIElementAction() { element = UIState14, content = () => IVLittleEndian.Replace(" ", "").Substring(8, 8) },
                    new UIElementAction() { element = UIState15, content = () => IVLittleEndian.Replace(" ", "").Substring(16, 8) }
                }
            };
            PageAction[] UIStateMatrixPageActions = new PageAction[]
            {
                #region Write Constants into State Matrix
                new PageAction()
                {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UIStateMatrixStepDescription, content = () => "The 512-bit (128-byte) ChaCha state can be interpreted as a 4x4 matrix, where each entry consists of 4 bytes interpreted as little-endian. " 
                        + " The first 16 bytes consist of the constants. " }
                    }
                },
                new PageAction() {
                    elementActions = new UIElementAction[] {
                        new UIElementAction() { element = UITransformInput, content = () => HexConstants },
                    },
                },
                new PageAction()
                {
                    elementActions = new UIElementAction[] {
                        new UIElementAction() { element = UITransformChunks, content = () => ConstantsChunks },
                    }
                },
                new PageAction()
                {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UITransformLittleEndian, content = () => ConstantsLittleEndian }
                    }
                },
                new PageAction()
                {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UIState0, content = () => ConstantsLittleEndian.Replace(" ", "").Substring(0, 8) },
                        new UIElementAction() { element = UIState1, content = () => ConstantsLittleEndian.Replace(" ", "").Substring(8, 8) },
                        new UIElementAction() { element = UIState2, content = () => ConstantsLittleEndian.Replace(" ", "").Substring(16, 8) },
                        new UIElementAction() { element = UIState3, content = () => ConstantsLittleEndian.Replace(" ", "").Substring(24, 8) },
                    }
                },
                #endregion
                #region Write Key into State Matrix
                new PageAction()
                {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UIStateMatrixStepDescription, content = () => "The next 32 bytes consist of the key. If the key consists of only 16 bytes, it is concatenated with itself. ",
                            action = UIElementAction.Action.ADD },
                        new UIElementAction() { element = UITransformInput, content = () => "" },
                        new UIElementAction() { element = UITransformChunks, content = () => "" },
                        new UIElementAction() { element = UITransformLittleEndian, content = () => "" },
                    }
                },
                new PageAction()
                {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UITransformInput, content = () => HexInputKey },
                    }
                },
                new PageAction()
                {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UITransformChunks, content = () => KeyChunks }
                    }
                },
                new PageAction()
                {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UITransformLittleEndian, content = () => KeyLittleEndian }
                    }
                },
                InputKey.Length == 16 ? UIStateMatrixPageKey16Action : UIStateMatrixPageKey32Action,
                #endregion
                #region Write IV into State Matrix
                new PageAction()
                {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UIStateMatrixStepDescription, content = () =>
                        string.Format("The last 16 bytes consist of the counter and the IV (in this order). Since the IV may vary between 8 and 12 bytes, the counter may vary between 8 and 4 bytes. You have chosen a {0}-byte IV. ", InputIV.Length)
                        + "First, we add the IV to the state. ", action = UIElementAction.Action.ADD },
                        new UIElementAction() { element = UITransformInput, content = () => "" },
                        new UIElementAction() { element = UITransformChunks, content = () => "" },
                        new UIElementAction() { element = UITransformLittleEndian, content = () => "" },
                    }
                },
                new PageAction()
                {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UITransformInput, content = () => HexInputIV },
                    }
                },
                new PageAction()
                {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UITransformChunks, content = () => IVChunks }
                    }
                },
                new PageAction()
                {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UITransformLittleEndian, content = () => IVLittleEndian }
                    }
                },
                InputIV.Length == 8 ? UIStateMatrixPageIV8Action : UIStateMatrixPageIV12Action,
                #endregion
                #region Write Counter into State Matrix
                new PageAction()
                {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UIStateMatrixStepDescription, content = () => "And then the counter. Since this is our first keystream block, we set the counter to 0. ",
                            action = UIElementAction.Action.ADD },
                        new UIElementAction() { element = UITransformInput, content = () => "" },
                        new UIElementAction() { element = UITransformChunks, content = () => "" },
                        new UIElementAction() { element = UITransformLittleEndian, content = () => "" },
                    }
                },
                new PageAction()
                {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UITransformInput, content = () => HexInitialCounter },
                    }
                },
                new PageAction()
                {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UITransformChunks, content = () => InitialCounterChunks }
                    }
                },
                new PageAction()
                {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UITransformLittleEndian, content = () => InitialCounterLittleEndian }
                    }
                },
                InputIV.Length == 8 ? new PageAction() {
                    elementActions = new UIElementAction[] {
                        new UIElementAction() { element = UIState12, content = () => InitialCounterLittleEndian.Replace(" ", "").Substring(0, 8) },
                        new UIElementAction() { element = UIState13, content = () => InitialCounterLittleEndian.Replace(" ", "").Substring(8, 8) },
                    }
                }: new PageAction() {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UIState12, content = () => InitialCounterLittleEndian.Replace(" ", "").Substring(0, 8) },
                    }
                },
                #endregion
                #region Inform user about next step
                new PageAction()
                {
                    elementActions = new UIElementAction[]
                    {
                        new UIElementAction() { element = UIStateMatrixStepDescription, content = () => "On the next page, we will use this initialized state matrix to generate the first keystream block.",
                            action = UIElementAction.Action.ADD }
                    }
                }
                #endregion
            };
            _pageRouting = new Page[] {
                new Page() { page = UILandingPage, actions = new PageAction[0] },
                new Page() { page = UIWorkflowPage, actions = new PageAction[0] },
                new Page() { page = UIStateMatrixPage, actions = UIStateMatrixPageActions },
                new Page() { page = UIKeystreamBlockGenPage, actions = new PageAction[0] },
            };
        }

        #region Navigation

        #region properties
        struct UIElementAction
        {
            public TextBlock element;
            public Func<string> content; // the content this UI element should be assigned
            public Action action;
            public Highlight highlight; // highlight the UI element for this action (will be unhighlighted next action)
            public enum Action
            {
                REPLACE,
                ADD
            }
            public enum Highlight
            {
                BOLD,
                NONE,
            }
        }
        struct PageAction
        {
            public UIElementAction[] elementActions; // list of UIelement with the content they should be assigned
        }

        struct Page
        {
            public UIElement page;
            public PageAction[] actions; // implements hiding and showing of specific page elements to implement action navigation
            public int ActionFrames
            {
                get
                {
                    return actions.Length;
                }
            }
        }

        // List with pages in particular order to implement page navigation + their page actions
        private readonly Page[] _pageRouting;
        private int _currentPageIndex = 0;
        private int _currentActionIndex = 0;

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
                return _pageRouting[CurrentPageIndex];
            }
        }

        private UIElementAction[] CurrentActions
        {
            get
            {
                return CurrentPage.actions[_currentActionIndex].elementActions;
            }
        }
        private UIElementAction[] PreviousActions
        {
            get
            {
                if(_currentActionIndex == 0)
                {
                    return new UIElementAction[0];
                }
                return CurrentPage.actions[_currentActionIndex - 1].elementActions;
            }
        }

        public bool NextPageIsEnabled
        {
            get
            {
                return CurrentPageIndex != _pageRouting.Length - 1; ;
            }
        }
        public bool PrevPageIsEnabled
        {
            get
            {
                return CurrentPageIndex != 0;
            }
        }
        public bool NextActionIsEnabled
        {
            get
            {
                // next action is only enabled if currentActionIndex is not already pointing outside of actionFrames array since we increase it after each action.
                // For example, if there are two action frames, we start with currentActionIndex = 0 and increase it each click _after_ we have processed the index
                // to retrieve the actions we need to take. After two clicks, we are at currentActionIndex = 2 which is the first invalid index.
                return CurrentPage.ActionFrames > 0 && CurrentActionIndex != CurrentPage.ActionFrames;
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
            CurrentPage.page.Visibility = Visibility.Collapsed;
            // undo actions on current page before switching
            UndoActions();
            CurrentPageIndex--;
            CurrentPage.page.Visibility = Visibility.Visible;
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage.page.Visibility = Visibility.Collapsed;
            // undo actions on current page before switching
            UndoActions();
            CurrentPageIndex++;
            CurrentPage.page.Visibility = Visibility.Visible;
        }
        private void PrevAction_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElementAction prevAction in PreviousActions)
            {
                if (prevAction.action == UIElementAction.Action.REPLACE)
                {
                    prevAction.element.Inlines.Clear();
                    if (CurrentActionIndex == 0)
                    {
                        // if the last action was the first action, we don't have to add specific inline content
                        // since we can assume that the elements never have any inline content in them at the start (at least this is the case up to now)
                        break;
                    }
                    else
                    {
                        // get the inline content what the element has had before the last action was applied.
                        // This is done by traversing the previous PageActions and checking it there is a UIElementAction applying an action to the same UIElement.
                        // The content function of that previous UIElementAction with the same UIElement gives us the correct content for undoing the last action.
                        for (int j = CurrentActionIndex - 2; j >= 0; --j)
                        {
                            foreach (UIElementAction prevAction_ in CurrentPage.actions[j].elementActions)
                            {
                                if (prevAction_.element.Name == prevAction.element.Name)
                                {
                                    Run r = CreateRunFromAction(prevAction_);
                                    prevAction_.element.Inlines.Add(r);
                                    goto End; // goto to break out of double for-loop
                                }
                            }
                        }
                    End:;
                    }
                }
                else if (prevAction.action == UIElementAction.Action.ADD)
                {
                    // Remove the last inline element that was added to undo action
                    RemoveLast(prevAction.element.Inlines);
                }
            }
            CurrentActionIndex--;
            // Re-highlight previously highlighted elements
            foreach(UIElementAction prevAction in PreviousActions)
            {
                ReplaceLast(prevAction.element.Inlines, CreateRunFromAction(prevAction));
            }
        }
        private void NextAction_Click(object sender, RoutedEventArgs e)
        {
            // unhighlight element added in previous action
            foreach (UIElementAction uie in PreviousActions)
            {
                ReplaceLast(uie.element.Inlines, CreateRunFromAction(uie, false));
            }
            foreach (UIElementAction uie in CurrentActions)
            {
                Run r = CreateRunFromAction(uie);
                if (uie.action == UIElementAction.Action.REPLACE)
                {
                    uie.element.Inlines.Clear();
                }
                uie.element.Inlines.Add(r);
            }
            CurrentActionIndex++;
        }

        private void UndoActions()
        {
            // undo all actions by simulating clicking as many times on PREV_ACTION as clicked on NEXT_ACTION
            for(int i = CurrentActionIndex; i > 0; --i)
            {
                PrevAction_Click(null, null);
            }
            Debug.Assert(CurrentActionIndex == 0);
        }

        private Run CreateRunFromAction(UIElementAction a, bool applyHighlightIfSpecified = true)
        {
            return new Run { Text = a.content(), FontWeight = a.highlight == UIElementAction.Highlight.BOLD && applyHighlightIfSpecified ? FontWeights.Bold : FontWeights.Normal };
        }

        private void RemoveLast(InlineCollection list)
        {
            list.Remove(list.LastInline);
        }

        private void ReplaceLast(InlineCollection list, Run r)
        {
            RemoveLast(list);
            list.Add(r);
        }

        #endregion


        #region Data binding notification
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion

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
}
