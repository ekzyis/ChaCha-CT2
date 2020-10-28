using Cryptool.Plugins.ChaChaVisualizationV2.Helper;
using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cryptool.Plugins.ChaChaVisualizationV2.View
{
    /// <summary>
    /// Interaction logic for StateMatrixInitialization.xaml
    /// </summary>
    public partial class StateMatrixInit : UserControl
    {
        public StateMatrixInit()
        {
            InitializeComponent();
            this.DataContext = new StateMatrixInitViewModel();
            InitStateMatrixGrid();
        }

        private class StateCell : Border
        {
            public StateCell(IGrid<uint> entry, int index)
            {
                TextBox tb = new TextBox();
                // TODO is there a way to not hardcode the path to the ObservableCollection in the view model?
                tb.SetBinding(TextBox.TextProperty, new Binding($"StateMatrixValues[{index}].Value"));
                TextBox = tb;

                Viewbox vb = new Viewbox
                {
                    Child = tb
                };
                this.Child = vb;
                Viewbox = vb;

                Grid.SetRow(this, entry.Row);
                Grid.SetColumn(this, entry.Column);
            }

            public TextBox TextBox { get; set; }
            public Viewbox Viewbox { get; set; }
        }

        /// <summary>
        /// Put the state matrix entries defined in the ViewModel into the grid.
        /// Workaround for not being able to do this in XAML with an ItemsControl.
        /// </summary>
        private void InitStateMatrixGrid()
        {
            StateMatrix.Children.Clear();
            for (int i = 0; i < 16; ++i)
            {
                IGrid<uint> entry = ((StateMatrixInitViewModel)DataContext).StateMatrixValues[i];
                StateCell cell = new StateCell(entry, i)
                {
                    Style = this.FindResource("stateBorder") as Style
                };
                cell.Viewbox.Style = this.FindResource("stateViewbox") as Style;
                cell.TextBox.Style = this.FindResource("stateText") as Style;
                StateMatrix.Children.Add(cell);
            }
        }
    }
}