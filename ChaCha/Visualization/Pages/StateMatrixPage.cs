﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Cryptool.Plugins.Chacha.Extensions;

namespace Cryptool.Plugins.ChaCha
{

    public class CounterInputValidationRule: ValidationRule
    {
        private ulong _maxCounter;
        public CounterInputValidationRule(ulong maxCounter)
        {
            _maxCounter = maxCounter;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string inputText = (string)value;
            ulong inputCounter = 0;

            try
            {
                inputCounter = ulong.Parse(inputText, NumberStyles.HexNumber);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"{e.Message}");
            }

            if (inputCounter > _maxCounter)
            {
                return new ValidationResult(false,
                    $"Counter must be in range 0-{_maxCounter}.");
            }
            return ValidationResult.ValidResult;
        }
    }

    public class HexStringToULongConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ChaChaPresentation.HexString((ulong)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ulong.Parse((string)value, NumberStyles.HexNumber);
        }
    }

    class StateMatrixPage : Page
    {
        private ChaChaPresentation pres;
        private RichTextBox diffusionKeyText;
        private Button toggleShowDiffusion;
        private Button resetDiffusion;
        private Grid diffusionGrid;

        private List<string> descriptions = new List<string>();
        private bool versionIsDJB;
        private bool keyIs32Byte;
        public StateMatrixPage(ContentControl pageElement, ChaChaPresentation pres_) : base(pageElement)
        {
            pres = pres_;
            versionIsDJB = pres.Version == ChaCha.Version.DJB;
            keyIs32Byte = pres.InputKey.Length == 32;
            descriptions.Add("The 512-bit (128-byte) ChaCha state can be interpreted as a 4x4 matrix, where each entry consists of 4 bytes. The state entries consist of the parameters you can see below. They will be encoded before insertion into the state matrix. The first 16 bytes consist of the constants. ");
            descriptions.Add("The next 32 bytes consist of the key. If the key consists of only 16 bytes, it is concatenated with itself. ");
            descriptions.Add($"The next {pres.InitialCounter.Length} bytes consist of the counter. The counter is special since we first reverse all bytes. This is so because all other parameters are assumed to be already in little-endian thus no reversing needed.");
            descriptions.Add($"The last {pres.InputIV.Length} bytes consist of the initialization vector. ");
            descriptions.Add("On the next page, we will use this initialized state matrix to generate the first keystream block.");

            Init();
        }

        private void Init()
        {
            AddConstantsActions();
            AddKeyActions();
            AddCounterActions();
            AddIVActions();
            PageAction nextPageDesc = new PageAction(() =>
            {
                UnboldLastFromDescription();
                AddBoldToDescription(descriptions[4]);
                ClearTransformInput();
                ClearTransformChunk();
                ClearTransformLittleEndian();
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
                ReplaceTransformInput(pres.HexInputIV);
                ReplaceTransformChunkIV();
                ReplaceTransformLittleEndianIV();
            });
            AddAction(nextPageDesc);
            InitDiffusion();
        }

        public override void Setup()
        {
            base.Setup();
            InitCounterInput();
            AddConstantsStepBoldToDescription();
        }

        public override void TearDown()
        {
            base.TearDown();
            ClearDescription();
        }

        #region Constants
        private void AddConstantsActions()
        {
            PageAction constantsInputAction = new PageAction(() =>
            {
                ReplaceTransformInputConstants();
            }, () =>
            {
                ClearTransformInput();
            });
            PageAction constantsChunksAction = new PageAction(() =>
            {
                ReplaceTransformChunkConstants();
            }, () =>
            {
                ClearTransformChunk();
            });
            PageAction constantsLittleEndianAction = new PageAction(() =>
            {
                ReplaceTransformLittleEndianConstants();
            }, () =>
            {
                ClearTransformLittleEndian();
            });
            
            PageAction copyConstantsToStateAction = new PageAction(() =>
            {
                string le0 = pres.UITransformLittleEndian0.Text;
                string le1 = pres.UITransformLittleEndian1.Text;
                string le2 = pres.UITransformLittleEndian2.Text;
                string le3 = pres.UITransformLittleEndian3.Text;
                pres.Nav.Replace(pres.UIState0, le0);
                pres.Nav.Replace(pres.UIState1, le1);
                pres.Nav.Replace(pres.UIState2, le2);
                pres.Nav.Replace(pres.UIState3, le3);
            }, () =>
            {
                pres.Nav.Clear(pres.UIState0);
                pres.Nav.Clear(pres.UIState1);
                pres.Nav.Clear(pres.UIState2);
                pres.Nav.Clear(pres.UIState3);
            });
            AddAction(constantsInputAction);
            AddAction(constantsChunksAction);
            AddAction(constantsLittleEndianAction);
            AddAction(copyConstantsToStateAction);
        }
        private void AddConstantsStepBoldToDescription()
        {
            AddBoldToDescription(descriptions[0]);
        }
        private void ReplaceTransformInputConstants()
        {
            ReplaceTransformInput(pres.HexConstants);
        }
        private void ReplaceTransformChunkConstants()
        {
            ReplaceTransformChunk(pres.ConstantsChunks[0], pres.ConstantsChunks[1], pres.ConstantsChunks[2], pres.ConstantsChunks[3]);
        }
        private void ReplaceTransformLittleEndianConstants()
        {
            ReplaceTransformLittleEndian(pres.ConstantsLittleEndian[0], pres.ConstantsLittleEndian[1], pres.ConstantsLittleEndian[2], pres.ConstantsLittleEndian[3]);
        }
        #endregion

        #region Key
        private void AddKeyActions()
        {
            PageAction keyStepDescriptionAction = new PageAction(() =>
            {
                UnboldLastFromDescription();
                AddKeyStepBoldToDescription();
                ClearTransformInput();
                ClearTransformChunk();
                ClearTransformLittleEndian();
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
                ReplaceTransformInputConstants();
                ReplaceTransformChunkConstants();
                ReplaceTransformLittleEndianConstants();
            });
            PageAction keyInputAction = new PageAction(() =>
            {
                ReplaceTransformInputKey();
            }, () =>
            {
                ClearTransformInputKey();
            });
            PageAction keyChunksAction = new PageAction(() =>
            {
                ReplaceTransformChunkKey();
            }, () =>
            {
                ClearTransformChunkKey();
            });
            PageAction keyLittleEndianAction = new PageAction(() =>
            {
                ReplaceTransformLittleEndianKey();
            }, () =>
            {
                ClearTransformLittleEndianKey();
            });
            PageAction copyKeyToStateActions = CopyKeyToStateAction();
            AddAction(keyStepDescriptionAction);
            AddAction(keyInputAction);
            AddAction(keyChunksAction);
            AddAction(keyLittleEndianAction);
            AddAction(copyKeyToStateActions);
        }
        private void AddKeyStepBoldToDescription()
        {
            AddBoldToDescription(descriptions[1]);
        }

        private void ClearTransformInputKey()
        {
            if (pres.InputKey.Length == 16)
            {
                pres.Nav.Clear(pres.UITransformInputKey);
                if (pres.DiffusionActive)
                {
                    pres.Nav.Clear(pres.UITransformInputKeyDiffusion);
                }
            }
            else if (pres.InputKey.Length == 32)
            {
                pres.Nav.Clear(pres.UITransformInputKey, pres.UITransformInputKey2);
                if (pres.DiffusionActive)
                {
                    pres.Nav.Clear(pres.UITransformInputKeyDiffusion, pres.UITransformInputKeyDiffusion2);
                }
            }
        }

        private void ReplaceTransformInputKey()
        {
            string keyText = pres.UIInputKey.Text;
            if (pres.InputKey.Length == 16)
            {
                Debug.Assert(keyText.Length == 32);
                pres.Nav.Replace(pres.UITransformInputKey, keyText);
            }
            else if (pres.InputKey.Length == 32)
            {
                Debug.Assert(keyText.Length == 64);
                pres.Nav.Replace(pres.UITransformInputKey, keyText.Substring(0, 32));
                pres.Nav.Replace(pres.UITransformInputKey2, keyText.Substring(32));
            }
            else
            {
                throw new InvalidOperationException("KeyInputAction: key was neither 16 byte nor 32 byte. This should not happen!");
            }
            if (pres.DiffusionActive)
            {
                ReplaceTransformInputKeyDiffusion();
            }
        }

        private void ReplaceTransformChunkKey()
        {
            pres.Nav.Replace(pres.UITransformChunkKey0, pres.KeyChunks[0]);
            pres.Nav.Replace(pres.UITransformChunkKey1, pres.KeyChunks[1]);
            pres.Nav.Replace(pres.UITransformChunkKey2, pres.KeyChunks[2]);
            pres.Nav.Replace(pres.UITransformChunkKey3, pres.KeyChunks[3]);
            pres.Nav.Replace(pres.UITransformChunkKey4, pres.KeyChunks[keyIs32Byte ? 4 : 0]);
            pres.Nav.Replace(pres.UITransformChunkKey5, pres.KeyChunks[keyIs32Byte ? 5 : 1]);
            pres.Nav.Replace(pres.UITransformChunkKey6, pres.KeyChunks[keyIs32Byte ? 6 : 2]);
            pres.Nav.Replace(pres.UITransformChunkKey7, pres.KeyChunks[keyIs32Byte ? 7: 3]);

            if(pres.DiffusionActive)
            {
                ReplaceTransformChunkDiffusion();
            }
        }

        private void ClearTransformChunkKey()
        {
            pres.Nav.Clear(pres.UITransformChunkKey0, pres.UITransformChunkKey1, pres.UITransformChunkKey2, pres.UITransformChunkKey3, pres.UITransformChunkKey4, pres.UITransformChunkKey5, pres.UITransformChunkKey6, pres.UITransformChunkKey7);

            if (pres.DiffusionActive)
            {
                ClearTransformChunkKeyDiffusion();
            }
        }

        private void ReplaceTransformLittleEndianKey()
        {
            pres.Nav.Replace(pres.UITransformLittleEndianKey0, pres.KeyLittleEndian[0]);
            pres.Nav.Replace(pres.UITransformLittleEndianKey1, pres.KeyLittleEndian[1]);
            pres.Nav.Replace(pres.UITransformLittleEndianKey2, pres.KeyLittleEndian[2]);
            pres.Nav.Replace(pres.UITransformLittleEndianKey3, pres.KeyLittleEndian[3]);
            pres.Nav.Replace(pres.UITransformLittleEndianKey4, pres.KeyLittleEndian[keyIs32Byte ? 4 : 0]);
            pres.Nav.Replace(pres.UITransformLittleEndianKey5, pres.KeyLittleEndian[keyIs32Byte ? 5 : 1]);
            pres.Nav.Replace(pres.UITransformLittleEndianKey6, pres.KeyLittleEndian[keyIs32Byte ? 6 : 2]);
            pres.Nav.Replace(pres.UITransformLittleEndianKey7, pres.KeyLittleEndian[keyIs32Byte ? 7 : 3]);

            if (pres.DiffusionActive)
            {
                ReplaceTransformLittleEndianDiffusion();
            }
        }

        private void ClearTransformLittleEndianKey()
        {
            pres.Nav.Clear(pres.UITransformLittleEndianKey0, pres.UITransformLittleEndianKey1, pres.UITransformLittleEndianKey2, pres.UITransformLittleEndianKey3, pres.UITransformLittleEndianKey4, pres.UITransformLittleEndianKey5, pres.UITransformLittleEndianKey6, pres.UITransformLittleEndianKey7);

            if (pres.DiffusionActive)
            {
                ClearTransformLittleEndianDiffusion();
            }
        }
        private PageAction CopyKeyToStateAction()
        {
            return new PageAction(() =>
            {
                string key0 = pres.UITransformLittleEndianKey0.Text;
                string key1 = pres.UITransformLittleEndianKey1.Text;
                string key2 = pres.UITransformLittleEndianKey2.Text;
                string key3 = pres.UITransformLittleEndianKey3.Text;
                string key4 = pres.UITransformLittleEndianKey4.Text;
                string key5 = pres.UITransformLittleEndianKey5.Text;
                string key6 = pres.UITransformLittleEndianKey6.Text;
                string key7 = pres.UITransformLittleEndianKey7.Text;
                pres.Nav.Replace(pres.UIState4, key0);
                pres.Nav.Replace(pres.UIState5, key1);
                pres.Nav.Replace(pres.UIState6, key2);
                pres.Nav.Replace(pres.UIState7, key3);
                pres.Nav.Replace(pres.UIState8, key4);
                pres.Nav.Replace(pres.UIState9, key5);
                pres.Nav.Replace(pres.UIState10, key6);
                pres.Nav.Replace(pres.UIState11, key7);
                if(pres.DiffusionActive)
                {
                    InsertDiffusionKeyIntoState();
                }
            }, () =>
            {
                pres.Nav.Clear(pres.UIState4, pres.UIState5, pres.UIState6, pres.UIState7, pres.UIState8, pres.UIState9, pres.UIState10, pres.UIState11);
                if(pres.DiffusionActive)
                {
                    ClearDiffusionKeyFromState();
                }
            });
        }
        #endregion

        #region Counter
        private void AddCounterActions()
        {
            PageAction counterStepDescriptionAction = new PageAction(() =>
            {
                UnboldLastFromDescription();
                AddCounterStepBoldToDescription();
                ClearTransformInputKey();
                ClearTransformChunkKey();
                ClearTransformLittleEndianKey();
                AddReverseBytesStep();
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
                ReplaceTransformInputKey();
                ReplaceTransformChunkKey();
                ReplaceTransformLittleEndianKey();
                RemoveReverseBytesStep();
            });
            PageAction counterInputAction = new PageAction(() =>
            {
                ReplaceTransformInputCounter();
            }, () =>
            {
                ClearTransformInput();
            });
            PageAction reverseBytesAction = new PageAction(() =>
            {
                AddReverseCounter();
            }, () =>
            {
                ClearReverseCounter();
            });
            PageAction counterChunksAction = new PageAction(() =>
            {
                ReplaceTransformChunkCounter();
            }, () =>
            {
                ClearTransformChunk();
            });
            PageAction counterLittleEndianAction = new PageAction(() =>
            {
                ReplaceTransformLittleEndianCounter();
            }, () =>
            {
                ClearTransformLittleEndian();
            });
            PageAction copyCounterToState = new PageAction(() =>
            {
                if(versionIsDJB)
                {
                    pres.Nav.SetTextBinding(pres.UIState12, "InputCounterLittleEndian[0]");
                    pres.Nav.SetTextBinding(pres.UIState13, "InputCounterLittleEndian[1]");
                }
                else
                {
                    pres.Nav.SetTextBinding(pres.UIState12, "InputCounterLittleEndian[0]");
                }
            }, () =>
            {
                pres.Nav.ClearTextBinding(pres.UIState12);
                if (versionIsDJB)
                {
                    pres.Nav.ClearTextBinding(pres.UIState13);
                }
            });
            AddAction(counterStepDescriptionAction);
            AddAction(counterInputAction);
            AddAction(reverseBytesAction);
            AddAction(counterChunksAction);
            AddAction(counterLittleEndianAction);
            AddAction(copyCounterToState);
        }
        private void AddCounterStepBoldToDescription()
        {
            AddBoldToDescription(descriptions[2]);
        }
        private void ReplaceTransformInputCounter()
        {
            SetTextBindingToTransformInput("HexInputCounter");
        }
        private void ReplaceTransformChunkCounter()
        {
            if (versionIsDJB)
            {
                SetTextBindingToTransformChunk("InputCounterChunks[0]", "InputCounterChunks[1]");
            }
            else
            {
                SetTextBindingToTransformChunk("InputCounterChunks[0]");
            }
        }
        private void ReplaceTransformLittleEndianCounter()
        {
            if (versionIsDJB)
            {
                SetTextBindingToTransformLittleEndian("InputCounterLittleEndian[0]", "InputCounterLittleEndian[1]");
            }
            else
            {
                SetTextBindingToTransformLittleEndian("InputCounterLittleEndian[0]");
            }
        }

        private void InitCounterInput()
        {
            ValidationRule counterInputValidationRule = new CounterInputValidationRule(versionIsDJB ? ulong.MaxValue : uint.MaxValue);
            Binding counterInputBinding = new Binding("InputCounter")
            { Mode = BindingMode.OneWayToSource, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Converter = new HexStringToULongConverter() };
            counterInputBinding.ValidationRules.Add(counterInputValidationRule);
            pres.UICounter.SetBinding(TextBox.TextProperty, counterInputBinding);
            pres.UICounter.Text = ChaChaPresentation.HexString(pres.InitialCounter);
        }

        private void AddReverseCounter()
        {
            TextBox reverseValue = (TextBox)LogicalTreeHelper.FindLogicalNode(pres.TransformGrid, "ReverseValue"); ;
            pres.Nav.SetTextBinding(reverseValue, "HexInputCounterReverse");
        }

        private void AddReverseBytesStep()
        {
            pres.TransformGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(pres.TransformChunkDesc, 2);
            Grid.SetRow(pres.TransformChunkValue, 2);
            Grid.SetRow(pres.TransformLittleEndianDesc, 3);
            Grid.SetRow(pres.TransformLittleEndianValue, 3);
            TextBox reverseDesc = new TextBox() { Style = pres.TransformGrid.FindResource("TransformDesc") as Style, Name = "ReverseDesc", Text = "Reverse byte order:" };
            TextBox reverseValue = new TextBox() { Style = pres.TransformGrid.FindResource("TransformValue") as Style, Name = "ReverseValue" };
            Grid.SetRow(reverseDesc, 1);
            Grid.SetRow(reverseValue, 1);
            Grid.SetColumn(reverseValue, 1);
            pres.TransformGrid.Children.Add(reverseDesc);
            pres.TransformGrid.Children.Add(reverseValue);
        }

        private void ClearReverseCounter()
        {
            TextBox reverseValue = (TextBox)LogicalTreeHelper.FindLogicalNode(pres.TransformGrid, "ReverseValue");
            pres.Nav.ClearTextBinding(reverseValue);
        }

        private void RemoveReverseBytesStep()
        {
            pres.TransformGrid.Children.RemoveAt(pres.TransformGrid.Children.Count - 1);
            pres.TransformGrid.Children.RemoveAt(pres.TransformGrid.Children.Count - 1);
            pres.TransformGrid.RowDefinitions.RemoveAt(3);
            Grid.SetRow(pres.TransformChunkDesc, 1);
            Grid.SetRow(pres.TransformChunkValue, 1);
            Grid.SetRow(pres.TransformLittleEndianDesc, 2);
            Grid.SetRow(pres.TransformLittleEndianValue, 2);
        }

        #endregion

        #region IV
        private void AddIVActions()
        {
            PageAction ivStepDescriptionAction = new PageAction(() =>
            {
                UnboldLastFromDescription();
                AddIVStepBoldToDescription();
                ClearTransformInput();
                ClearTransformChunk();
                ClearTransformLittleEndian();
                ClearReverseCounter();
                RemoveReverseBytesStep();
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
                ReplaceTransformInputCounter();
                ReplaceTransformChunkCounter();
                ReplaceTransformLittleEndianCounter();
                AddReverseBytesStep();
                AddReverseCounter();
            });
            PageAction ivInputAction = new PageAction(() =>
            {
                ReplaceTransformInputIV();
            }, () =>
            {
                ClearTransformInput();
            });
            PageAction ivChunksAction = new PageAction(() =>
            {
                ReplaceTransformChunkIV();
            }, () =>
            {
                ClearTransformChunk();
            });
            PageAction ivLittleEndianAction = new PageAction(() =>
            {
                ReplaceTransformLittleEndianIV();
            }, () =>
            {
                ClearTransformLittleEndian();
            });
            PageAction copyIVToStateAction = new PageAction(() =>
            {
                if(versionIsDJB)
                {
                    string iv0 = pres.UITransformLittleEndian1.Text;
                    string iv1 = pres.UITransformLittleEndian2.Text;
                    pres.Nav.Replace(pres.UIState14, iv0);
                    pres.Nav.Replace(pres.UIState15, iv1);
                }
                else
                {
                    string iv0 = pres.UITransformLittleEndian0.Text;
                    string iv1 = pres.UITransformLittleEndian1.Text;
                    string iv2 = pres.UITransformLittleEndian2.Text;
                    pres.Nav.Replace(pres.UIState13, iv0);
                    pres.Nav.Replace(pres.UIState14, iv1);
                    pres.Nav.Replace(pres.UIState15, iv2);
                }
            }, () =>
            {
                pres.Nav.Clear(pres.UIState14, pres.UIState15);
                if(!versionIsDJB)
                {
                    pres.Nav.Clear(pres.UIState13);
                }
            });
            AddAction(ivStepDescriptionAction);
            AddAction(ivInputAction);
            AddAction(ivChunksAction);
            AddAction(ivLittleEndianAction);
            AddAction(copyIVToStateAction);
        }
        private void AddIVStepBoldToDescription()
        {
            AddBoldToDescription(descriptions[3]);
        }
        private void ReplaceTransformInputIV()
        {
            ReplaceTransformInput(pres.HexInputIV);
        }
        private void ReplaceTransformChunkIV()
        {
            if (versionIsDJB)
            {
                ReplaceTransformChunk(pres.IVChunks[0], pres.IVChunks[1]);
            }
            else
            {
                ReplaceTransformChunk(pres.IVChunks[0], pres.IVChunks[1], pres.IVChunks[2]);
            }
        }
        private void ReplaceTransformLittleEndianIV()
        {
            if (versionIsDJB)
            {
                ReplaceTransformLittleEndian(pres.IVLittleEndian[0], pres.IVLittleEndian[1]);
            }
            else
            {
                ReplaceTransformLittleEndian(pres.IVLittleEndian[0], pres.IVLittleEndian[1], pres.IVLittleEndian[2]);
            }
        }
        #endregion

        #region Diffusion

        private void InsertDiffusionKeyIntoState()
        {
            FlowDocument fullDKey = GetDiffusionKeyLittleEndian();
            FlowDocument[] chunkDocs = SplitDocument(fullDKey, 8);
            for(int i = 4; i < 12; ++i)
            {
                RichTextBox diffusionState = (RichTextBox)pres.FindName($"UIStateDiffusion{i}");
                pres.Nav.SetDocumentAndShow(diffusionState, chunkDocs[i - 4]);
            }
        }

        private void ClearDiffusionKeyFromState()
        {
            for (int i = 4; i < 12; ++i)
            {
                RichTextBox diffusionState = (RichTextBox)pres.FindName($"UIStateDiffusion{i}");
                pres.Nav.ClearAndCollapse(diffusionState);
            }
        }

        private void ReplaceTransformInputKeyDiffusion()
        {
            FlowDocument fullDKey = GetDiffusionKey();
            if (pres.InputKey.Length == 16)
            {
                pres.Nav.SetDocumentAndShow(pres.UITransformInputKeyDiffusion, fullDKey);
            }
            else
            {
                FlowDocument dKeyRow1;
                FlowDocument dKeyRow2;
                (dKeyRow1, dKeyRow2, _) = SplitDocument(fullDKey, 2);
                pres.Nav.SetDocumentAndShow(pres.UITransformInputKeyDiffusion, dKeyRow1);
                pres.Nav.SetDocumentAndShow(pres.UITransformInputKeyDiffusion2, dKeyRow2);
            }
        }

        private void ClearTransformChunkKeyDiffusion()
        {
            pres.Nav.Clear(pres.UITransformChunkKeyDiffusion0, pres.UITransformChunkKeyDiffusion1, pres.UITransformChunkKeyDiffusion2, pres.UITransformChunkKeyDiffusion3, pres.UITransformChunkKeyDiffusion4, pres.UITransformChunkKeyDiffusion5, pres.UITransformChunkKeyDiffusion6, pres.UITransformChunkKeyDiffusion7);
        }

        private void ReplaceTransformChunkDiffusion()
        {
            FlowDocument fullDKey = GetDiffusionKey();
            FlowDocument[] chunkDocs = SplitDocument(fullDKey, 8);
            for (int i = 0; i < 8; ++i)
            {
                RichTextBox diffusionChunk = (RichTextBox)pres.FindName($"UITransformChunkKeyDiffusion{i}");
                pres.Nav.SetDocumentAndShow(diffusionChunk, chunkDocs[i]);
            }
        }
        private void ReplaceTransformLittleEndianDiffusion()
        {
            FlowDocument fullDKeyLittleEndian = GetDiffusionKeyLittleEndian();
            FlowDocument[] chunkDocs = SplitDocument(fullDKeyLittleEndian, 8);
            for (int i = 0; i < 8; ++i)
            {
                RichTextBox diffusionChunk = (RichTextBox)pres.FindName($"UITransformLittleEndianKeyDiffusion{i}");
                pres.Nav.SetDocumentAndShow(diffusionChunk, chunkDocs[i]);
            }
        }

        private void ClearTransformLittleEndianDiffusion()
        {
            pres.Nav.Clear(pres.UITransformLittleEndianKeyDiffusion0, pres.UITransformLittleEndianKeyDiffusion1, pres.UITransformLittleEndianKeyDiffusion2, pres.UITransformLittleEndianKeyDiffusion3, pres.UITransformLittleEndianKeyDiffusion4, pres.UITransformLittleEndianKeyDiffusion5, pres.UITransformLittleEndianKeyDiffusion6, pres.UITransformLittleEndianKeyDiffusion7);
        }

        private FlowDocument[] SplitDocument(FlowDocument fullFd, int n)
        {
            FlowDocument[] split = new FlowDocument[n];
            Debug.Assert(fullFd.Blocks.Count == 1, $"SplitDocument: FlowDocument block count mismatch. Expected: 1, Actual: {fullFd.Blocks.Count}");
            Debug.Assert(n % 2 == 0, $"SplitDocument: odd n are not supported. Got {n}");
            Paragraph fullP = (Paragraph)fullFd.Blocks.FirstBlock;
            int totalInlinesCount = fullP.Inlines.Count;
            for(int i = 0; i < n; ++i)
            {
                Paragraph splitP = new Paragraph();
                splitP.Inlines.AddRange(fullP.Inlines.ToArray().Take(totalInlinesCount / n));
                FlowDocument splitFd = new FlowDocument() { TextAlignment = TextAlignment.Center };
                splitFd.Blocks.Add(splitP);
                split[i] = splitFd;
            }
            return split;
        }
        private void InitDiffusion()
        {
            toggleShowDiffusion = pres.ToggleShowDiffusion;
            diffusionGrid = pres.DiffusionGrid;
            diffusionKeyText = pres.UIInputDiffusionKey;
            resetDiffusion = pres.ResetDiffusion;
            pres.DiffusionKey = (byte[])pres.InputKey.Clone();

            InitDiffusionButtons();
            InitDiffusionKey();
            InitDiffusionGridLayout();
            InitDiffusionFlipBitButtons();
        }
        private void ToggleShowDiffusion(object sender, RoutedEventArgs e)
        {
            pres.ShowDiffusion = !pres.ShowDiffusion;
            toggleShowDiffusion.Content = $"{(pres.ShowDiffusion == true ? "Hide" : "Show")} Diffusion display";
        }
        private void ResetDiffusion(object sender, RoutedEventArgs e)
        {
            pres.DiffusionKey = (byte[])pres.InputKey.Clone();
        }
        private void InitDiffusionButtons()
        {
            toggleShowDiffusion.Click += ToggleShowDiffusion;
            resetDiffusion.Click += ResetDiffusion;
        }
        private TextBlock GetDiffusionKeyNibble(int nibbleIndex)
        {
            TextBlock tb = new TextBlock();
            tb.SetBinding(TextBlock.TextProperty, new Binding($"DKeyNibbleHex{nibbleIndex}"));
            tb.SetBinding(TextBlock.ForegroundProperty, new Binding($"DKeyNibble{nibbleIndex}Flipped") { Converter = new BoolToForegroundConverter() });

            Setter markNibble = new Setter() { Property = TextBlock.ForegroundProperty, Value = Brushes.Red };
            Trigger onHover = new Trigger()
            {
                Property = TextBlock.IsMouseOverProperty,
                Value = true
            };
            onHover.Setters.Add(markNibble);

            /*
            DataTrigger onButtonHover = new DataTrigger() {
                Binding = new Binding() { ElementName = $"DKeyBit{nibbleIndex}Button", Path = new PropertyPath("IsMouseOver") },
                Value = true
            };
            onButtonHover.Setters.Add(markNibble);
            */

            Style s = new Style();
            s.Triggers.Add(onHover);
            //s.Triggers.Add(onButtonHover);
            tb.Style = s;
            return tb;
        }
        private FlowDocument GetDiffusionKey()
        {
            FlowDocument doc = new FlowDocument();
            Paragraph para = new Paragraph();
            for (int i = 0; i < (keyIs32Byte ? 32 : 16) * 2; ++i)
            {
                TextBlock nibbleBox = GetDiffusionKeyNibble(i);
                para.Inlines.Add(nibbleBox);
            }
            doc.Blocks.Add(para);
            return doc;
        }
        private FlowDocument GetDiffusionKeyLittleEndian()
        {
            FlowDocument doc = new FlowDocument();
            Paragraph para = new Paragraph();
            for (int i = 0; i < (keyIs32Byte ? 32 : 16) * 2; i+=8)
            {
                for (int j = i + 7; j >= i; j-=2)
                {
                    TextBlock nibble2 = GetDiffusionKeyNibble(j);
                    TextBlock nibble1 = GetDiffusionKeyNibble(j-1);
                    para.Inlines.Add(nibble1);
                    para.Inlines.Add(nibble2);
                }
            }
            doc.Blocks.Add(para);
            return doc;
        }
        private void InitDiffusionKey()
        {
            diffusionKeyText.Document = GetDiffusionKey();
        }
        private void InitDiffusionGridLayout()
        {
            diffusionGrid.ColumnDefinitions.Clear();
            diffusionGrid.RowDefinitions.Clear();
            for (int i = 0; i < 64; ++i)
            {
                diffusionGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            diffusionGrid.RowDefinitions.Add(new RowDefinition());
            diffusionGrid.RowDefinitions.Add(new RowDefinition());
            if (keyIs32Byte)
            {
                diffusionGrid.RowDefinitions.Add(new RowDefinition());
                diffusionGrid.RowDefinitions.Add(new RowDefinition());
            }
        }
        private void FlipDiffusionBit(int bitIndex)
        {
            int bit = pres.DKeyBit(bitIndex);
            if (bit == 1)
            {
                pres.UnsetDKeyBit(bitIndex);
            }
            else
            {
                pres.SetDKeyBit(bitIndex);
            }
        }
        class BoolToForegroundConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (bool)value == true ? Brushes.Red : Brushes.Black;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value != Brushes.Black;
            }
        }
        private Button CreateDiffusionButton(int bitIndex)
        {
            // Bit indices start at 0 on the most significant bit which is in the string representation in big endian notation.
            // This means we start counting from zero at the left but the zero-th bit is - maybe a bit unintuitively - the most significant bit.
            Button b = new Button() { Height = 24, FontSize = 10 };
            b.SetBinding(Button.ContentProperty, new Binding($"DKeyBit{bitIndex}"));
            b.SetBinding(Button.ForegroundProperty, new Binding($"DKeyBit{bitIndex}Flipped") { Converter = new BoolToForegroundConverter() });
            b.Margin = new Thickness(bitIndex % 4 == 0 ? 3 : 0, 0, 0, 3);
            b.Name = $"DKeyBit{bitIndex}Button";
            b.Click += (obj, e) => FlipDiffusionBit(bitIndex);
            return b;
        }
        private void InitDiffusionFlipBitButtons()
        {
            for (int i = 0; i < (keyIs32Byte ? 256 : 128); ++i)
            {
                Button b = CreateDiffusionButton(i);
                Grid.SetRow(b, i / 64);
                Grid.SetColumn(b, i % 64);
                diffusionGrid.Children.Add(b);
            }
        }
        #endregion
        
        #region Low Level API (independent of Constants, Key, Counter, IV)

        #region Description
        private void AddBoldToDescription(string descToAdd)
        {
            pres.Nav.AddBold(pres.UIStateMatrixStepDescription, descToAdd);
        }
        private void ClearDescription()
        {
            pres.Nav.Clear(pres.UIStateMatrixStepDescription);
        }
        private void UnboldLastFromDescription()
        {
            pres.Nav.UnboldLast(pres.UIStateMatrixStepDescription);
        }
        private void MakeLastBoldInDescription()
        {
            pres.Nav.MakeBoldLast(pres.UIStateMatrixStepDescription);
        }
        private void RemoveLastFromDescription()
        {
            pres.Nav.RemoveLast(pres.UIStateMatrixStepDescription);
        }
        #endregion

        #region TransformInput
        private void ReplaceTransformInput(string input)
        {
            pres.Nav.Replace(pres.UITransformInput, input);
        }
        private void SetTextBindingToTransformInput(string bindingVarName)
        {
            pres.Nav.SetTextBinding(pres.UITransformInput, bindingVarName);
        }
        private void ClearTransformInput()
        {
            pres.Nav.Clear(pres.UITransformInput);
            pres.Nav.ClearTextBinding(pres.UITransformInput);
        }
        #endregion

        #region TransformChunk
        private void ReplaceTransformChunk(params string[] chunk)
        {
            if (chunk.Length == 2)
            {
                // use borders in center to center text
                pres.Nav.Replace(pres.UITransformChunk1, chunk[0]);
                pres.Nav.Replace(pres.UITransformChunk2, chunk[1]);
            }
            else
            {
                for (int i = 0; i < chunk.Length; ++i)
                {
                    pres.Nav.Replace((TextBox)pres.FindName($"UITransformChunk{i}"), chunk[i]);
                }
            }
        }
        private void SetTextBindingToTransformChunk(params string[] bindingVarNames)
        {
            if (bindingVarNames.Length == 2)
            {
                // use borders in center to center text
                pres.Nav.SetTextBinding(pres.UITransformChunk1, bindingVarNames[0]);
                pres.Nav.SetTextBinding(pres.UITransformChunk2, bindingVarNames[1]);
            }
            else
            {
                for (int i = 0; i < bindingVarNames.Length; ++i)
                {
                    pres.Nav.SetTextBinding((TextBox)pres.FindName($"UITransformChunk{i}"), bindingVarNames[i]);
                }
            }
        }
        private void ClearTransformChunk()
        {
            for(int i = 0; i < 8; ++i)
            {
                TextBox tb = (TextBox)pres.FindName($"UITransformChunk{i}");
                pres.Nav.Clear(tb);
                pres.Nav.ClearTextBinding(tb);
            }
        }
        #endregion TransformChunk

        #region TransformLittleEndian
        private void ReplaceTransformLittleEndian(params string[] le)
        {
            // TODO create another grid with 3 rows to center IV in IETF version and add branch for le.Length == 3 (IV IETF) and le.Length == 1 (counter IETF) here
            if (le.Length == 2)
            {
                // use borders in center to center text
                pres.Nav.Replace(pres.UITransformLittleEndian1, le[0]);
                pres.Nav.Replace(pres.UITransformLittleEndian2, le[1]);
            }
            else
            {
                for (int i = 0; i < le.Length; ++i)
                {
                    pres.Nav.Replace((TextBox)pres.FindName($"UITransformLittleEndian{i}"), le[i]);
                }
            }
        }
        private void SetTextBindingToTransformLittleEndian(params string[] bindingVarNames)
        {
            // TODO create another grid with 3 rows to center IV in IETF version and add branch for le.Length == 3 (IV IETF) and le.Length == 1 (counter IETF) here
            if (bindingVarNames.Length == 2)
            {
                // use borders in center to center text
                pres.Nav.SetTextBinding(pres.UITransformLittleEndian1, bindingVarNames[0]);
                pres.Nav.SetTextBinding(pres.UITransformLittleEndian2, bindingVarNames[1]);
            }
            else
            {
                for (int i = 0; i < bindingVarNames.Length; ++i)
                {
                    pres.Nav.SetTextBinding((TextBox)pres.FindName($"UITransformLittleEndian{i}"), bindingVarNames[i]);
                }
            }
        }
        private void ClearTransformLittleEndian()
        {
            for (int i = 0; i < 8; ++i)
            {
                TextBox tb = (TextBox)pres.FindName($"UITransformLittleEndian{i}");
                pres.Nav.Clear(tb);
                pres.Nav.ClearTextBinding(tb);
            }
        }
        #endregion
            
        #endregion
    }

    partial class Page
    {
        public static StateMatrixPage StateMatrixPage(ChaChaPresentation pres)
        {
            // using static function as factory to hide the name assigned to the KeystreamBlockGenPage ContentControl element in the XAML code
            return new StateMatrixPage(pres.UIStateMatrixPage, pres);
        }
    }
}