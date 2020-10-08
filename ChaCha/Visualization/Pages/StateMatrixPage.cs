using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Cryptool.Plugins.ChaCha
{
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
            descriptions.Add("The 512-bit (128-byte) ChaCha state can be interpreted as a 4x4 matrix, where each entry consists of 4 bytes interpreted as little-endian. The first 16 bytes consist of the constants. ");
            descriptions.Add("The next 32 bytes consist of the key. If the key consists of only 16 bytes, it is concatenated with itself. ");
            descriptions.Add($"The next { pres.InitialCounter.Length} bytes consist of the counter.Since this is our first keystream block, we set the counter to zero.The counter is special since we first reverse all bytes before applying the transformations but since the counter is zero, this does not matter for the first block. ");
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

        private void InitDiffusion()
        {
            toggleShowDiffusion = pres.ToggleShowDiffusion;
            diffusionGrid = pres.DiffusionGrid;
            diffusionKeyText = pres.DiffusionKeyTextBlock;
            resetDiffusion = pres.ResetDiffusion;
            pres.DiffusionKey = (byte[])pres.InputKey.Clone();

            InitDiffusionButtons();
            InitDiffusionKey();
            InitDiffusionGridLayout();
            InitDiffusionFlipBitButtons();
        }
        private void ToggleDiffusion(object sender, RoutedEventArgs e)
        {
            pres.ShowDiffusion = !pres.ShowDiffusion;
            toggleShowDiffusion.Content = $"{(pres.ShowDiffusion == true ? "Verstecke" : "Zeige")} Diffusionsanzeige";
        }
        private void ResetDiffusion(object sender, RoutedEventArgs e)
        {
            pres.DiffusionKey = (byte[])pres.InputKey.Clone();
        }
        private void InitDiffusionButtons()
        {
            toggleShowDiffusion.Click += ToggleDiffusion;
            resetDiffusion.Click += ResetDiffusion;
        }
        private TextBlock CreateDiffusionKeyNibbleTextBox(int nibbleIndex)
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
        private FlowDocument GetDiffusionKeyFlowDocument()
        {
            FlowDocument doc = new FlowDocument();
            Paragraph para = new Paragraph();
            for (int i = 0; i < (keyIs32Byte ? 32 : 16) * 2; ++i)
            {
                TextBlock nibbleBox = CreateDiffusionKeyNibbleTextBox(i);
                para.Inlines.Add(nibbleBox);
            }
            doc.Blocks.Add(para);
            return doc;
        }
        private void InitDiffusionKey()
        {
            diffusionKeyText.Document = GetDiffusionKeyFlowDocument();
        }
        private void InitDiffusionGridLayout()
        {
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
            if(bit == 1)
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
            Button b = new Button() { Height = 16, FontSize = 10 };
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
        private void ReplaceTransformInput(string input)
        {
            if (input.Length > 32)
            {
                pres.Nav.Replace(pres.UITransformInput, input.Substring(0, 32));
                pres.Nav.Replace(pres.UITransformInput2, input.Substring(32));
            }
            else
            {
                pres.Nav.Replace(pres.UITransformInput, input);
            }
        }
        private void ClearTransformInput()
        {
            pres.Nav.Clear(pres.UITransformInput, pres.UITransformInput2);
            pres.Nav.ClearDocument(pres.UITransformInputDiffusion, pres.UITransformInputDiffusion2);
            pres.Nav.Collapse(pres.UITransformInputCell, pres.UITransformInputCell2);
        }
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
        private void ClearTransformChunk()
        {
            pres.Nav.Clear(pres.UITransformChunk0);
            pres.Nav.Clear(pres.UITransformChunk1);
            pres.Nav.Clear(pres.UITransformChunk2);
            pres.Nav.Clear(pres.UITransformChunk3);
            pres.Nav.Clear(pres.UITransformChunk4);
            pres.Nav.Clear(pres.UITransformChunk5);
            pres.Nav.Clear(pres.UITransformChunk6);
            pres.Nav.Clear(pres.UITransformChunk7);
        }
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
        private void ClearTransformLittleEndian()
        {
            pres.Nav.Clear(pres.UITransformLittleEndian0);
            pres.Nav.Clear(pres.UITransformLittleEndian1);
            pres.Nav.Clear(pres.UITransformLittleEndian2);
            pres.Nav.Clear(pres.UITransformLittleEndian3);
            pres.Nav.Clear(pres.UITransformLittleEndian4);
            pres.Nav.Clear(pres.UITransformLittleEndian5);
            pres.Nav.Clear(pres.UITransformLittleEndian6);
            pres.Nav.Clear(pres.UITransformLittleEndian7);
        }
        private void ReplaceTransformChunkCounter()
        {
            if (versionIsDJB)
            {
                ReplaceTransformChunk(pres.InitialCounterChunks[0], pres.InitialCounterChunks[1]);
            }
            else
            {
                ReplaceTransformChunk(pres.InitialCounterChunks[0]);
            }
        }
        private void ReplaceTransformLittleEndianCounter()
        {
            if (versionIsDJB)
            {
                ReplaceTransformLittleEndian(pres.InitialCounterLittleEndian[0], pres.InitialCounterLittleEndian[1]);
            }
            else
            {
                ReplaceTransformLittleEndian(pres.InitialCounterLittleEndian[0]);
            }
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
        private PageAction[] InputAction(Border copyFrom, string text = null)
        {
            if (text == null)
            {
                text = ((TextBox)copyFrom.Child).Text;
            }
            PageAction[] copyActions = pres.Nav.CopyActions(new Border[] { copyFrom }, new Border[] { pres.UITransformInputCell }, new string[] { "" });
            if (text.Length > 32)
            {
                copyActions[1].AddToExec(() => {
                    pres.Nav.SetCopyBackground(pres.UITransformInputCell2);
                    ReplaceTransformInput(text);
                });
                copyActions[1].AddToUndo(() =>
                {
                    pres.Nav.UnsetBackground(pres.UITransformInputCell2);
                    ClearTransformInput();
                });
                copyActions[2].AddToExec(() => { pres.Nav.UnsetBackground(pres.UITransformInputCell2); });
                copyActions[2].AddToUndo(() => { pres.Nav.SetCopyBackground(pres.UITransformInputCell2); });
            }
            return copyActions;
        }
        private PageAction[] KeyInputAction()
        {
            Border inputKeyCell = pres.UIInputKeyCell;
            string text = pres.UIInputKey.Text;
            PageAction[] copyActions = InputAction(inputKeyCell, text);
            if (pres.InputKey.Length == 16)
            {
                copyActions[1].AddToExec(() =>
                {
                    pres.Nav.SetCopyBackground(pres.UITransformInputCell2);
                    ReplaceTransformInput($"{text}{text}");
                });
                copyActions[1].AddToUndo(() =>
                {
                    pres.Nav.UnsetBackground(pres.UITransformInputCell2);
                    ClearTransformInput();
                });
                copyActions[2].AddToExec(() => { pres.Nav.UnsetBackground(pres.UITransformInputCell2); });
                copyActions[2].AddToUndo(() => { pres.Nav.SetCopyBackground(pres.UITransformInputCell2); });
            }
            copyActions[1].Add(AddTransformInputDiffusion());
            return copyActions;
        }
        private PageAction AddTransformInputDiffusion()
        {
            return new PageAction(() =>
            {
                if(pres.ShowDiffusion)
                {
                    FlowDocument fullDKey = GetDiffusionKeyFlowDocument();
                    FlowDocument dKeyRow1 = fullDKey;
                    FlowDocument dKeyRow2 = fullDKey;
                    if(pres.InputKey.Length == 32)
                    {
                        // split diffusion key into two rows
                        Paragraph fullDKeyParagraph = (Paragraph)fullDKey.Blocks.FirstBlock;
                        Console.WriteLine($"full inlines count: {fullDKeyParagraph.Inlines.Count}");
                        dKeyRow1 = new FlowDocument() { TextAlignment = TextAlignment.Center };
                        Paragraph dKeyRow1Paragraph = new Paragraph();
                        // first 32 nibbles
                        dKeyRow1Paragraph.Inlines.AddRange(fullDKeyParagraph.Inlines.ToArray().Take(32));
                        Console.WriteLine($"full inlines count: {fullDKeyParagraph.Inlines.Count}");
                        dKeyRow1.Blocks.Add(dKeyRow1Paragraph);
                        Console.WriteLine($"row1 inlines count: {dKeyRow1Paragraph.Inlines.Count}");
                        dKeyRow2 = new FlowDocument() { TextAlignment = TextAlignment.Center };
                        Paragraph dKeyRow2Paragraph = new Paragraph();
                        // last 32 nibbles
                        dKeyRow2Paragraph.Inlines.AddRange(fullDKeyParagraph.Inlines.ToArray().Take(32));
                        Console.WriteLine($"full inlines count: {fullDKeyParagraph.Inlines.Count}");
                        dKeyRow2.Blocks.Add(dKeyRow2Paragraph);
                        Console.WriteLine($"row2 inlines count: {dKeyRow2Paragraph.Inlines.Count}");
                    }
                    pres.Nav.SetDocument(pres.UITransformInputDiffusion, dKeyRow1);
                    pres.Nav.SetDocument(pres.UITransformInputDiffusion2, dKeyRow2);
                    pres.Nav.Show(pres.UITransformInputDiffusionCell);
                    pres.Nav.Show(pres.UITransformInputDiffusionCell2);
                }
            }, () =>
            {
                if (pres.ShowDiffusion)
                {
                    pres.Nav.Collapse(pres.UITransformInputDiffusionCell);
                    pres.Nav.Collapse(pres.UITransformInputDiffusionCell2);
                    pres.Nav.ClearDocument(pres.UITransformInputDiffusion);
                    pres.Nav.ClearDocument(pres.UITransformInputDiffusion2);
                }
            });
        }

        private void AddConstantsActions()
        {
            PageAction constantsStepDescriptionAction = new PageAction(() =>
            {
                AddBoldToDescription(descriptions[0]);
            }, () =>
            {
                ClearDescription();
            });
            PageAction[] constantsInputAction = InputAction(pres.UIConstantsCell);
            PageAction constantsChunksAction = new PageAction(() =>
            {
                ReplaceTransformChunk(pres.ConstantsChunks[0], pres.ConstantsChunks[1], pres.ConstantsChunks[2], pres.ConstantsChunks[3]);
            }, () =>
            {
                ClearTransformChunk();
            });
            PageAction constantsLittleEndianAction = new PageAction(() =>
            {
                ReplaceTransformLittleEndian(pres.ConstantsLittleEndian[0], pres.ConstantsLittleEndian[1], pres.ConstantsLittleEndian[2], pres.ConstantsLittleEndian[3]);
            }, () =>
            {
                ClearTransformLittleEndian();
            });
            PageAction[] copyConstantsToStateActions = pres.Nav.CopyActions(
                new Border[] { pres.UITransformLittleEndian0Cell, pres.UITransformLittleEndian1Cell, pres.UITransformLittleEndian2Cell, pres.UITransformLittleEndian3Cell },
                new Border[] { pres.UIState0Cell, pres.UIState1Cell, pres.UIState2Cell, pres.UIState3Cell },
                new string[] { "", "", "", "" });
            AddAction(constantsStepDescriptionAction);
            AddAction(constantsInputAction);
            AddAction(constantsChunksAction);
            AddAction(constantsLittleEndianAction);
            AddAction(copyConstantsToStateActions);
        }
        private void AddKeyActions()
        {
            PageAction keyStepDescriptionAction = new PageAction(() =>
            {
                UnboldLastFromDescription();
                AddBoldToDescription(descriptions[1]);
                ClearTransformInput();
                ClearTransformChunk();
                ClearTransformLittleEndian();
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
                ReplaceTransformInput(pres.HexConstants);
                ReplaceTransformChunk(pres.ConstantsChunks[0], pres.ConstantsChunks[1], pres.ConstantsChunks[2], pres.ConstantsChunks[3]);
                ReplaceTransformLittleEndian(pres.ConstantsLittleEndian[0], pres.ConstantsLittleEndian[1], pres.ConstantsLittleEndian[2], pres.ConstantsLittleEndian[3]);
            });
            PageAction[] keyInputAction = KeyInputAction();
            PageAction keyChunksAction = new PageAction(() =>
            {
                ReplaceTransformChunk(
                    pres.KeyChunks[0], pres.KeyChunks[1], pres.KeyChunks[2], pres.KeyChunks[3],
                    pres.KeyChunks[keyIs32Byte ? 4 : 0], pres.KeyChunks[keyIs32Byte ? 5 : 1], pres.KeyChunks[keyIs32Byte ? 6 : 2], pres.KeyChunks[keyIs32Byte ? 7 : 3]);
            }, () =>
            {
                ClearTransformChunk();
            });
            PageAction keyLittleEndianAction = new PageAction(() =>
            {
                ReplaceTransformLittleEndian(
                    pres.KeyLittleEndian[0], pres.KeyLittleEndian[1], pres.KeyLittleEndian[2], pres.KeyLittleEndian[3],
                    pres.KeyLittleEndian[keyIs32Byte ? 4 : 0], pres.KeyLittleEndian[keyIs32Byte ? 5 : 1], pres.KeyLittleEndian[keyIs32Byte ? 6 : 2], pres.KeyLittleEndian[keyIs32Byte ? 7 : 3]);
            }, () =>
            {
                ClearTransformLittleEndian();
            });
            PageAction[] copyKeyToStateActions = pres.Nav.CopyActions(
                new Border[] { pres.UITransformLittleEndian0Cell, pres.UITransformLittleEndian1Cell, pres.UITransformLittleEndian2Cell, pres.UITransformLittleEndian3Cell, pres.UITransformLittleEndian4Cell, pres.UITransformLittleEndian5Cell, pres.UITransformLittleEndian6Cell, pres.UITransformLittleEndian7Cell },
                new Border[] { pres.UIState4Cell, pres.UIState5Cell, pres.UIState6Cell, pres.UIState7Cell, pres.UIState8Cell, pres.UIState9Cell, pres.UIState10Cell, pres.UIState11Cell },
                new string[] { "", "", "", "", "", "", "", "" });
            AddAction(keyStepDescriptionAction);
            AddAction(keyInputAction);
            AddAction(keyChunksAction);
            AddAction(keyLittleEndianAction);
            AddAction(copyKeyToStateActions);
        }
        private void AddCounterActions()
        {
            PageAction counterStepDescriptionAction = new PageAction(() =>
            {
                UnboldLastFromDescription();
                AddBoldToDescription(descriptions[2]);
                ClearTransformInput();
                ClearTransformChunk();
                ClearTransformLittleEndian();
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
                ReplaceTransformInput(pres.HexInputKey);
                ReplaceTransformChunk(
                    pres.KeyChunks[0], pres.KeyChunks[1], pres.KeyChunks[2], pres.KeyChunks[3],
                    pres.KeyChunks[keyIs32Byte ? 4 : 0], pres.KeyChunks[keyIs32Byte ? 5 : 1], pres.KeyChunks[keyIs32Byte ? 6 : 2], pres.KeyChunks[keyIs32Byte ? 7 : 3]);
                ReplaceTransformLittleEndian(
                    pres.KeyLittleEndian[0], pres.KeyLittleEndian[1], pres.KeyLittleEndian[2], pres.KeyLittleEndian[3],
                    pres.KeyLittleEndian[keyIs32Byte ? 4 : 0], pres.KeyLittleEndian[keyIs32Byte ? 5 : 1], pres.KeyLittleEndian[keyIs32Byte ? 6 : 2], pres.KeyLittleEndian[keyIs32Byte ? 7 : 3]);
            });
            PageAction counterInputAction = new PageAction(() =>
            {
                ReplaceTransformInput(pres.HexInitialCounter);
            }, () =>
            {
                ClearTransformInput();
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
            PageAction[] copyCounterToStateActions = versionIsDJB ?
                pres.Nav.CopyActions(
                    new Border[] { pres.UITransformLittleEndian1Cell, pres.UITransformLittleEndian2Cell },
                    new Border[] { pres.UIState12Cell, pres.UIState13Cell },
                    new string[] { "", "" })
                // TODO create another grid with 3 rows to center counter in IETF version and update this code here
                : pres.Nav.CopyActions(
                    new Border[] { pres.UITransformLittleEndian0Cell },
                    new Border[] { pres.UIState12Cell },
                    new string[] { "" });
            AddAction(counterStepDescriptionAction);
            AddAction(counterInputAction);
            AddAction(counterChunksAction);
            AddAction(counterLittleEndianAction);
            AddAction(copyCounterToStateActions);
        }
        private void AddIVActions()
        {
            PageAction ivStepDescriptionAction = new PageAction(() =>
            {
                UnboldLastFromDescription();
                AddBoldToDescription(descriptions[3]);
                ClearTransformInput();
                ClearTransformChunk();
                ClearTransformLittleEndian();
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
                ReplaceTransformInput(pres.HexInitialCounter);
                ReplaceTransformChunkCounter();
                ReplaceTransformLittleEndianCounter();
            });
            PageAction[] ivInputAction = InputAction(pres.UIInputIVCell);
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
            PageAction[] copyIVToStateActions = versionIsDJB ?
                pres.Nav.CopyActions(
                    new Border[] { pres.UITransformLittleEndian1Cell, pres.UITransformLittleEndian2Cell },
                    new Border[] { pres.UIState14Cell, pres.UIState15Cell },
                    new string[] { "", "" })
                // TODO create another grid with 3 rows to center IV in IETF version and update this code here
                : pres.Nav.CopyActions(
                    new Border[] { pres.UITransformLittleEndian0Cell, pres.UITransformLittleEndian1Cell, pres.UITransformLittleEndian2Cell },
                    new Border[] { pres.UIState13Cell, pres.UIState14Cell, pres.UIState15Cell },
                    new string[] { "", "", "" });
            AddAction(ivStepDescriptionAction);
            AddAction(ivInputAction);
            AddAction(ivChunksAction);
            AddAction(ivLittleEndianAction);
            AddAction(copyIVToStateActions);
        }
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