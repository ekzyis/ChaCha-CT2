using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        // List which indicates page order to implement page navigation
        private UIElement[] pageRouting;
        private int currentPageIndex = 0;

        public ChaChaPresentation()
        {
            InitializeComponent();
            DataContext = this;
            pageRouting = new UIElement[] { UILandingPage, UIWorkflowPage, UIStateMatrixPage };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #region Input
        private String _constants;
        private String _inputKey;
        private String _inputIV;
        private String _inputData;

        public String Constants
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

        public String InputKey
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

        public String InputIV
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

        public String InputData
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

        #region Transform
        private String transformInput;
        private String transformChunks;
        private String transformLittleEndian;
        public String TransformInput
        {
            get
            {
                return transformInput;
            }
            set
            {
                transformInput = value;
                OnPropertyChanged("TransformInput");
            }
        }
        public String TransformChunks
        {
            get
            {
                return transformChunks;
            }
            set
            {
                transformChunks = value;
                OnPropertyChanged("TransformChunks");
            }
        }
        public String TransformLittleEndian
        {
            get
            {
                return transformLittleEndian;
            }
            set
            {
                transformLittleEndian = value;
                OnPropertyChanged("TransformLittleEndian");
            }
        }
        #endregion

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
            pageRouting[currentPageIndex].Visibility = Visibility.Collapsed;
            currentPageIndex--;
            pageRouting[currentPageIndex].Visibility = Visibility.Visible;
            updatePageNavigation();
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            pageRouting[currentPageIndex].Visibility = Visibility.Collapsed;
            currentPageIndex++;
            pageRouting[currentPageIndex].Visibility = Visibility.Visible;
            updatePageNavigation();
        }
        private void updatePageNavigation()
        {
            PrevPageIsEnabled = currentPageIndex != 0;
            NextPageIsEnabled = currentPageIndex != pageRouting.Length - 1;
        }
    }
}
