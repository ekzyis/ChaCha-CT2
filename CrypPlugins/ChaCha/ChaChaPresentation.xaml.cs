﻿using Cryptool.PluginBase.Miscellaneous;
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
                        + " The first 16 bytes consist of the constants. ", highlight = UIElementAction.Highlight.BOLD }
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
                            action = UIElementAction.Action.ADD, highlight = UIElementAction.Highlight.BOLD },
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
                        String.Format("The last 16 bytes consist of the counter and the IV (in this order). Since the IV may vary between 8 and 12 bytes, the counter may vary between 8 and 4 bytes. You have chosen a {0}-byte IV. ", InputIV.Length)
                        + "First, we add the IV to the state. ", action = UIElementAction.Action.ADD, highlight = UIElementAction.Highlight.BOLD },
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
                        new UIElementAction() { element = UIStateMatrixStepDescription, content = () => "And then the counter. Since this is our first keystream block, we set the counter to 0.",
                            action = UIElementAction.Action.ADD, highlight = UIElementAction.Highlight.BOLD },
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
                }
                #endregion
            };
            _pageRouting = new Page[] {
                new Page() { page = UILandingPage, actions = new PageAction[0] },
                new Page() { page = UIWorkflowPage, actions = new PageAction[0] },
                new Page() { page = UIStateMatrixPage, actions = UIStateMatrixPageActions },
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
                NONE,
                BOLD,
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
            public int actionFrames
            {
                get
                {
                    return actions.Length;
                }
            }
        }

        // List with pages in particular order to implement page navigation + their page actions
        private Page[] _pageRouting;
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
                _currentPageIndex = value;
                OnPropertyChanged("CurrentPageIndex");
                OnPropertyChanged("CurrentPage");
                OnPropertyChanged("NextPageIsEnabled");
                OnPropertyChanged("PrevPageIsEnabled");
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
                return CurrentPage.actionFrames > 0 && CurrentActionIndex != CurrentPage.actionFrames;
            }
        }
        public bool PrevActionIsEnabled
        {
            get
            {
                return CurrentPage.actionFrames > 0 && CurrentActionIndex != 0;
            }
        }
        #endregion

        #region Click handlers
        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage.page.Visibility = Visibility.Collapsed;
            CurrentPageIndex--;
            CurrentPage.page.Visibility = Visibility.Visible;
            CurrentActionIndex = 0;
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage.page.Visibility = Visibility.Collapsed;
            CurrentPageIndex++;
            CurrentPage.page.Visibility = Visibility.Visible;
            CurrentActionIndex = 0;
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
                                    Run r = createRunFromAction(prevAction_);
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
                    removeLast(prevAction.element.Inlines);
                }
            }
            CurrentActionIndex--;
        }
        private void NextAction_Click(object sender, RoutedEventArgs e)
        {
            // unhighlight element added in previous action
            foreach (UIElementAction uie in PreviousActions)
            {
                Run r = createRunFromAction(uie, false);
                removeLast(uie.element.Inlines);
                uie.element.Inlines.Add(r);
            }
            foreach (UIElementAction uie in CurrentActions)
            {
                Run r = createRunFromAction(uie);
                if (uie.action == UIElementAction.Action.REPLACE)
                {
                    uie.element.Inlines.Clear();
                }
                uie.element.Inlines.Add(r);
            }
            CurrentActionIndex++;
        }

        private Run createRunFromAction(UIElementAction a, bool applyHighlightIfSpecified = true)
        {
            return new Run { Text = a.content(), FontWeight = a.highlight == UIElementAction.Highlight.BOLD && applyHighlightIfSpecified ? FontWeights.Bold : FontWeights.Normal };
        }

        private void removeLast(InlineCollection list)
        {
            list.Remove(list.LastInline);
        }

        #endregion


        #region Data binding notification
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
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
        public String HexConstants
        {
            get
            {
                return hexString(_constants);
            }
        }
        /* Constants splitted into 4 byte chunks */
        public String ConstantsChunks
        {
            get
            {
                // insert space after every 8 characters
                return chunkify(hexString(_constants), 8);
            }
        }
        /* Constants with each 4 byte in little endian format*/
        public String ConstantsLittleEndian
        {
            get
            {
                return chunkify(hexStringLittleEndian(_constants), 8);
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
        public String HexInputKey
        {
            get
            {
                return hexString(_inputKey);
            }
        }
        public String KeyChunks
        {
            get
            {
                return chunkify(hexString(_inputKey), 8);
            }
        }
        public String KeyLittleEndian
        {
            get
            {
                return chunkify(hexStringLittleEndian(_inputKey), 8);
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
        public String HexInputIV
        {
            get
            {
                return hexString(_inputIV);
            }
        }
        public String IVChunks
        {
            get
            {
                return chunkify(hexString(_inputIV), 8);
            }
        }
        public String IVLittleEndian
        {
            get
            {
                return chunkify(hexStringLittleEndian(_inputIV), 8);
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
        public String HexInitialCounter
        {
            get
            {
                return hexString(_initialCounter);
            }
        }
        public String InitialCounterChunks
        {
            get
            {
                return chunkify(hexString(_initialCounter), 8);
            }
        }
        public String InitialCounterLittleEndian
        {
            get
            {
                return chunkify(hexStringLittleEndian(_initialCounter), 8);
            }
        }
        #endregion

        #region State
        private String _state0;
        private String _state1;
        private String _state2;
        private String _state3;
        private String _state4;
        private String _state5;
        private String _state6;
        private String _state7;
        private String _state8;
        private String _state9;
        private String _state10;
        private String _state11;
        private String _state12;
        private String _state13;
        private String _state14;
        private String _state15;
        public String State0
        {
            get
            {
                return _state0;
            }
            set
            {
                _state0 = value;
                OnPropertyChanged("State0");
            }
        }
        public String State1
        {
            get
            {
                return _state1;
            }
            set
            {
                _state1 = value;
                OnPropertyChanged("State1");
            }
        }
        public String State2
        {
            get
            {
                return _state2;
            }
            set
            {
                _state2 = value;
                OnPropertyChanged("State2");
            }
        }
        public String State3
        {
            get
            {
                return _state3;
            }
            set
            {
                _state3 = value;
                OnPropertyChanged("State3");
            }
        }
        public String State4
        {
            get
            {
                return _state4;
            }
            set
            {
                _state4 = value;
                OnPropertyChanged("State4");
            }
        }
        public String State5
        {
            get
            {
                return _state5;
            }
            set
            {
                _state5 = value;
                OnPropertyChanged("State5");
            }
        }
        public String State6
        {
            get
            {
                return _state6;
            }
            set
            {
                _state6 = value;
                OnPropertyChanged("State6");
            }
        }
        public String State7
        {
            get
            {
                return _state7;
            }
            set
            {
                _state7 = value;
                OnPropertyChanged("State7");
            }
        }
        public String State8
        {
            get
            {
                return _state8;
            }
            set
            {
                _state8 = value;
                OnPropertyChanged("State8");
            }
        }
        public String State9
        {
            get
            {
                return _state9;
            }
            set
            {
                _state9 = value;
                OnPropertyChanged("State9");
            }
        }
        public String State10
        {
            get
            {
                return _state10;
            }
            set
            {
                _state10 = value;
                OnPropertyChanged("State10");
            }
        }
        public String State11
        {
            get
            {
                return _state11;
            }
            set
            {
                _state11 = value;
                OnPropertyChanged("State11");
            }
        }
        public String State12
        {
            get
            {
                return _state12;
            }
            set
            {
                _state12 = value;
                OnPropertyChanged("State12");
            }
        }
        public String State13
        {
            get
            {
                return _state13;
            }
            set
            {
                _state13 = value;
                OnPropertyChanged("State13");
            }
        }
        public String State14
        {
            get
            {
                return _state14;
            }
            set
            {
                _state14 = value;
                OnPropertyChanged("State14");
            }
        }
        public String State15
        {
            get
            {
                return _state15;
            }
            set
            {
                _state15 = value;
                OnPropertyChanged("State15");
            }
        }
        #endregion

        #region ValueConversion


        /* insert a space after every n characters */
        private String chunkify(string text, int n)
        {
            string pattern = String.Format(".{{{0}}}", n);
            return Regex.Replace(text, pattern, "$0 ");
        }

        /* print a hex presentation of the byte array*/
        public string hexString(byte[] bytes, int offset, int length)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = offset; i < offset + length; ++i)
            {
                sb.Append(bytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public string hexString(byte[] bytes)
        {
            return hexString(bytes, 0, bytes.Length);
        }

        public string hexString(uint u)
        {
            return hexString(ChaCha.getBytes(u));
        }
        /* Write bytes as hex string with each 4 byte written in little-endian */
        public String hexStringLittleEndian(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 4)
            {
                sb.Append(hexString(ChaCha.To4ByteLE(bytes, i)));
            }
            return sb.ToString();
        }
        #endregion
    }
}
