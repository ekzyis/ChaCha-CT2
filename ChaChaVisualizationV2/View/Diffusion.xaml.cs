using Cryptool.Plugins.ChaCha;
using Cryptool.Plugins.ChaChaVisualizationV2.Helper;
using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Cryptool.Plugins.ChaChaVisualizationV2.View
{
    /// <summary>
    /// Interaction logic for Diffusion.xaml
    /// </summary>
    public partial class Diffusion : UserControl
    {
        public Diffusion()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DiffusionViewModel ViewModel = (DiffusionViewModel)e.NewValue;
            if (ViewModel != null)
            {
                DiffusionInputKey.Text = Formatter.HexString(ViewModel.ChaChaVisualization.InputKey);
                EditingCommands.ToggleInsert.Execute(null, DiffusionInputKey);
                DiffusionInputIV.Text = Formatter.HexString(ViewModel.ChaChaVisualization.InputIV);
                EditingCommands.ToggleInsert.Execute(null, DiffusionInputIV);
                BigInteger initialCounter = ViewModel.ChaChaVisualization.InitialCounter;
                DiffusionInitialCounter.Text = Formatter.HexString(ViewModel.Settings.Version == Version.DJB ? (ulong)initialCounter : (uint)initialCounter);
                EditingCommands.ToggleInsert.Execute(null, DiffusionInitialCounter);
            }
        }
    }
}