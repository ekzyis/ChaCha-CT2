﻿using Cryptool.Plugins.ChaCha.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaCha.View
{
    /// <summary>
    /// Interaction logic for Xor.xaml
    /// </summary>
    [PluginBase.Attributes.Localization("Cryptool.Plugins.ChaCha.Properties.Resources")]
    public partial class Xor : UserControl
    {
        public Xor()
        {
            InitializeComponent();
            ActionViewBase.LoadLocaleResources(this);
            this.DataContextChanged += OnDataContextChanged;
        }

        private XorViewModel ViewModel { get; set; }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            XorViewModel ViewModel = (XorViewModel)e.NewValue;
            if (ViewModel != null)
            {
                this.ViewModel = ViewModel;
                if (ViewModel.DiffusionActive) InitDiffusionValues();
            }
        }

        /// <summary>
        /// Init the diffusion values for the keystream and the output text.
        /// </summary>
        private void InitDiffusionValues()
        {
            Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionKeystream, ViewModel.ChaCha.KeystreamDiffusion.ToArray(), ViewModel.ChaCha.Keystream.ToArray());
            Plugins.ChaCha.ViewModel.Components.Diffusion.InitXORValue(DiffusionKeystreamXOR, ViewModel.ChaCha.KeystreamDiffusion.ToArray(), ViewModel.ChaCha.Keystream.ToArray());
            Plugins.ChaCha.ViewModel.Components.Diffusion.InitDiffusionValue(DiffusionOutput, ViewModel.ChaCha.OutputDiffusion.ToArray(), ViewModel.ChaCha.Output.ToArray());
            Plugins.ChaCha.ViewModel.Components.Diffusion.InitXORValue(DiffusionOutputXOR, ViewModel.ChaCha.OutputDiffusion.ToArray(), ViewModel.ChaCha.Output.ToArray());
        }
    }
}