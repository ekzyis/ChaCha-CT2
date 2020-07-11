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

        #region StateParams
        private String _stateParamsConstant;
        private String _stateParamsKey;
        private String _stateParamsIV;
        private String _stateParamsInput;

        public String StateParamsConstant
        {
            get
            {
                return _stateParamsConstant;
            }
            set
            {
                _stateParamsConstant = value;
                OnPropertyChanged("StateParamsConstant");
            }
        }

        public String StateParamsKey
        {
            get
            {
                return _stateParamsKey;
            }
            set
            {
                _stateParamsKey = value;
                OnPropertyChanged("StateParamsKey");
            }
        }

        public String StateParamsIV
        {
            get
            {
                return _stateParamsIV;
            }
            set
            {
                _stateParamsIV = value;
                OnPropertyChanged("StateParamsIV");
            }
        }

        public String StateParamsInput
        {
            get
            {
                return _stateParamsInput;
            }
            set
            {
                _stateParamsInput = value;
                OnPropertyChanged("StateParamsInput");
            }
        }
        #endregion

        #region State
        private String _stateC0;
        private String _stateC1;
        private String _stateC2;
        private String _stateC3;
        private String _stateK0;
        private String _stateK1;
        private String _stateK2;
        private String _stateK3;
        private String _stateK4;
        private String _stateK5;
        private String _stateK6;
        private String _stateK7;
        private String _stateInput0;
        private String _stateInput1;
        private String _stateInput2;
        private String _stateInput3;
        public String StateC0
        {
            get
            {
                return _stateC0;
            }
            set
            {
                _stateC0 = value;
                OnPropertyChanged("StateC0");
            }
        }
        public String StateC1
        {
            get
            {
                return _stateC1;
            }
            set
            {
                _stateC1 = value;
                OnPropertyChanged("StateC1");
            }
        }
        public String StateC2
        {
            get
            {
                return _stateC2;
            }
            set
            {
                _stateC2 = value;
                OnPropertyChanged("StateC2");
            }
        }
        public String StateC3
        {
            get
            {
                return _stateC3;
            }
            set
            {
                _stateC3 = value;
                OnPropertyChanged("StateC3");
            }
        }
        public String StateK0
        {
            get
            {
                return _stateK0;
            }
            set
            {
                _stateK0 = value;
                OnPropertyChanged("StateK0");
            }
        }
        public String StateK1
        {
            get
            {
                return _stateK1;
            }
            set
            {
                _stateK1 = value;
                OnPropertyChanged("StateK1");
            }
        }
        public String StateK2
        {
            get
            {
                return _stateK2;
            }
            set
            {
                _stateK2 = value;
                OnPropertyChanged("StateK2");
            }
        }
        public String StateK3
        {
            get
            {
                return _stateK3;
            }
            set
            {
                _stateK3 = value;
                OnPropertyChanged("StateK3");
            }
        }
        public String StateK4
        {
            get
            {
                return _stateK4;
            }
            set
            {
                _stateK4 = value;
                OnPropertyChanged("StateK4");
            }
        }
        public String StateK5
        {
            get
            {
                return _stateK5;
            }
            set
            {
                _stateK5 = value;
                OnPropertyChanged("StateK5");
            }
        }
        public String StateK6
        {
            get
            {
                return _stateK6;
            }
            set
            {
                _stateK6 = value;
                OnPropertyChanged("StateK6");
            }
        }
        public String StateK7
        {
            get
            {
                return _stateK7;
            }
            set
            {
                _stateK7 = value;
                OnPropertyChanged("StateK7");
            }
        }
        public String StateInput0
        {
            get
            {
                return _stateInput0;
            }
            set
            {
                _stateInput0 = value;
                OnPropertyChanged("StateInput0");
            }
        }
        public String StateInput1
        {
            get
            {
                return _stateInput1;
            }
            set
            {
                _stateInput1 = value;
                OnPropertyChanged("StateInput1");
            }
        }
        public String StateInput2
        {
            get
            {
                return _stateInput2;
            }
            set
            {
                _stateInput2 = value;
                OnPropertyChanged("StateInput2");
            }
        }
        public String StateInput3
        {
            get
            {
                return _stateInput3;
            }
            set
            {
                _stateInput3 = value;
                OnPropertyChanged("StateInput3");
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
