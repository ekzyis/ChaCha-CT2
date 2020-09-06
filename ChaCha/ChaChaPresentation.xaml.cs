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
    public partial class ChaChaPresentation : UserControl, INotifyPropertyChanged
    {
        #region private variables

        #endregion
        public ChaChaPresentation()
        {
            ContentControl c = new ContentControl();
            InitializeComponent();
            InitVisualization();
            //InitNavigationPageClickHandlers();
            DataContext = this;
        }

        #region Data binding notification
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
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
        public string[] ConstantsChunks
        {
            get
            {
                // insert space after every 8 characters
                return Chunkify(HexString(_constants), 8).Split(' ');
            }
        }
        /* Constants with each 4 byte in little endian format*/
        public string[] ConstantsLittleEndian
        {
            get
            {
                return Chunkify(HexStringLittleEndian(_constants), 8).Split(' ');
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
        public string[] KeyChunks
        {
            get
            {
                return Chunkify(HexString(_inputKey), 8).Split(' ');
            }
        }
        public string[] KeyLittleEndian
        {
            get
            {
                return Chunkify(HexStringLittleEndian(_inputKey), 8).Split(' ');
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
        public string[] IVChunks
        {
            get
            {
                return Chunkify(HexString(_inputIV), 8).Split(' ');
            }
        }
        public string[] IVLittleEndian
        {
            get
            {
                return Chunkify(HexStringLittleEndian(_inputIV), 8).Split(' ');
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
        public string[] InitialCounterChunks
        {
            get
            {
                return Chunkify(HexString(_initialCounter), 8).Split(' ');
            }
        }
        public string[] InitialCounterLittleEndian
        {
            get
            {
                return Chunkify(HexStringLittleEndian(_initialCounter), 8).Split(' ');
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

        private int _rounds;
        public int Rounds
        {
            get
            {
                return _rounds;
            }
            set
            {
                _rounds = value;
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
                    throw new ArgumentException(string.Format("InterimResultList of type {0}, index {1} does not exist", type.ToString(), index));
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
}
