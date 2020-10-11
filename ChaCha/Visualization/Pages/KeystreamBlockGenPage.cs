﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cryptool.Plugins.ChaCha
{
    class KeystreamBlockGenPage : Page
    {
        ChaChaPresentation pres;

        const string ACTIONLABEL_QR_START = "QUARTERROUND_START";
        const string ACTIONLABEL_ROUND_START = "ROUND";
        const string ACTIONLABEL_QR_END = "QUARTERROUND_END";

        List<string> descriptions = new List<string>();
        private string[] originalState;
        private bool versionIsDJB;
        private ulong keyBlockNr;
        public KeystreamBlockGenPage(ContentControl pageElement, ChaChaPresentation pres_, ulong keyblockNr_) : base(pageElement)
        {
            pres = pres_;
            keyBlockNr = keyblockNr_;
            versionIsDJB = pres.Version == ChaCha.Version.DJB;
            originalState = GetOriginalState();
            descriptions.Add("To generate a keystream block, we apply the ChaCha hash function to the state. "
                    + $"The ChaCha hash function consists of {pres.Rounds} rounds. One round applies the quarterround function four times hence the name \"quarterround\". The quarterround function takes in 4 state entries and modifies them.");
            descriptions.Add("Every odd round consists of 4 so called column rounds since all entries in a column are selected as the input to the quarterround function. ");
            descriptions.Add("Every even round consists of 4 so called diagonal rounds since all diagonal entries are selected as the input to the quarterround function. ");
            descriptions.Add("After all rounds, we add the original state (the state before we applied the ChaCha Hash function) ...");
            descriptions.Add("... and transform each 4 byte of the state into little-endian notation.");
            Init();
            Cache = GenerateCache();
        }

        private void Init()
        {
            PageAction initAction = new PageAction(() =>
            {
                // Hotfix for ClearDescription changing top margin for some reason. It is executed when hitting an cache action so we just call it here straightaway.
                ClearDescription(pres);
                AddToState(originalState);
                AddOriginalDiffusionToState();
                AssertInitialState();
                pres.KeystreamBlocksNeededTextBlock.Text = keyBlockNr.ToString();
            }, () =>
            {
                ClearState();
            });
            AddInitAction(initAction);
            PageAction generalDescriptionAction = new PageAction(() =>
            {
                AddBoldToDescription(descriptions[0]);
            }, () =>
            {
                ClearDescription();
            });
            PageAction firstColumnRoundDescriptionAction = new PageAction(() =>
            {
                UnboldLastFromDescription();
                AddBoldToDescription(descriptions[1]);
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
            });
            AddAction(generalDescriptionAction);
            AddAction(firstColumnRoundDescriptionAction);

            for (int qrIndex = 1; qrIndex <= pres.Rounds * 4; ++qrIndex)
            {
                int round = CalculateRoundFromQRIndex(qrIndex);
                AddAction(CopyFromStateTOQRInputActions(qrIndex, round));
                AddAction(QRExecActions(1, qrIndex));
                AddAction(QRExecActions(2, qrIndex));
                AddAction(QRExecActions(3, qrIndex));
                AddAction(QRExecActions(4, qrIndex));
                AddAction(QROutputActions());
                AddAction(ReplaceStateEntriesWithQROutput(qrIndex));
                if (qrIndex != pres.Rounds * 4)
                {
                    AddAction(ClearQRDetail(qrIndex));
                }
            }
            AddAction(AddOriginalState());
            AddAction(ConvertStateEntriesToLittleEndian());
        }

        private ActionCache GenerateCache()
        {
            Debug.Assert(Actions.Length != 0, $"Actions empty while generating cache. Did you initialize the actions?");
            CachePageAction GenerateQuarterroundCacheEntry(int qrIndex)
            {
                CachePageAction cache = new CachePageAction();
                cache.AddToExec(() =>
                {
                    ClearQRDetail();
                    // update state matrix
                    ClearState();
                    TextBox[] stateTextBoxes = GetStateTextBoxes(pres);
                    uint[] stateEntries;
                    if (qrIndex == 1)
                    {
                        stateEntries = pres.GetResult(ResultType.CHACHA_HASH_ORIGINAL_STATE, (int)keyBlockNr - 1);
                    }
                    else
                    {
                        stateEntries = GetMappedResult(ResultType.CHACHA_HASH_QUARTERROUND, qrIndex - 2);
                    }
                    Debug.Assert(stateTextBoxes.Length == stateEntries.Length);
                    for (int x = 0; x < stateEntries.Length; ++x)
                    {
                        stateTextBoxes[x].Text = ChaChaPresentation.HexString(stateEntries[x]);
                    }
                    // highlight corresponding state entries which will be copied into QR detail in next action
                    (int i, int j, int k, int l) = GetStateIndicesFromQRIndex(qrIndex);
                    UnmarkAllStateEntriesExcept(pres, i, j, k, l);
                    // update round indicator
                    int round = CalculateRoundFromQRIndex(qrIndex);
                    pres.CurrentRoundIndex = round;
                    pres.CurrentQuarterroundIndexTextBox = (qrIndex - 1) % 4 + 1;
                    pres.Nav.Replace(pres.CurrentRound, round.ToString());
                    ShowQRButtons(pres, round);
                    HideAllQRArrowsExcept(pres, ((qrIndex - 1) % 8) + 1);

                    ClearDescription(pres);
                    AddToDescription(pres, descriptions[0]);
                    if (round >= 2)
                    {
                        AddToDescription(pres, descriptions[1]);
                        AddBoldToDescription(pres, descriptions[2]);
                    }
                    else
                    {
                        AddBoldToDescription(pres, descriptions[1]);
                    }
                });
                return cache;
            }
            ActionCache qrCache = new ActionCache();
            for (int qrIndex = 1; qrIndex <= pres.Rounds * 4; ++qrIndex)
            {
                int actionIndex = ChaChaPresentation.GetLabeledPageActionIndex(QuarterroundStartLabelWithoutRound(qrIndex), Actions) + 1;
                qrCache.Set(GenerateQuarterroundCacheEntry(qrIndex), actionIndex);
            }
            return qrCache;
        }

        #region State methods
        private string[] GetCurrentState()
        {
            return Enumerable.Range(0, 16).Select(i => ((TextBox)GetIndexElement(pres, "UIKeystreamBlockGen", i, "")).Text).ToArray();
        }
        private string[] GetTemplateState()
        {
            return new string[] {
                pres.ConstantsLittleEndian[0], pres.ConstantsLittleEndian[1], pres.ConstantsLittleEndian[2], pres.ConstantsLittleEndian[3],
                pres.KeyLittleEndian[0], pres.KeyLittleEndian[1], pres.KeyLittleEndian[2], pres.KeyLittleEndian[3],
                pres.KeyLittleEndian[pres.InputKey.Length == 32 ? 4 : 0], pres.KeyLittleEndian[pres.InputKey.Length == 32 ? 5 : 1], pres.KeyLittleEndian[pres.InputKey.Length == 32 ? 6 : 2], pres.KeyLittleEndian[pres.InputKey.Length == 32 ? 7 : 3],
                "", "", "", "" };
        }
        private void InsertCounter(string[] state, ulong counter64)
        {
            if (versionIsDJB)
            {
                // 64-bit counter, 64-bit IV
                byte[] counter = ChaCha.GetBytes(counter64);
                // counter as little-endian
                Array.Reverse(counter);
                string counterStr = ChaChaPresentation.HexStringLittleEndian(counter);
                Debug.Assert(counterStr.Length == 16, $"Counter string is of size {counterStr.Length}. Expected: 16 (version DJB)");
                state[12] = counterStr.Substring(0, 8);
                state[13] = counterStr.Substring(8);
                // Debug.Assert(pres.IVLittleEndian.Length == 2, $"IVLittleEndian array is of size {pres.IVLittleEndian.Length}. Expected: 2 (version DJB)");
                state[14] = pres.IVLittleEndian[0];
                state[15] = pres.IVLittleEndian[1];
            }
            else
            {
                // 32-bit counter, 96-bit IV
                byte[] counter = ChaCha.GetBytes((uint)counter64);
                Array.Reverse(counter);
                string counterStr = ChaChaPresentation.HexStringLittleEndian(counter);
                Debug.Assert(counterStr.Length == 8, $"Counter string is of size {counterStr.Length}. Expected: 8 (version IETF)");
                state[12] = counterStr;
                Debug.Assert(pres.IVLittleEndian.Length == 3, $"IVLittleEndian array is of size {pres.IVLittleEndian.Length}. Expected: 3 (version IETF)");
                state[13] = pres.IVLittleEndian[0];
                state[14] = pres.IVLittleEndian[1];
                state[15] = pres.IVLittleEndian[2];
            }
        }

        private string[] GetOriginalState()
        {
            string[] state = GetTemplateState();
            InsertCounter(state, keyBlockNr - 1);
            return state;
        }
        
        private void AddToState(string[] state)
        {
            Debug.Assert(state.Length == 16, $"AddToState: given array is not of length 16");
            for (int i = 0; i < 16; ++i)
            {
                pres.Nav.Add((TextBox)GetIndexElement(pres, "UIKeystreamBlockGen", i, ""), state[i]);
            }
        }
        
        private void ShowAddition()
        {
            string[] add = originalState.Select(x => $"+{x}").ToArray();
            for (int i = 0; i < add.Length; ++i)
            {
                TextBox tb = new TextBox() { Text = add[i], Style = pres.FindResource("hexCell") as Style, Margin = new Thickness(-15, 0, 0, 0) };
                pres.Nav.Add((StackPanel)GetIndexElement(pres, "UIKeystreamBlockGenPanel", i, ""), tb);
            }
        }
        private void ClearAddition()
        {
            RemoveLastFromEachStateCellStackPanel();
        }
        private void ReplaceState(params string[] state)
        {
            for (int i = 0; i < state.Length; ++i)
            {
                pres.Nav.Replace((TextBox)GetIndexElement(pres, "UIKeystreamBlockGen", i, ""), state[i]);
            }
        }
        private void ClearState()
        {
            for (int i = 0; i < 16; ++i)
            {
                pres.Nav.Clear((TextBox)GetIndexElement(pres, "UIKeystreamBlockGen", i, ""));
                pres.Nav.ClearDocument((RichTextBox)GetIndexElement(pres, "UIKeystreamBlockGenDiffusion", i, ""));
                pres.Nav.Collapse((Border)GetIndexElement(pres, "UIKeystreamBlockGenCellDiffusion", i));
                StackPanel sp = (StackPanel)GetIndexElement(pres, "UIKeystreamBlockGenPanel", i, "");
                while (sp.Children.Count > 1)
                {
                    pres.Nav.RemoveLast(sp);
                }
            }

        }
        private void ShowAdditionResult()
        {
            string[] result = GetMappedResult(ResultType.CHACHA_HASH_ADD_ORIGINAL_STATE, (int)keyBlockNr - 1).Select(u => ChaChaPresentation.HexString(u)).ToArray();
            for (int i = 0; i < result.Length; ++i)
            {
                TextBox tb = new TextBox() { Text = result[i], Style = pres.FindResource("hexCell") as Style, TextDecorations = TextDecorations.OverLine };
                pres.Nav.Add((StackPanel)GetIndexElement(pres, "UIKeystreamBlockGenPanel", i, ""), tb);
            }
        }
        private void ClearAdditionResult()
        {
            RemoveLastFromEachStateCellStackPanel();
        }
        private void ShowStateLittleEndianTransformResult()
        {
            string[] littleEndianState = GetMappedResult(ResultType.CHACHA_HASH_LITTLEENDIAN_STATE, (int)keyBlockNr - 1).Select(s => ChaChaPresentation.HexString(s)).ToArray();
            for (int i = 0; i < littleEndianState.Length; ++i)
            {
                TextBox tb = new TextBox() { Text = littleEndianState[i], Style = pres.FindResource("hexCell") as Style, TextDecorations = TextDecorations.OverLine };
                pres.Nav.Add((StackPanel)GetIndexElement(pres, "UIKeystreamBlockGenPanel", i, ""), tb);
            }
        }
        private void ClearStateLittleEndianTransformResult()
        {
            RemoveLastFromEachStateCellStackPanel();
        }
        private void RemoveLastFromEachStateCellStackPanel()
        {
            for (int i = 0; i < 16; ++i)
            {
                pres.Nav.RemoveLast((StackPanel)GetIndexElement(pres, "UIKeystreamBlockGenPanel", i, ""));
            }
        }
        public static Border GetStateCell(ChaChaPresentation pres, int stateIndex)
        {
            return (Border)GetIndexElement(pres, "UIKeystreamBlockGenCell", stateIndex);
        }

        public static TextBox[] GetStateTextBoxes(ChaChaPresentation pres)
        {
            return Enumerable.Range(0, 16).Select(i => (TextBox)GetIndexElement(pres, "UIKeystreamBlockGen", i, "")).ToArray();
        }
        public static void UnmarkAllStateEntriesExcept(ChaChaPresentation pres, int i, int j, int k, int l)
        {
            for (int x = 0; x < 16; ++x)
            {
                pres.Nav.UnsetBackground((Border)GetIndexElement(pres, "UIKeystreamBlockGenCell", x));
            }
            pres.Nav.SetCopyBackground((Border)GetIndexElement(pres, "UIKeystreamBlockGenCell", i));
            pres.Nav.SetCopyBackground((Border)GetIndexElement(pres, "UIKeystreamBlockGenCell", j));
            pres.Nav.SetCopyBackground((Border)GetIndexElement(pres, "UIKeystreamBlockGenCell", k));
            pres.Nav.SetCopyBackground((Border)GetIndexElement(pres, "UIKeystreamBlockGenCell", l));
        }
        private void AssertInitialState()
        {
            // Check that the state entries in the state matrix visualization are the same as the actual values in the uint[] array
            string[] expectedState = GetMappedResult(ResultType.CHACHA_HASH_ORIGINAL_STATE, (int)keyBlockNr - 1).Select(s => ChaChaPresentation.HexString(s)).ToArray();
            string[] visualState = new string[16];
            for (int i = 0; i < 16; ++i)
            {
                visualState[i] = ((TextBox)GetIndexElement(pres, "UIKeystreamBlockGen", i, "")).Text;
                Debug.Assert(expectedState[i] == visualState[i], $"Visual init state does not match actual init state! expected {expectedState[i]} at index {i}, but got {visualState[i]}");
            }
        }
        private PageAction[] AddOriginalState()
        {
            PageAction updateDescription = new PageAction(() =>
            {
                UnboldLastFromDescription();
                AddBoldToDescription(descriptions[3]);
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
            });
            PageAction showOriginalState = new PageAction(() =>
            {
                ShowAddition();
                ShowAdditionResult();
            }, () =>
            {
                ClearAddition();
                ClearAdditionResult();
            });
            string[] previousState = GetMappedResult(ResultType.CHACHA_HASH_QUARTERROUND, pres.Rounds * 4 - 1).Select(s => ChaChaPresentation.HexString(s)).ToArray();
            PageAction addStates = new PageAction(() =>
            {
                ClearAddition();
                ClearAdditionResult();
                ReplaceState(GetMappedResult(ResultType.CHACHA_HASH_ADD_ORIGINAL_STATE, (int)keyBlockNr - 1).Select(u => ChaChaPresentation.HexString(u)).ToArray());
            }, () =>
            {
                ShowAddition();
                ShowAdditionResult();
                ReplaceState(previousState);
            });
            return new PageAction[] { updateDescription, showOriginalState, addStates };
        }
        private PageAction[] ConvertStateEntriesToLittleEndian()
        {
            PageAction updateDescription = new PageAction(() =>
            {
                UnboldLastFromDescription();
                AddBoldToDescription(descriptions[4]);
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
            });
            PageAction showResult = new PageAction(() =>
            {
                ShowStateLittleEndianTransformResult();
            }, () =>
            {
                ClearStateLittleEndianTransformResult();
            });
            uint[] state = GetMappedResult(ResultType.CHACHA_HASH_ADD_ORIGINAL_STATE, (int)keyBlockNr - 1);
            string[] previousState = state.Select(u => ChaChaPresentation.HexString(u)).ToArray();
            string[] littleEndianState = GetMappedResult(ResultType.CHACHA_HASH_LITTLEENDIAN_STATE, (int)keyBlockNr - 1).Select(s => ChaChaPresentation.HexString(s)).ToArray();
            PageAction convert = new PageAction(() =>
            {
                ClearStateLittleEndianTransformResult();
                ReplaceState(littleEndianState);
            }, () =>
            {
                ReplaceState(previousState);
                ShowStateLittleEndianTransformResult();
            });
            return new PageAction[] { updateDescription, showResult, convert };
        }
        #endregion

        #region Diffusion methods
        private string[] GetTemplateDiffusionState()
        {
            byte[] dkey = pres.DiffusionKey;
            byte[] key = pres.InputKey;
            string[] dkeyLe = pres.DiffusionKeyLittleEndian;
            string[] keyLe = pres.KeyLittleEndian;
            Debug.Assert(dkey.Length == key.Length, $"DiffusionKey ({dkey.Length}) has not same length as Input Key ({key.Length})");
            Debug.Assert(dkeyLe.Length == keyLe.Length, $"DiffusionKeyLittleEndian ({dkeyLe.Length}) has not same length as KeyLittleEndian ({keyLe.Length})");
            int dkeyLength = dkey.Length;
            string[] templateState = GetTemplateState();
            templateState[4] = dkeyLe[0];
            templateState[5] = dkeyLe[1];
            templateState[6] = dkeyLe[2];
            templateState[7] = dkeyLe[3];
            templateState[8] = dkeyLe[dkeyLength == 32 ? 4 : 0];
            templateState[9] = dkeyLe[dkeyLength == 32 ? 5 : 1];
            templateState[10] = dkeyLe[dkeyLength == 32 ? 6 : 2];
            templateState[11] = dkeyLe[dkeyLength == 32 ? 7 : 3];
            return templateState;
        }
        private string[] GetDiffusionOriginalState()
        {
            string[] dKeyOriginalState = GetTemplateDiffusionState();
            InsertCounter(dKeyOriginalState, keyBlockNr - 1);
            return dKeyOriginalState;
        }
        private void AddOriginalDiffusionToState()
        {
            string[] diffusionOriginalState = GetDiffusionOriginalState();
            AddDiffusionToState(diffusionOriginalState);
        }
        private void AddDiffusionToState(params string[] diffusionState)
        {
            if (!pres.DiffusionActive) return;
            Debug.Assert(diffusionState.Length == 16, $"AddDiffusionToState: given array is not of length 16");
            string[] currentState = GetCurrentState();
            for (int i = 0; i < 16; ++i)
            {
                RichTextBox diffusionValue = (RichTextBox)GetIndexElement(pres, "UIKeystreamBlockGenDiffusion", i, "");
                pres.Nav.SetDocument(diffusionValue, MarkDifferenceRed(diffusionState[i], currentState[i]));
                pres.Nav.Show((Border)GetIndexElement(pres, "UIKeystreamBlockGenCellDiffusion", i));
            }
        }
        private FlowDocument MarkDifferenceRed(string diffusionValue, string normalValue)
        {
            FlowDocument fd = new FlowDocument() { TextAlignment = TextAlignment.Center };
            Paragraph p = new Paragraph();
            Debug.Assert(diffusionValue.Length == normalValue.Length, $"MarkDifferenceRed: string lengths do not match. Got {diffusionValue.Length} and {normalValue.Length}");
            for(int i = 0; i < diffusionValue.Length; ++i)
            {
                char d = diffusionValue[i];
                char v = normalValue[i];
                p.Inlines.Add(RedIfDifferent(d, v));
            }
            fd.Blocks.Add(p);
            return fd;
        }
        private TextBlock RedIfDifferent(char d, char v)
        {
            return new TextBlock() { Text = d.ToString(), Foreground = d != v ? Brushes.Red : Brushes.Black };
        }
        #endregion

        #region QR Visualization
        public static void ShowQRButtons(ChaChaPresentation pres, int round)
        {
            for (int i = 1; i <= 8; ++i)
            {
                StackPanel qrButton = (StackPanel)GetIndexElement(pres, "QRound_Button", i);
                qrButton.Visibility = ((round % 2 == 1 || round == 0) && i <= 4) || (round % 2 == 0 && round != 0 && i > 4) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public static void HideAllQRArrowsExcept(ChaChaPresentation pres, int arrowIndex)
        {
            ((TextBox)GetIndexElement(pres, "ArrowQRRound", 1)).Visibility = Visibility.Hidden;
            ((TextBox)GetIndexElement(pres, "ArrowQRRound", 2)).Visibility = Visibility.Hidden;
            ((TextBox)GetIndexElement(pres, "ArrowQRRound", 3)).Visibility = Visibility.Hidden;
            ((TextBox)GetIndexElement(pres, "ArrowQRRound", 4)).Visibility = Visibility.Hidden;
            ((TextBox)GetIndexElement(pres, "ArrowQRRound", 5)).Visibility = Visibility.Hidden;
            ((TextBox)GetIndexElement(pres, "ArrowQRRound", 6)).Visibility = Visibility.Hidden;
            ((TextBox)GetIndexElement(pres, "ArrowQRRound", 7)).Visibility = Visibility.Hidden;
            ((TextBox)GetIndexElement(pres, "ArrowQRRound", 8)).Visibility = Visibility.Hidden;
            ((TextBox)GetIndexElement(pres, "ArrowQRRound", arrowIndex)).Visibility = Visibility.Visible;
        }
        public void ClearQRDetail()
        {
            pres.Nav.Clear(pres.QRInACell, pres.QRInBCell, pres.QRInCCell, pres.QRInDCell);
            pres.Nav.Clear(pres.QROutACell, pres.QROutBCell, pres.QROutCCell, pres.QROutDCell);
            pres.Nav.Clear(pres.QROutputPath_A, pres.QROutputPath_B, pres.QROutputPath_C, pres.QROutputPath_D);
            for (int i = 1; i <= 4; ++i)
            {
                pres.Nav.Clear((Shape)GetIndexElement(pres, "QRAddPath", i));
                pres.Nav.Clear((Border)GetIndexElement(pres, "QRAddCell", i));
                pres.Nav.Clear((Shape)GetIndexElement(pres, "QRAddCircle", i));

                pres.Nav.Clear((Shape)GetIndexElement(pres, "QRXORPath", i));
                pres.Nav.Clear((Border)GetIndexElement(pres, "QRXORCell", i));
                pres.Nav.Clear((Shape)GetIndexElement(pres, "QRXORCircle", i));

                pres.Nav.Clear((Shape)GetIndexElement(pres, "QRShiftPath", i));
                pres.Nav.Clear((Border)GetIndexElement(pres, "QRShiftCell", i));
                pres.Nav.Clear((Shape)GetIndexElement(pres, "QRShiftCircle", i));
            }
        }
        public PageAction ClearQRDetail(int qrIndex)
        {
            string qrInA = pres.GetHexResult(ResultType.QR_INPUT_A, qrIndex - 1);
            string qrInB = pres.GetHexResult(ResultType.QR_INPUT_B, qrIndex - 1);
            string qrInC = pres.GetHexResult(ResultType.QR_INPUT_C, qrIndex - 1);
            string qrInD = pres.GetHexResult(ResultType.QR_INPUT_D, qrIndex - 1);
            string[,] qrDetailValues = new string[4, 3];
            for (int i = 0; i < 4; ++i)
            {
                qrDetailValues[i, 0] = pres.GetHexResult(ResultType.QR_ADD, i + (qrIndex - 1) * 4);
                qrDetailValues[i, 1] = pres.GetHexResult(ResultType.QR_XOR, i + (qrIndex - 1) * 4);
                qrDetailValues[i, 2] = pres.GetHexResult(ResultType.QR_SHIFT, i + (qrIndex - 1) * 4);
            }
            string qrOutA = pres.GetHexResult(ResultType.QR_OUTPUT_A, qrIndex - 1);
            string qrOutB = pres.GetHexResult(ResultType.QR_OUTPUT_B, qrIndex - 1);
            string qrOutC = pres.GetHexResult(ResultType.QR_OUTPUT_C, qrIndex - 1);
            string qrOutD = pres.GetHexResult(ResultType.QR_OUTPUT_D, qrIndex - 1);

            return new PageAction(() =>
            {
                ClearQRDetail();
            }, () =>
            {
                pres.Nav.Replace(pres.QRInA, qrInA);
                pres.Nav.Replace(pres.QRInB, qrInB);
                pres.Nav.Replace(pres.QRInC, qrInC);
                pres.Nav.Replace(pres.QRInD, qrInD);
                for (int i = 1; i <= 4; ++i)
                {
                    pres.Nav.Replace((TextBox)GetIndexElement(pres, "QRAdd", i), qrDetailValues[i - 1, 0]);
                    pres.Nav.Replace((TextBox)GetIndexElement(pres, "QRXOR", i), qrDetailValues[i - 1, 1]);
                    pres.Nav.Replace((TextBox)GetIndexElement(pres, "QRShift", i), qrDetailValues[i - 1, 2]);
                }
                pres.Nav.Replace(pres.QROutA, qrOutA);
                pres.Nav.Replace(pres.QROutB, qrOutB);
                pres.Nav.Replace(pres.QROutC, qrOutC);
                pres.Nav.Replace(pres.QROutD, qrOutD);
            });
        }
        private PageAction[] QROutputActions()
        {
            return pres.Nav.CopyActions(
                new Border[] { (Border)GetIndexElement(pres, "QRAddCell", 3), (Border)GetIndexElement(pres, "QRShiftCell", 4), (Border)GetIndexElement(pres, "QRAddCell", 4), (Border)GetIndexElement(pres, "QRShiftCell", 3) },
                new Shape[] { pres.QROutputPath_A, pres.QROutputPath_B, pres.QROutputPath_C, pres.QROutputPath_D },
                new Border[] { pres.QROutACell, pres.QROutBCell, pres.QROutCCell, pres.QROutDCell },
                new string[] { "", "", "", "" }
                );
        }
        // TODO: Following three functions are very similar and can be refactored into one function
        private PageAction[] ExecAddActions(int actionIndex, int qrIndex)
        {
            (Border, Border) GetInputs(int actionIndex_)
            {
                switch (actionIndex_)
                {
                    case 1:
                        return (pres.QRInACell, pres.QRInBCell);
                    case 2:
                        return (pres.QRInCCell, pres.QRShiftCell_1);
                    case 3:
                        return (pres.QRAddCell_1, pres.QRShiftCell_2);
                    case 4:
                        return (pres.QRAddCell_2, pres.QRShiftCell_3);
                    default:
                        Debug.Assert(false, $"No add inputs found for actionIndex {actionIndex_}");
                        return (null, null);

                }
            }
            Border input1, input2;
            (input1, input2) = GetInputs(actionIndex);
            Border addCell = (Border)GetIndexElement(pres, "QRAddCell", actionIndex);
            Shape addPath = (Shape)GetIndexElement(pres, "QRAddPath", actionIndex);
            Shape addCircle = (Shape)GetIndexElement(pres, "QRAddCircle", actionIndex);
            TextBox addBox = (TextBox)GetIndexElement(pres, "QRAdd", actionIndex);
            void Mark()
            {
                pres.Nav.MarkShape(addPath, addCircle);
                pres.Nav.SetCopyBackground(input1, input2);
            }
            void Unmark()
            {
                pres.Nav.UnmarkShape(addPath, addCircle);
                pres.Nav.UnsetBackground(input1, input2);
            }
            PageAction mark = new PageAction(Mark, Unmark);
            PageAction exec = new PageAction(() =>
            {
                pres.Nav.SetCopyBackground(addCell);
                pres.Nav.Replace(addBox, GetMappedHexResult(ResultType.QR_ADD, ResultIndex(actionIndex, qrIndex)));
            }, () =>
            {
                pres.Nav.UnsetBackground(addCell);
                pres.Nav.Clear(addBox);
            });
            PageAction unmark = new PageAction(Unmark, Mark);
            unmark.AddToExec(() =>
            {
                pres.Nav.UnsetBackground(addCell);
            });
            unmark.AddToUndo(() =>
            {
                pres.Nav.SetCopyBackground(addCell);
            });
            return new PageAction[] { mark, exec, unmark };
        }
        private PageAction[] ExecXORActions(int actionIndex, int qrIndex)
        {
            (Border, Border) GetInputs(int actionIndex_)
            {
                switch (actionIndex_)
                {
                    case 1:
                        return (pres.QRAddCell_1, pres.QRInDCell);
                    case 2:
                        return (pres.QRInBCell, pres.QRAddCell_2);
                    case 3:
                        return (pres.QRAddCell_3, pres.QRShiftCell_1);
                    case 4:
                        return (pres.QRShiftCell_2, pres.QRAddCell_4);
                    default:
                        Debug.Assert(false, $"No add inputs found for actionIndex {actionIndex_}");
                        return (null, null);

                }
            }
            Border input1, input2;
            (input1, input2) = GetInputs(actionIndex);
            Border xorCell = (Border)GetIndexElement(pres, "QRXORCell", actionIndex);
            Shape xorPath = (Shape)GetIndexElement(pres, "QRXORPath", actionIndex);
            Shape xorCircle = (Shape)GetIndexElement(pres, "QRXORCircle", actionIndex);
            TextBox xorBox = (TextBox)GetIndexElement(pres, "QRXOR", actionIndex);
            void Mark()
            {
                pres.Nav.MarkShape(xorPath, xorCircle);
                pres.Nav.SetCopyBackground(input1, input2);
            }
            void Unmark()
            {
                pres.Nav.UnmarkShape(xorPath, xorCircle);
                pres.Nav.UnsetBackground(input1, input2);
            }
            PageAction mark = new PageAction(Mark, Unmark);
            PageAction exec = new PageAction(() =>
            {
                pres.Nav.SetCopyBackground(xorCell);
                pres.Nav.Replace(xorBox, GetMappedHexResult(ResultType.QR_XOR, ResultIndex(actionIndex, qrIndex)));
            }, () =>
            {
                pres.Nav.UnsetBackground(xorCell);
                pres.Nav.Clear(xorBox);
            });
            PageAction unmark = new PageAction(Unmark, Mark);
            unmark.AddToExec(() =>
            {
                pres.Nav.UnsetBackground(xorCell);
            });
            unmark.AddToUndo(() =>
            {
                pres.Nav.SetCopyBackground(xorCell);
            });
            return new PageAction[] { mark, exec, unmark };
        }
        private PageAction[] ExecShiftActions(int actionIndex, int qrIndex)
        {
            Border input = (Border)GetIndexElement(pres, "QRXORCell", actionIndex);
            Border shiftCell = (Border)GetIndexElement(pres, "QRShiftCell", actionIndex);
            Shape shiftPath = (Shape)GetIndexElement(pres, "QRShiftPath", actionIndex);
            Shape shiftCircle = (Shape)GetIndexElement(pres, "QRShiftCircle", actionIndex);
            TextBox shiftBox = (TextBox)GetIndexElement(pres, "QRShift", actionIndex);
            void Mark()
            {
                pres.Nav.MarkShape(shiftPath, shiftCircle);
                pres.Nav.SetCopyBackground(input);
            }
            void Unmark()
            {
                pres.Nav.UnmarkShape(shiftPath, shiftCircle);
                pres.Nav.UnsetBackground(input);
            }
            PageAction mark = new PageAction(Mark, Unmark);
            PageAction exec = new PageAction(() =>
            {
                pres.Nav.SetCopyBackground(shiftCell);
                pres.Nav.Replace(shiftBox, GetMappedHexResult(ResultType.QR_SHIFT, ResultIndex(actionIndex, qrIndex)));
            }, () =>
            {
                pres.Nav.UnsetBackground(shiftCell);
                pres.Nav.Clear(shiftBox);
            });
            PageAction unmark = new PageAction(Unmark, Mark);
            unmark.AddToExec(() =>
            {
                pres.Nav.UnsetBackground(shiftCell);
            });
            unmark.AddToUndo(() =>
            {
                pres.Nav.SetCopyBackground(shiftCell);
            });
            return new PageAction[] { mark, exec, unmark };
        }
        private void AssertQRExecActions(ChaChaPresentation pres, int actionIndex, int qrIndex)
        {
            int resultIndex = ResultIndex(actionIndex, qrIndex);
            string add = ((TextBox)GetIndexElement(pres, "QRAdd", actionIndex)).Text;
            string expectedAdd = GetMappedHexResult(ResultType.QR_ADD, resultIndex);
            string xor = ((TextBox)GetIndexElement(pres, "QRXOR", actionIndex)).Text;
            string expectedXor = GetMappedHexResult(ResultType.QR_XOR, resultIndex);
            string shift = ((TextBox)GetIndexElement(pres, "QRShift", actionIndex)).Text;
            string expectedShift = GetMappedHexResult(ResultType.QR_SHIFT, resultIndex);
            Debug.Assert(add == expectedAdd, $"Visual value after ADD execution does not match actual value! expected {expectedAdd}, got {add}");
            Debug.Assert(xor == expectedXor, $"Visual value after XOR execution does not match actual value! expected {expectedXor}, got {xor}");
            Debug.Assert(shift == expectedShift, $"Visual value after SHIFT execution does not match actual value! expected {expectedShift}, got {shift}");
        }
        private PageAction[] QRExecActions(int actionIndex, int qrIndex)
        {
            List<PageAction> actions = new List<PageAction>();
            PageAction[] execAdd = ExecAddActions(actionIndex, qrIndex);
            PageAction[] execXOR = ExecXORActions(actionIndex, qrIndex);
            PageAction[] execShift = ExecShiftActions(actionIndex, qrIndex);
            execShift[2].AddToExec(() =>
            {
                AssertQRExecActions(pres, actionIndex, qrIndex);
            });
            actions.AddRange(execAdd);
            actions.AddRange(execXOR);
            actions.AddRange(execShift);
            return actions.ToArray();
        }
        private static (int, int, int, int) GetStateIndicesFromQRIndex(int qrIndex)
        {
            switch (((qrIndex - 1) % 8) + 1)
            {
                case 1:
                    return (0, 4, 8, 12);
                case 2:
                    return (1, 5, 9, 13);
                case 3:
                    return (2, 6, 10, 14);
                case 4:
                    return (3, 7, 11, 15);
                case 5:
                    return (0, 5, 10, 15);
                case 6:
                    return (1, 6, 11, 12);
                case 7:
                    return (2, 7, 8, 13);
                case 8:
                    return (3, 4, 9, 14);
                default:
                    Debug.Assert(false, $"No state indices found for qrIndex {qrIndex}");
                    return (-1, -1, -1, -1);
            }
        }
        private static int CalculateRoundFromQRIndex(int qrIndex)
        {
            /**
            * Returns one-based round value (between 1 - 20) for one-based indices quarterround index (between 1 - 8).
            * 
            * Examples: 
            *   Quarterround index 1 -> Round 1
            *   Quarterround index 4 -> Round 1
            *   Quarterround index 5 -> Round 2
            *   Quarterround index 8 -> Round 2
            *   Quarterround index 9 -> Round 3
            *   Quarterround index 13 -> Round 4
            */
            return (int)Math.Floor((double)(qrIndex - 1) / 4) + 1;
        }
        private PageAction[] CopyFromStateTOQRInputActions(int qrIndex, int round)
        {
            (int i, int j, int k, int l) = GetStateIndicesFromQRIndex(qrIndex);
            Border[] stateCells = new Border[] { GetStateCell(pres, i), GetStateCell(pres, j), GetStateCell(pres, k), GetStateCell(pres, l) };
            PageAction[] copyActions = pres.Nav.CopyActions(stateCells,
                new Border[] { pres.QRInACell, pres.QRInBCell, pres.QRInCCell, pres.QRInDCell },
                new string[] { "", "", "", "" }
                );
            if ((qrIndex - 1) % 4 == 0)
            {
                // new round begins
                PageAction roundUpdate = new PageAction(() =>
                {
                    pres.CurrentRoundIndex = round;
                    pres.Nav.Replace(pres.CurrentRound, round.ToString());
                    ShowQRButtons(pres, round);
                }, () =>
                {
                    string text = "";
                    if (round > 1)
                    {
                        text = (round - 1).ToString();
                    }
                    else if (round == 1)
                    {
                        text = "-";
                    }
                    pres.CurrentRoundIndex = round - 1;
                    pres.Nav.Replace(pres.CurrentRound, text);
                    ShowQRButtons(pres, round - 1);
                });
                copyActions[0].Add(roundUpdate);
                copyActions[0].AddLabel(RoundStartLabel(round));
                if (round == 2)
                {
                    PageAction updateDescription = new PageAction(() =>
                    {
                        AddBoldToDescription(descriptions[2]);
                    }, () =>
                    {
                        RemoveLastFromDescription();
                        MakeLastBoldInDescription();
                    });
                    copyActions[0].Add(updateDescription);
                }
            }
            int QR_ARROW_MAX_INDEX = 8;
            (int, int) calculateQRArrowIndex(int qrIndex_)
            {
                int qrArrowIndex = ((qrIndex_ - 1) % QR_ARROW_MAX_INDEX) + 1;
                int qrPrevArrowIndex = qrArrowIndex == 1 ? QR_ARROW_MAX_INDEX : qrArrowIndex - 1;
                return (qrArrowIndex, qrPrevArrowIndex);
            }
            PageAction updateQRArrow = new PageAction(() =>
            {
                pres.CurrentQuarterroundIndexTextBox = (qrIndex - 1) % 4 + 1;
                (int qrArrowIndex, int qrPrevArrowIndex) = calculateQRArrowIndex(qrIndex);
                ((TextBox)GetIndexElement(pres, "ArrowQRRound", qrPrevArrowIndex)).Visibility = Visibility.Hidden;
                ((TextBox)GetIndexElement(pres, "ArrowQRRound", qrArrowIndex)).Visibility = Visibility.Visible;
            }, () =>
            {
                pres.CurrentQuarterroundIndexTextBox = (qrIndex - 2) % 4 + 1;
                if (qrIndex > 1)
                {
                    (int qrArrowIndex, int qrPrevArrowIndex) = calculateQRArrowIndex(qrIndex);
                    ((TextBox)GetIndexElement(pres, "ArrowQRRound", qrPrevArrowIndex)).Visibility = Visibility.Visible;
                    ((TextBox)GetIndexElement(pres, "ArrowQRRound", qrArrowIndex)).Visibility = Visibility.Hidden;
                }
            });
            copyActions[0].Add(updateQRArrow);
            copyActions[0].AddLabel(QuarterroundStartLabelWithRound(qrIndex, round));
            copyActions[0].AddLabel(QuarterroundStartLabelWithoutRound(qrIndex));
            return copyActions;
        }
        private void AssertStateAfterQuarterround(int qrIndex)
        {
            // Check that the state entries in the state matrix visualization are the same as the actual values in the uint[] array
            string[] expectedState = GetMappedResult(ResultType.CHACHA_HASH_QUARTERROUND, qrIndex - 1).Select(s => ChaChaPresentation.HexString(s)).ToArray();
            string[] visualState = new string[16];
            for (int i = 0; i < 16; ++i)
            {
                visualState[i] = ((TextBox)GetIndexElement(pres, "UIKeystreamBlockGen", i, "")).Text;
                Debug.Assert(expectedState[i] == visualState[i], $"Visual state after quarterround {qrIndex} execution does not match actual state! expected {expectedState[i]} at index {i}, but got {visualState[i]}");
            }
        }
        private PageAction[] ReplaceStateEntriesWithQROutput(int qrIndex)
        {
            (int i, int j, int k, int l) = GetStateIndicesFromQRIndex(qrIndex);
            Border[] stateCells = new Border[] { GetStateCell(pres, i), GetStateCell(pres, j), GetStateCell(pres, k), GetStateCell(pres, l) };
            string[] previousStateEntries = new string[4];
            previousStateEntries[0] = GetMappedHexResult(ResultType.QR_INPUT_A, qrIndex - 1);
            previousStateEntries[1] = GetMappedHexResult(ResultType.QR_INPUT_B, qrIndex - 1);
            previousStateEntries[2] = GetMappedHexResult(ResultType.QR_INPUT_C, qrIndex - 1);
            previousStateEntries[3] = GetMappedHexResult(ResultType.QR_INPUT_D, qrIndex - 1);
            PageAction[] copyActions = pres.Nav.CopyActions(new Border[] { pres.QROutACell, pres.QROutBCell, pres.QROutCCell, pres.QROutDCell }, stateCells, previousStateEntries, true);
            copyActions[1].AddLabel(QuarterroundEndLabelWithRound(qrIndex, CalculateRoundFromQRIndex(qrIndex)));
            copyActions[2].AddToExec(() =>
            {
                AssertStateAfterQuarterround(qrIndex);
            });
            return copyActions;
        }
        #endregion

        #region Intermediate Result
        private int MapIndex(ResultType<uint[]> resultType, int i)
        {
            int offset = 0;
            switch (resultType.Name)
            {
                case "CHACHA_HASH_ORIGINAL_STATE":
                case "CHACHA_HASH_ADD_ORIGINAL_STATE":
                case "CHACHA_HASH_LITTLEENDIAN_STATE":
                    // only executed once per keystream block generation thus no offset.
                    offset = 0;
                    break;
                case "CHACHA_HASH_QUARTERROUND":
                    // executed once per quarterround and each round has four quarterrounds thus offset is ROUNDS * 4
                    offset = pres.Rounds * 4;
                    break;
            }
            return (int)((ulong)offset * (keyBlockNr - 1)) + i;
        }
        private int MapIndex(ResultType<uint> resultType, int i)
        {
            int offset = 0;
            switch (resultType.Name)
            {
                case "QR_INPUT_A":
                case "QR_INPUT_B":
                case "QR_INPUT_C":
                case "QR_INPUT_D":
                case "QR_OUTPUT_A":
                case "QR_OUTPUT_B":
                case "QR_OUTPUT_C":
                case "QR_OUTPUT_D":
                    // executed once per quarterround and each round has four quarterrounds thus offset is ROUNDS * 4
                    offset = pres.Rounds * 4;
                    break;
                case "QR_ADD":
                case "QR_XOR":
                case "QR_SHIFT":
                    // executed four times per quarterround and each round has four quarterrounds thus offset is ROUNDS * 4 * 4
                    offset = pres.Rounds * 4 * 4;
                    break;
            }
            return (int)((ulong)offset * (keyBlockNr - 1)) + i;
        }
        private uint[] GetMappedResult(ResultType<uint[]> resultType, int index)
        {
            int mapIndex = MapIndex(resultType, index);
            return pres.GetResult(resultType, mapIndex);
        }
        private string GetMappedHexResult(ResultType<uint[]> resultType, int i, int j)
        {
            return pres.GetHexResult(resultType, MapIndex(resultType, i), j);
        }
        private string GetMappedHexResult(ResultType<uint> resultType, int index)
        {
            int mapIndex = MapIndex(resultType, index);
            return pres.GetHexResult(resultType, MapIndex(resultType, index));
        }
        private static int ResultIndex(int actionIndex, int qrIndex)
        {
            return (qrIndex - 1) * 4 + (actionIndex - 1);
        }
        #endregion

        #region Low Level API
        private static void AddBoldToDescription(ChaChaPresentation _pres, string descToAdd)
        {
            _pres.Nav.AddBold(_pres.UIKeystreamBlockGenStepDescription, descToAdd);
        }
        private void AddBoldToDescription(string descToAdd)
        {
            AddBoldToDescription(pres, descToAdd);
        }
        private static void AddToDescription(ChaChaPresentation _pres, string descToAdd)
        {
            _pres.Nav.Add(_pres.UIKeystreamBlockGenStepDescription, descToAdd);
        }
        private static void ClearDescription(ChaChaPresentation _pres)
        {
            _pres.Nav.Clear(_pres.UIKeystreamBlockGenStepDescription);
        }
        private void ClearDescription()
        {
            ClearDescription(pres);
        }
        private void UnboldLastFromDescription()
        {

            pres.Nav.UnboldLast(pres.UIKeystreamBlockGenStepDescription);
        }
        void MakeLastBoldInDescription()
        {
            pres.Nav.MakeBoldLast(pres.UIKeystreamBlockGenStepDescription);
        }
        void RemoveLastFromDescription()
        {
            pres.Nav.RemoveLast(pres.UIKeystreamBlockGenStepDescription);
        }
        public static object GetIndexElement(ChaChaPresentation pres, string nameId, int index, string delimiter = "_")
        {
            return pres.FindName(string.Format("{0}{1}{2}", nameId, delimiter, index));
        }
        #endregion

        #region Labels
        public static string QuarterroundStartLabelWithoutRound(int qrIndex)
        {
            // Creates a label where the quarterround indicator is continuous.
            // For example, the label for the beginning of the third quarterround in the second round: *PREFIX*_7
            // ( 7 because 1,2,3,4 (first round) 5,6,7 (second round) )
            return $"{ACTIONLABEL_QR_START}_{qrIndex}";
        }
        public static string QuarterroundStartLabelWithRound(int qrIndex, int round)
        {
            // Creates a label with quarterround indicator between 1-4.
            // For example, the label for the beginning of third quarterround in the secound round: *PREFIX*_3_2
            return $"{ACTIONLABEL_QR_START}_{((qrIndex - 1) % 4) + 1}_{round}";
        }
        public static string QuarterroundEndLabelWithRound(int qrIndex, int round)
        {
            // Creates a label with quarterround indicator between 1-4.
            // For example, the label for the end of third quarterround in the secound round: *PREFIX*_3_2
            return $"{ACTIONLABEL_QR_END}_{((qrIndex - 1) % 4) + 1}_{round}";
        }
        public static string RoundStartLabel(int round)
        {
            return $"{ACTIONLABEL_ROUND_START}_{round}";
        }
        #endregion

    }

    partial class Page
    {
        public static KeystreamBlockGenPage KeystreamBlockGenPage(ChaChaPresentation pres, ulong keyBlockNr)
        {
            // using static function as factory to hide the name assigned to the KeystreamBlockGenPage ContentControl element in the XAML code
            return new KeystreamBlockGenPage(pres.UIKeystreamBlockGenPage, pres, keyBlockNr);
        }
    }
}