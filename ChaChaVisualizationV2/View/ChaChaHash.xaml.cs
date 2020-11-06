using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaChaVisualizationV2.View
{
    /// <summary>
    /// Interaction logic for ChaChaHash.xaml
    /// </summary>
    public partial class ChaChaHash : UserControl
    {
        public ChaChaHash()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
        }

        private ChaChaHashViewModel ViewModel { get; set; }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ChaChaHashViewModel ViewModel = (ChaChaHashViewModel)e.NewValue;
            if (ViewModel != null)
            {
                this.ViewModel = ViewModel;

                // Add value changed event handler to action slider
                Root.ApplyTemplate();
                Slider actionSlider = (Slider)Root.Template.FindName("ActionSlider", Root);
                actionSlider.ValueChanged += ViewModel.ActionSliderValueChange;
            }
        }
    }
}