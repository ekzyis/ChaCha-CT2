using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaCha
{
    /// <summary>
    /// Interaction logic for ChaChaPresentation.xaml
    /// </summary>
    [PluginBase.Attributes.Localization("Cryptool.Plugins.ChaCha.Properties.Resources")]
    public partial class ChaChaPresentation : UserControl, INotifyPropertyChanged
    {
        private readonly IntermediateResultsManager<uint> uint_resultsManager;
        private readonly IntermediateResultsManager<uint[]> uint_array_resultsManager;
        private readonly ActionCache cache;
        public ChaChaPresentation()
        {
            uint_resultsManager = new IntermediateResultsManager<uint>();
            uint_array_resultsManager = new IntermediateResultsManager<uint[]>();
            cache = new ActionCache(this);
            InitializeComponent();
            InitVisualization();
            DataContext = this;
        }

        #region State Initialization Variables
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
        #endregion

        #region Settings Variables
        public ChaCha.Version Version { get; set; }
        public int Rounds { get; set; }
        #endregion

        #region IntermediateResultsManager API
        public void AddResult(ResultType<uint> type, uint result)
        {
            uint_resultsManager.AddResult(type, result);
        }
        public string GetHexResult(ResultType<uint> type, int index)
        {
            return HexString(uint_resultsManager.Get(type, index));
        }
        public void AddResult(ResultType<uint[]> type, uint[] result)
        {
            uint_array_resultsManager.AddResult(type, result);
        }
        public string GetHexResult(ResultType<uint[]> type, int i, int j)
        {
            return HexString(uint_array_resultsManager.Get(type, i)[j]);
        }
        public uint[] GetResult(ResultType<uint[]> type, int index)
        {
            return uint_array_resultsManager.Get(type, index);
        }
        public void ClearResults()
        {
            uint_resultsManager.Clear();
            uint_array_resultsManager.Clear();
        }
        #endregion

        #region Visualization Helper Methods (Stringify)
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
