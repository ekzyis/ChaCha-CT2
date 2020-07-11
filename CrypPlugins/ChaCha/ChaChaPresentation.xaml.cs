using System;
using System.Collections.Generic;
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
    public partial class ChaChaPresentation : UserControl
    {
        // List which indicates page order to implement page navigation
        private UIElement[] pageRouting;
        private int currentPageIndex = 0;
        public ChaChaPresentation()
        {
            InitializeComponent();
            pageRouting = new UIElement[] { Landingpage, Workflowpage, Statematrixpage };
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            pageRouting[currentPageIndex].Visibility = Visibility.Collapsed;
            currentPageIndex++;
            pageRouting[currentPageIndex].Visibility = Visibility.Visible;
        }
    }
}
