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
        }

        #region Navigation

        #region properties

        struct PageAction
        {
            Action execAction;
            Action undoAction;
        }
        struct Page
        {
            UIElement _page; // the UI element which contains the page - the Visibility of this element will be set to Collapsed / Visible when going to next / previous page.
            PageAction[] _pageActions;
            public int ActionFrames
            {
                get
                {
                    return _pageActions.Length;
                }
            }
        }

        // List with pages in particular order to implement page navigation + their page actions
        private readonly Page[] _pageRouting;
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
                return _pageRouting[CurrentPageIndex];
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
                return _pageRouting.Length - 1;
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
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
        }
        private void PrevAction_Click(object sender, RoutedEventArgs e)
        {
        }
        private void NextAction_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion


        #region Data binding notification
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if(PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
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
