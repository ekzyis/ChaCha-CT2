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
        public ChaChaPresentation()
        {
            InitializeComponent();
        }

        private bool On(UIElement page)
        {
            return page.Visibility == Visibility.Visible;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (On(Landingpage))
            {
                Landingpage.Visibility = Visibility.Collapsed;
                Workflowpage.Visibility = Visibility.Visible;
            }
        }
    }
}
