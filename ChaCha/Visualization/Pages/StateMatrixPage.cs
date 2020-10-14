using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using Cryptool.Plugins.Chacha.Extensions;

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

        #region Constants
        private void AddConstantsActions()
        {
            PageAction constantsStepDescriptionAction = new PageAction(() =>
            {
                AddConstantsStepBoldToDescription();
            }, () =>
            {
                ClearDescription();
            });
            PageAction[] constantsInputAction = ConstantsInputAction();
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
            PageAction[] copyConstantsToStateActions = CopyConstantsToStateActions();
            AddAction(constantsStepDescriptionAction);
            AddAction(constantsInputAction);
            AddAction(constantsChunksAction);
            AddAction(constantsLittleEndianAction);
            AddAction(copyConstantsToStateActions);
        }
        private void AddConstantsStepBoldToDescription()
        {
            AddBoldToDescription(descriptions[0]);
        }
        private PageAction[] ConstantsInputAction()
        {
            return InputAction(pres.UIConstantsCell);
        }
        private void ReplaceTransformInputConstants()
        {
            ReplaceTransformInput(pres.HexInitialCounter);
        }
        private void ReplaceTransformChunkConstants()
        {
            ReplaceTransformChunk(pres.ConstantsChunks[0], pres.ConstantsChunks[1], pres.ConstantsChunks[2], pres.ConstantsChunks[3]);
        }
        private void ReplaceTransformLittleEndianConstants()
        {
            ReplaceTransformLittleEndian(pres.ConstantsLittleEndian[0], pres.ConstantsLittleEndian[1], pres.ConstantsLittleEndian[2], pres.ConstantsLittleEndian[3]);
        }
        private PageAction[] CopyConstantsToStateActions()
        {
            return pres.Nav.CopyActions(
                new Border[] { pres.UITransformLittleEndianCell0, pres.UITransformLittleEndianCell1, pres.UITransformLittleEndianCell2, pres.UITransformLittleEndianCell3 },
                new Border[] { pres.UIStateCell0, pres.UIStateCell1, pres.UIStateCell2, pres.UIStateCell3 },
                new string[] { "", "", "", "" });
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
            PageAction[] keyInputAction = KeyInputAction();
            PageAction keyChunksAction = new PageAction(() =>
            {
                ReplaceTransformChunkKey();
            }, () =>
            {
                ClearTransformChunk();
            });
            PageAction keyLittleEndianAction = new PageAction(() =>
            {
                ReplaceTransformLittleEndianKey();
            }, () =>
            {
                ClearTransformLittleEndian();
            });
            PageAction[] copyKeyToStateActions = CopyKeyToStateActions();
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
            copyActions[1].Add(TransformInputDiffusionAction());
            return copyActions;
        }
        private void ReplaceTransformInputKey()
        {
            // Ternary operator must return something thus wrapping function into action. (Guess I really wanted a one-liner for this and not if-else.)
            // https://stackoverflow.com/questions/5490095/method-call-using-ternary-operator
            (pres.DiffusionActive ? new Action(ReplaceTransformInputDiffusion) : new Action(() => ReplaceTransformInput(pres.HexInputKey)))();
        }
        private void ReplaceTransformChunkKey()
        {
            ReplaceTransformChunk(
                    pres.KeyChunks[0], pres.KeyChunks[1], pres.KeyChunks[2], pres.KeyChunks[3],
                    pres.KeyChunks[keyIs32Byte ? 4 : 0], pres.KeyChunks[keyIs32Byte ? 5 : 1], pres.KeyChunks[keyIs32Byte ? 6 : 2], pres.KeyChunks[keyIs32Byte ? 7 : 3]);
            if (pres.DiffusionActive)
            {
                ReplaceTransformChunkDiffusion();
            }
        }
        private void ReplaceTransformLittleEndianKey()
        {
            ReplaceTransformLittleEndian(
                   pres.KeyLittleEndian[0], pres.KeyLittleEndian[1], pres.KeyLittleEndian[2], pres.KeyLittleEndian[3],
                   pres.KeyLittleEndian[keyIs32Byte ? 4 : 0], pres.KeyLittleEndian[keyIs32Byte ? 5 : 1], pres.KeyLittleEndian[keyIs32Byte ? 6 : 2], pres.KeyLittleEndian[keyIs32Byte ? 7 : 3]);
            if(pres.DiffusionActive)
            {
                ReplaceTransformLittleEndianDiffusion();
            }
        }
        private PageAction[] CopyKeyToStateActions()
        {
            PageAction[] copyActions = pres.Nav.CopyActions(
                new Border[] { pres.UITransformLittleEndianCell0, pres.UITransformLittleEndianCell1, pres.UITransformLittleEndianCell2, pres.UITransformLittleEndianCell3, pres.UITransformLittleEndianCell4, pres.UITransformLittleEndianCell5, pres.UITransformLittleEndianCell6, pres.UITransformLittleEndianCell7 },
                new Border[] { pres.UIStateCell4, pres.UIStateCell5, pres.UIStateCell6, pres.UIStateCell7, pres.UIStateCell8, pres.UIStateCell9, pres.UIStateCell10, pres.UIStateCell11 },
                new string[] { "", "", "", "", "", "", "", "" });
            copyActions[1].Add(AddCopyDiffusionKeyToStateActions());
            return copyActions;
        }
        #endregion

        #region Counter
        private void AddCounterActions()
        {
            PageAction counterStepDescriptionAction = new PageAction(() =>
            {
                UnboldLastFromDescription();
                AddCounterStepBoldToDescription();
                ClearTransformInput();
                ClearTransformChunk();
                ClearTransformLittleEndian();
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
                ReplaceTransformInputKey();
                ReplaceTransformChunkKey();
                ReplaceTransformLittleEndianKey();
            });
            PageAction counterInputAction = new PageAction(() =>
            {
                ReplaceTransformInputCounter();
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
            PageAction[] copyCounterToStateActions = CopyCounterToStateActions();
            AddAction(counterStepDescriptionAction);
            AddAction(counterInputAction);
            AddAction(counterChunksAction);
            AddAction(counterLittleEndianAction);
            AddAction(copyCounterToStateActions);
        }
        private void AddCounterStepBoldToDescription()
        {
            AddBoldToDescription(descriptions[2]);
        }
        private void ReplaceTransformInputCounter()
        {
            ReplaceTransformInput(pres.HexInitialCounter);
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
        private PageAction[] CopyCounterToStateActions()
        {
            return versionIsDJB ?
                pres.Nav.CopyActions(
                    new Border[] { pres.UITransformLittleEndianCell1, pres.UITransformLittleEndianCell2 },
                    new Border[] { pres.UIStateCell12, pres.UIStateCell13 },
                    new string[] { "", "" })
                // TODO create another grid with 3 rows to center counter in IETF version and update this code here
                : pres.Nav.CopyActions(
                    new Border[] { pres.UITransformLittleEndianCell0 },
                    new Border[] { pres.UIStateCell12 },
                    new string[] { "" });
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
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
                ReplaceTransformInputCounter();
                ReplaceTransformChunkCounter();
                ReplaceTransformLittleEndianCounter();
            });
            PageAction[] ivInputAction = IVInputAction();
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
            PageAction[] copyIVToStateActions = CopyIVToStateActions();
            AddAction(ivStepDescriptionAction);
            AddAction(ivInputAction);
            AddAction(ivChunksAction);
            AddAction(ivLittleEndianAction);
            AddAction(copyIVToStateActions);
        }
        private void AddIVStepBoldToDescription()
        {
            AddBoldToDescription(descriptions[3]);
        }
        private PageAction[] IVInputAction()
        {
            return InputAction(pres.UIInputIVCell);
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
        private PageAction[] CopyIVToStateActions()
        {
            return versionIsDJB ?
                pres.Nav.CopyActions(
                    new Border[] { pres.UITransformLittleEndianCell1, pres.UITransformLittleEndianCell2 },
                    new Border[] { pres.UIStateCell14, pres.UIStateCell15 },
                    new string[] { "", "" })
                // TODO create another grid with 3 rows to center IV in IETF version and update this code here
                : pres.Nav.CopyActions(
                    new Border[] { pres.UITransformLittleEndianCell0, pres.UITransformLittleEndianCell1, pres.UITransformLittleEndianCell2 },
                    new Border[] { pres.UIStateCell13, pres.UIStateCell14, pres.UIStateCell15 },
                    new string[] { "", "", "" });
        }
        #endregion

        #region Diffusion
        private void ReplaceTransformInputDiffusion()
        {
            FlowDocument fullDKey = GetDiffusionKey();
            FlowDocument dKeyRow1 = fullDKey;
            FlowDocument dKeyRow2 = fullDKey;
            if (pres.InputKey.Length == 32)
            {
                (dKeyRow1, dKeyRow2, _) = SplitDocument(fullDKey, 2);
            }
            pres.Nav.SetDocument(pres.UITransformInputDiffusion, dKeyRow1);
            pres.Nav.SetDocument(pres.UITransformInputDiffusion2, dKeyRow2);
            pres.Nav.Show(pres.UITransformInputDiffusionCell);
            pres.Nav.Show(pres.UITransformInputDiffusionCell2);
        }
        private void ClearTransformInputDiffusion()
        {
            pres.Nav.Collapse(pres.UITransformInputDiffusionCell, pres.UITransformInputDiffusionCell2);
            pres.Nav.Clear(pres.UITransformInputDiffusion, pres.UITransformInputDiffusion2);
        }
        private PageAction TransformInputDiffusionAction()
        {
            return new PageAction(() =>
            {
                if (pres.DiffusionActive)
                {
                    ReplaceTransformInputDiffusion();
                }
            }, () =>
            {
                if (pres.DiffusionActive)
                {
                    ClearTransformInputDiffusion();
                }
            });
        }
        private void ReplaceTransformChunkDiffusion()
        {
            FlowDocument fullDKey = GetDiffusionKey();
            FlowDocument[] chunkDocs = SplitDocument(fullDKey, 8);
            for (int i = 0; i < 8; ++i)
            {
                RichTextBox diffusionChunk = (RichTextBox)pres.FindName($"UITransformChunkDiffusion{i}");
                Border diffusionChunkCell = (Border)diffusionChunk.Parent;
                pres.Nav.SetDocument(diffusionChunk, chunkDocs[i]);
                pres.Nav.Show(diffusionChunkCell);
            }
        }
        private void ReplaceTransformLittleEndianDiffusion()
        {
            FlowDocument fullDKeyLittleEndian = GetDiffusionKeyLittleEndian();
            FlowDocument[] chunkDocs = SplitDocument(fullDKeyLittleEndian, 8);
            for (int i = 0; i < 8; ++i)
            {
                RichTextBox diffusionChunk = (RichTextBox)pres.FindName($"UITransformLittleEndianDiffusion{i}");
                Border diffusionChunkCell = (Border)diffusionChunk.Parent;
                pres.Nav.SetDocument(diffusionChunk, chunkDocs[i]);
                pres.Nav.Show(diffusionChunkCell);
            }
        }
        private PageAction AddCopyDiffusionKeyToStateActions()
        {
            PageAction addDKeyToState = new PageAction(() =>
            {
                if (!pres.DiffusionActive) return;
                FlowDocument fullDKeyLittleEndian = GetDiffusionKeyLittleEndian();
                FlowDocument[] chunkDocs = SplitDocument(fullDKeyLittleEndian, 8);
                for (int i = 4; i < 12; ++i)
                {
                    RichTextBox diffusionState = (RichTextBox)pres.FindName($"UIStateDiffusion{i}");
                    Border diffusionStateCell = (Border)diffusionState.Parent;
                    pres.Nav.SetDocument(diffusionState, chunkDocs[i - 4]);
                    pres.Nav.Show(diffusionStateCell);
                }
            }, () =>
            {
                if (!pres.DiffusionActive) return;
                for (int i = 4; i < 12; ++i)
                {
                    RichTextBox diffusionState = (RichTextBox)pres.FindName($"UIStateDiffusion{i}");
                    Border diffusionStateCell = (Border)diffusionState.Parent;
                    pres.Nav.Clear(diffusionState);
                    pres.Nav.Collapse(diffusionStateCell);
                }
            });
            return addDKeyToState;
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
            diffusionKeyText = pres.DiffusionKeyTextBlock;
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
            toggleShowDiffusion.Content = $"{(pres.ShowDiffusion == true ? "Verstecke" : "Zeige")} Diffusionsanzeige";
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
            pres.Nav.Clear(pres.UITransformInputDiffusion, pres.UITransformInputDiffusion2);
            pres.Nav.Collapse(pres.UITransformInputDiffusionCell, pres.UITransformInputDiffusionCell2);
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
        private void ClearTransformChunk()
        {
            for(int i = 0; i < 8; ++i)
            {
                pres.Nav.Clear((TextBox)pres.FindName($"UITransformChunk{i}"));
                RichTextBox diffusionChunk = (RichTextBox)pres.FindName($"UITransformChunkDiffusion{i}");
                pres.Nav.Clear(diffusionChunk);
                pres.Nav.Collapse((Border)diffusionChunk.Parent);
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
        private void ClearTransformLittleEndian()
        {
            for (int i = 0; i < 8; ++i)
            {
                pres.Nav.Clear((TextBox)pres.FindName($"UITransformLittleEndian{i}"));
                pres.Nav.UnsetBackground((Border)pres.FindName($"UITransformLittleEndianCell{i}"));
                RichTextBox diffusionChunk = (RichTextBox)pres.FindName($"UITransformLittleEndianDiffusion{i}");
                pres.Nav.Clear(diffusionChunk);
                pres.Nav.Collapse((Border)diffusionChunk.Parent);
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