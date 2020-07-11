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

        private String stateParamsConstant;
        private String stateParamsKey;
        private String stateParamsIV;
        private String stateParamsInput;

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

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            pageRouting[currentPageIndex].Visibility = Visibility.Collapsed;
            currentPageIndex++;
            pageRouting[currentPageIndex].Visibility = Visibility.Visible;
        }
    }
}
