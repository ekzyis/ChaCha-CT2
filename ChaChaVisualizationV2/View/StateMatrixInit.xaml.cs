using Cryptool.Plugins.ChaChaVisualizationV2.Model;
using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaChaVisualizationV2.View
{
    /// <summary>
    /// Interaction logic for StateMatrixInitialization.xaml
    /// </summary>
    public partial class StateMatrixInit : UserControl
    {
        public StateMatrixInit()
        {
            ViewModel = new StateMatrixInitViewModel();
            this.DataContext = ViewModel;
            InitializeComponent();
        }

        private StateMatrixInitViewModel ViewModel { get; set; }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var rnd = new Random();
            uint value = (uint)rnd.Next();
            ViewModel.StateMatrixValues.Add(new StateMatrixValue(value, 0, 0));
        }
    }
}