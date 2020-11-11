using Cryptool.Plugins.ChaCha.Visualization.ViewModel;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaCha.Visualization.View
{
    /// <summary>
    /// Interaction logic for ChaChaPresentation.xaml
    /// </summary>
    public partial class ChaChaPresentation : UserControl
    {
        public ChaChaPresentation(ChaChaVisualization chachaVisualization)
        {
            InitializeComponent();
            this.DataContext = new ChaChaPresentationViewModel(chachaVisualization);
        }
    }
}