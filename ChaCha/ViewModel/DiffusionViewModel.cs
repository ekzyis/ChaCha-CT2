﻿using Cryptool.Plugins.ChaCha.Helper;
using Cryptool.Plugins.ChaCha.ViewModel.Components;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaCha.ViewModel
{
    internal class DiffusionViewModel : ViewModelBase, INavigation, ITitle, IChaCha
    {
        public DiffusionViewModel(ChaChaPresentationViewModel chachaPresentationViewModel)
        {
            PresentationViewModel = chachaPresentationViewModel;
            ChaCha.PropertyChanged += new PropertyChangedEventHandler(PluginInputChanged);
            Name = "Diffusion";
            Title = "Diffusion";
            InitHandlers();
        }

        private void PluginInputChanged(object sender, PropertyChangedEventArgs e)
        {
            DiffusionKey = ChaCha.InputKey;
            DiffusionKeyExplicit = ChaCha.InputKey;
            DiffusionKeyXOR = ByteUtil.XOR(DiffusionKey, ChaCha.InputKey);
            DiffusionIV = ChaCha.InputIV;
            DiffusionInitialCounter = ChaCha.InitialCounter;
        }

        #region Input Handlers

        public delegate TextChangedEventHandler ValidatedTextChangedEventHandler(ValidationRule validationRule);

        public static ValidatedTextChangedEventHandler ValidateInputBeforeHandle(Action<string> handle)
        {
            return (ValidationRule validationRule) =>
            {
                return (object sender, TextChangedEventArgs e) =>
                {
                    string value = ((TextBox)sender).Text;
                    ValidationResult result = validationRule.Validate(value, null);
                    if (result == ValidationResult.ValidResult)
                    {
                        handle(value);
                    }
                };
            };
        }

        private void InitHandlers()
        {
            DiffusionKeyExplicitInputHandler = ValidateInputBeforeHandle((string value) =>
            {
                DiffusionKey = Formatter.Bytes(value);
                DiffusionKeyXOR = ByteUtil.XOR(DiffusionKey, ChaCha.InputKey);
            });
            DiffusionKeyXORInputHandler = ValidateInputBeforeHandle((string value) =>
            {
                byte[] input = Formatter.Bytes(value);
                DiffusionKey = ByteUtil.XOR(input, ChaCha.InputKey);
                DiffusionKeyExplicit = DiffusionKey;
            });
        }

        public ValidatedTextChangedEventHandler DiffusionKeyExplicitInputHandler;

        public ValidatedTextChangedEventHandler DiffusionKeyXORInputHandler;

        #endregion Input Handlers

        #region Binding Properties

        /// <summary>
        /// The value which is shown in the diffusion key input box.
        /// </summary>
        public byte[] _diffusionKeyExplicit; public byte[] DiffusionKeyExplicit

        {
            get
            {
                return _diffusionKeyExplicit;
            }
            set
            {
                if (_diffusionKeyExplicit != value)
                {
                    _diffusionKeyExplicit = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The value which is shown in the diffusion XOR key input box.
        /// </summary>
        public byte[] _diffusionKeyXOR; public byte[] DiffusionKeyXOR

        {
            get
            {
                return _diffusionKeyXOR;
            }
            set
            {
                if (_diffusionKeyXOR != value)
                {
                    _diffusionKeyXOR = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The actual diffusion key which will be used for cipher execution.
        /// </summary>
        private byte[] _diffusionKey; public byte[] DiffusionKey

        {
            get
            {
                return _diffusionKey;
            }
            set
            {
                if (_diffusionKey != value)
                {
                    _diffusionKey = value;
                    OnPropertyChanged();
                    OnPropertyChanged("DiffusionActive");
                    OnPropertyChanged("FlippedBits");
                    OnPropertyChanged("FlippedBitsPercentage");
                }
            }
        }

        private byte[] _diffusionIV; public byte[] DiffusionIV
        {
            get
            {
                return _diffusionIV;
            }
            set
            {
                if (_diffusionIV != value)
                {
                    _diffusionIV = value;
                    OnPropertyChanged();
                    OnPropertyChanged("DiffusionActive");
                    OnPropertyChanged("FlippedBits");
                    OnPropertyChanged("FlippedBitsPercentage");
                }
            }
        }

        private BigInteger _diffusionInitialCounter; public BigInteger DiffusionInitialCounter
        {
            get
            {
                return _diffusionInitialCounter;
            }
            set
            {
                if (_diffusionInitialCounter != value)
                {
                    _diffusionInitialCounter = value;
                    OnPropertyChanged();
                    OnPropertyChanged("DiffusionActive");
                    OnPropertyChanged("FlippedBits");
                    OnPropertyChanged("FlippedBitsPercentage");
                }
            }
        }

        public int FlippedBits
        {
            get => BitFlips.FlippedBits(DiffusionKey, ChaCha.InputKey) + BitFlips.FlippedBits(DiffusionIV, ChaCha.InputIV) + BitFlips.FlippedBits((ulong)DiffusionInitialCounter, (ulong)ChaCha.InitialCounter);
        }

        public int TotalBits
        {
            get => DiffusionKey.Length * 8 + DiffusionIV.Length * 8 + (int)Settings.Version.CounterBits;
        }

        public double FlippedBitsPercentage
        {
            get => (double)FlippedBits / TotalBits;
        }

        public bool DiffusionActive
        {
            get => !(DiffusionKey.SequenceEqual(ChaCha.InputKey) && DiffusionIV.SequenceEqual(ChaCha.InputIV) && DiffusionInitialCounter == ChaCha.InitialCounter);
        }

        #endregion Binding Properties

        #region INavigation

        private string _name; public string Name
        {
            get
            {
                if (_name == null) _name = "";
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Setup()
        {
        }

        public void Teardown()
        {
            if (DiffusionActive)
            {
                // Execute ChaCha with Diffusion values.
                ChaCha.ExecuteDiffusion(DiffusionKey, DiffusionIV, (ulong)DiffusionInitialCounter);
            }
        }

        #endregion INavigation

        #region ITitle

        private string _title; public string Title
        {
            get
            {
                if (_title == null) _title = "";
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion ITitle

        #region IChaCha

        public ChaChaPresentationViewModel PresentationViewModel { get; private set; }
        public ChaCha ChaCha { get => PresentationViewModel.ChaCha; }
        public ChaChaSettings Settings { get => (ChaChaSettings)ChaCha.Settings; }

        #endregion IChaCha
    }
}