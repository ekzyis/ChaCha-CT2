using Cryptool.Plugins.ChaCha.ViewModel;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaCha.View
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