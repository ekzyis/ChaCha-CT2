using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaChaVisualizationV2.View
{
    /// <summary>
    /// Interaction logic for Overview.xaml
    /// </summary>
    public partial class Overview : UserControl
    {
        public Overview()
        {
            InitializeComponent();
            this.DataContext = new TitlePageViewModel();
        }
    }
}