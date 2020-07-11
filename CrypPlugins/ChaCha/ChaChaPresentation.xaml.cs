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
            pageRouting = new UIElement[] { Landingpage, Workflowpage, Statematrixpage };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #region StateParams
        private String stateParamsConstant;
        private String stateParamsKey;
        private String stateParamsIV;
        private String stateParamsInput;

        public String StateParamsConstant
        {
            get
            {
                return stateParamsConstant;
            }
            set
            {
                stateParamsConstant = value;
                OnPropertyChanged("StateParamsConstant");
            }
        }

        public String StateParamsKey
        {
            get
            {
                return stateParamsKey;
            }
            set
            {
                stateParamsKey = value;
                OnPropertyChanged("StateParamsKey");
            }
        }

        public String StateParamsIV
        {
            get
            {
                return stateParamsIV;
            }
            set
            {
                stateParamsIV = value;
                OnPropertyChanged("StateParamsIV");
            }
        }

        public String StateParamsInput
        {
            get
            {
                return stateParamsInput;
            }
            set
            {
                stateParamsInput = value;
                OnPropertyChanged("StateParamsInput");
            }
        }
        #endregion

        #region State
        private String stateC0;
        private String stateC1;
        private String stateC2;
        private String stateC3;
        private String stateK0;
        private String stateK1;
        private String stateK2;
        private String stateK3;
        private String stateK4;
        private String stateK5;
        private String stateK6;
        private String stateK7;
        private String stateInput0;
        private String stateInput1;
        private String stateInput2;
        private String stateInput3;
        public String StateC0
        {
            get
            {
                return stateC0;
            }
            set
            {
                stateC0 = value;
                OnPropertyChanged("StateC0");
            }
        }
        public String StateC1
        {
            get
            {
                return stateC1;
            }
            set
            {
                stateC1 = value;
                OnPropertyChanged("StateC1");
            }
        }
        public String StateC2
        {
            get
            {
                return stateC2;
            }
            set
            {
                stateC2 = value;
                OnPropertyChanged("StateC2");
            }
        }
        public String StateC3
        {
            get
            {
                return stateC3;
            }
            set
            {
                stateC3 = value;
                OnPropertyChanged("StateC3");
            }
        }
        public String StateK0
        {
            get
            {
                return stateK0;
            }
            set
            {
                stateK0 = value;
                OnPropertyChanged("StateK0");
            }
        }
        public String StateK1
        {
            get
            {
                return stateK1;
            }
            set
            {
                stateK1 = value;
                OnPropertyChanged("StateK1");
            }
        }
        public String StateK2
        {
            get
            {
                return stateK2;
            }
            set
            {
                stateK2 = value;
                OnPropertyChanged("StateK2");
            }
        }
        public String StateK3
        {
            get
            {
                return stateK3;
            }
            set
            {
                stateK3 = value;
                OnPropertyChanged("StateK3");
            }
        }
        public String StateK4
        {
            get
            {
                return stateK4;
            }
            set
            {
                stateK4 = value;
                OnPropertyChanged("StateK4");
            }
        }
        public String StateK5
        {
            get
            {
                return stateK5;
            }
            set
            {
                stateK5 = value;
                OnPropertyChanged("StateK5");
            }
        }
        public String StateK6
        {
            get
            {
                return stateK6;
            }
            set
            {
                stateK6 = value;
                OnPropertyChanged("StateK6");
            }
        }
        public String StateK7
        {
            get
            {
                return stateK7;
            }
            set
            {
                stateK7 = value;
                OnPropertyChanged("StateK7");
            }
        }
        public String StateInput0
        {
            get
            {
                return stateInput0;
            }
            set
            {
                stateInput0 = value;
                OnPropertyChanged("StateInput0");
            }
        }
        public String StateInput1
        {
            get
            {
                return stateInput1;
            }
            set
            {
                stateInput1 = value;
                OnPropertyChanged("StateInput1");
            }
        }
        public String StateInput2
        {
            get
            {
                return stateInput2;
            }
            set
            {
                stateInput2 = value;
                OnPropertyChanged("StateInput2");
            }
        }
        public String StateInput3
        {
            get
            {
                return stateInput3;
            }
            set
            {
                stateInput3 = value;
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

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            pageRouting[currentPageIndex].Visibility = Visibility.Collapsed;
            currentPageIndex++;
            pageRouting[currentPageIndex].Visibility = Visibility.Visible;
        }
    }
}
