using Cryptool.Plugins.ChaCha.Visualization.ViewModel;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaCha.Visualization.View
{
    /// <summary>
    /// Interaction logic for Overview.xaml
    /// </summary>
    public partial class Overview : UserControl
    {
        public Overview()
        {
            InitializeComponent();
            this.DataContext = new OverviewViewModel();
        }
    }
}