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

namespace Cryptool.Plugins.ChaCha
{
    /// <summary>
    /// Interaction logic for ChaChaPresentation.xaml
    /// </summary>
    [PluginBase.Attributes.Localization("Cryptool.Plugins.ChaCha.Properties.Resources")]
    public partial class ChaChaPresentation : UserControl, INotifyPropertyChanged
    {
        // List with pages in particular order to implement page navigation + their page actions
        private Page[] pageRouting;
        private int currentPageIndex = 0;
        private int currentActionIndex = 0;
        private ByteToHexStringConverter byteHexStringConverter = new ByteToHexStringConverter();

        struct UIElementAction
        {
            public ContentControl element; 
            public Func<object> content; // the content this UI element should be assigned
        }
        struct PageAction
        {
            public UIElementAction[] elementActions; // list of UIelement with the content they should be assigned
        }

        struct Page {
            public UIElement page;
            public PageAction[] actions; // implements hiding and showing of specific page elements to implement action navigation
            public int actionFrames {
                get
                {
                    return actions.Length;
                }
            }
        }

        public ChaChaPresentation()
        {
            InitializeComponent();
            DataContext = this;
            PageAction[] UIStateMatrixPageActions = new PageAction[]
            {
                new PageAction() {
                    elementActions = new UIElementAction[] {
                        new UIElementAction() { element = UITransformInput, content = () => byteHexStringConverter.Convert(Constants, Type.GetType("String"), null, null) },
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
                }
            };
            pageRouting = new Page[] {
                new Page() { page = UILandingPage, actions = new PageAction[0] },
                new Page() { page = UIWorkflowPage, actions = new PageAction[0] },
                new Page() { page = UIStateMatrixPage, actions = UIStateMatrixPageActions },
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #region Input
        private byte[] _constants;
        private byte[] _inputKey;
        private byte[] _inputIV;
        private byte[] _inputData;

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
            }
        }
        public String ConstantsChunks
        {
            get
            {

                return FourByteChunks(_constants);
            }
        }
        public String ConstantsLittleEndian
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 16; i+=4)
                {
                    sb.Append(FourByteChunks(ChaCha.getBytes(ChaCha.To4ByteLE(_constants, i))));
                }
                return sb.ToString();
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

        #region Navigation

        private bool _nextPageIsEnabled = false;
        public bool NextPageIsEnabled
        {
            get
            {
                return _nextPageIsEnabled;
            }
            set
            {
                _nextPageIsEnabled = value;
                OnPropertyChanged("NextPageIsEnabled");
            }
        }
        private bool _prevPageIsEnabled = false;
        public bool PrevPageIsEnabled
        {
            get
            {
                return _prevPageIsEnabled;
            }
            set
            {
                _prevPageIsEnabled = value;
                OnPropertyChanged("PrevPageIsEnabled");
            }
        }
        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            pageRouting[currentPageIndex].page.Visibility = Visibility.Collapsed;
            currentPageIndex--;
            pageRouting[currentPageIndex].page.Visibility = Visibility.Visible;
            currentActionIndex = 0;
            updatePageNavigation();
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            pageRouting[currentPageIndex].page.Visibility = Visibility.Collapsed;
            currentPageIndex++;
            pageRouting[currentPageIndex].page.Visibility = Visibility.Visible;
            currentActionIndex = 0;
            updatePageNavigation();
        }
        private bool _prevActionIsEnabled = false;
        public bool PrevActionIsEnabled
        {
            get
            {
                return _prevActionIsEnabled;
            }
            set
            {
                _prevActionIsEnabled = value;
                OnPropertyChanged("PrevActionIsEnabled");
            }
        }

        private bool _nextActionIsEnabled = false;
        public bool NextActionIsEnabled
        {
            get
            {
                return _nextActionIsEnabled;
            }
            set
            {
                _nextActionIsEnabled = value;
                OnPropertyChanged("NextActionIsEnabled");
            }
        }
        private void PrevAction_Click(object sender, RoutedEventArgs e)
        {

        }
        private void NextAction_Click(object sender, RoutedEventArgs e)
        {
            UIElementAction[] actions = pageRouting[currentPageIndex].actions[currentActionIndex].elementActions;
            for (int i = 0; i < actions.Length; i++)
            {
                Console.WriteLine(actions[i].element);
                Console.WriteLine(actions[i].content());
                actions[i].element.Content = actions[i].content();
            }
            currentActionIndex++;
            updatePageNavigation();
        }
        private void updatePageNavigation()
        {
            PrevPageIsEnabled = currentPageIndex != 0;
            NextPageIsEnabled = currentPageIndex != pageRouting.Length - 1;
            PrevActionIsEnabled = pageRouting[currentPageIndex].actionFrames > 0 && currentActionIndex != 0;
            NextActionIsEnabled = pageRouting[currentPageIndex].actionFrames > 0 && currentActionIndex != pageRouting[currentPageIndex].actionFrames;
        }

        #endregion

        #region ValueConversion

        private String FourByteChunks(byte[] b)
        {
            if (b == null)
                return "";
            StringBuilder chunk = new StringBuilder();
            for (int i = 0; i < b.Length; i += 4)
            {
                chunk.AppendFormat("{0:x2}{1:x2}{2:x2}{3:x2} ", b[i], b[i + 1], b[i + 2], b[i + 3]);
            }
            return chunk.ToString();

        }

        #endregion
    }

    [ValueConversion(typeof(byte[]), typeof(String))]
    public class ByteToHexStringConverter : IValueConverter
    {
        /** Return a hex representation of the byte array.*/
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            byte[] bytes = (byte[])value;
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:x2} ", b);
            return hex.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String hexString = (String)value;
            byte[] Bytes = new byte[hexString.Length / 2];
            int[] HexValue = new int[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

            for (int x = 0, i = 0; i < hexString.Length; i += 2, x += 1)
            {
                Bytes[x] = (byte)(HexValue[Char.ToUpper(hexString[i + 0]) - '0'] << 4 |
                                  HexValue[Char.ToUpper(hexString[i + 1]) - '0']);
            }

            return Bytes;
        }
    }
}
