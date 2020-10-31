﻿using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaChaVisualizationV2.View
{
    /// <summary>
    /// Interaction logic for ChaChaPresentation.xaml
    /// </summary>
    public partial class ChaChaPresentation : UserControl
    {
        public ChaChaPresentation(ChaChaVisualizationV2 chachaVisualization)
        {
            InitializeComponent();
            this.DataContext = new ChaChaPresentationViewModel(chachaVisualization);
        }
    }
}