using Cryptool.Plugins.ChaCha;
using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaChaVisualizationV2.View
{
    /// <summary>
    /// Interaction logic for ChaChaPresentation.xaml
    /// </summary>
    public partial class ChaChaPresentation : UserControl
    {
        public ChaChaPresentation(ChaCha.ChaCha chacha, ChaChaSettings settings)
        {
            InitializeComponent();
            this.DataContext = new ChaChaPresentationViewModel(chacha, settings);
        }
    }
}