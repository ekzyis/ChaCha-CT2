using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using System;

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
            if(versionIsDJB)
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

        private void Init()
        {
            PageAction initAction = new PageAction(() =>
            {
                AddToState(originalState);
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
                AddAction(ClearQRDetail(qrIndex));
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
        private void AddToState(string[] state)
        {
            AddToState(state[0], state[1], state[2], state[3], state[4], state[5], state[6], state[7], state[8], state[9], state[10], state[11], state[12], state[13], state[14], state[15]);
        }
        private void AddToState(
                string state0, string state1, string state2, string state3,
                string state4, string state5, string state6, string state7,
                string state8, string state9, string state10, string state11,
                string state12, string state13, string state14, string state15)
        {
            pres.Nav.Add(pres.UIKeystreamBlockGen0, state0);
            pres.Nav.Add(pres.UIKeystreamBlockGen1, state1);
            pres.Nav.Add(pres.UIKeystreamBlockGen2, state2);
            pres.Nav.Add(pres.UIKeystreamBlockGen3, state3);
            pres.Nav.Add(pres.UIKeystreamBlockGen4, state4);
            pres.Nav.Add(pres.UIKeystreamBlockGen5, state5);
            pres.Nav.Add(pres.UIKeystreamBlockGen6, state6);
            pres.Nav.Add(pres.UIKeystreamBlockGen7, state7);
            pres.Nav.Add(pres.UIKeystreamBlockGen8, state8);
            pres.Nav.Add(pres.UIKeystreamBlockGen9, state9);
            pres.Nav.Add(pres.UIKeystreamBlockGen10, state10);
            pres.Nav.Add(pres.UIKeystreamBlockGen11, state11);
            pres.Nav.Add(pres.UIKeystreamBlockGen12, state12);
            pres.Nav.Add(pres.UIKeystreamBlockGen13, state13);
            pres.Nav.Add(pres.UIKeystreamBlockGen14, state14);
            pres.Nav.Add(pres.UIKeystreamBlockGen15, state15);
        }
        private void ReplaceStateSecondaryRow(string[] state)
        {
            ReplaceStateSecondaryRow(state[0], state[1], state[2], state[3], state[4], state[5], state[6], state[7], state[8], state[9], state[10], state[11], state[12], state[13], state[14], state[15]);
        }
        private void ReplaceStateSecondaryRow(string state0, string state1, string state2, string state3,
                string state4, string state5, string state6, string state7,
                string state8, string state9, string state10, string state11,
                string state12, string state13, string state14, string state15)
        {
            pres.Nav.Add(pres.UIKeystreamBlockGen0_1, state0);
            pres.Nav.Add(pres.UIKeystreamBlockGen1_1, state1);
            pres.Nav.Add(pres.UIKeystreamBlockGen2_1, state2);
            pres.Nav.Add(pres.UIKeystreamBlockGen3_1, state3);
            pres.Nav.Add(pres.UIKeystreamBlockGen4_1, state4);
            pres.Nav.Add(pres.UIKeystreamBlockGen5_1, state5);
            pres.Nav.Add(pres.UIKeystreamBlockGen6_1, state6);
            pres.Nav.Add(pres.UIKeystreamBlockGen7_1, state7);
            pres.Nav.Add(pres.UIKeystreamBlockGen8_1, state8);
            pres.Nav.Add(pres.UIKeystreamBlockGen9_1, state9);
            pres.Nav.Add(pres.UIKeystreamBlockGen10_1, state10);
            pres.Nav.Add(pres.UIKeystreamBlockGen11_1, state11);
            pres.Nav.Add(pres.UIKeystreamBlockGen12_1, state12);
            pres.Nav.Add(pres.UIKeystreamBlockGen13_1, state13);
            pres.Nav.Add(pres.UIKeystreamBlockGen14_1, state14);
            pres.Nav.Add(pres.UIKeystreamBlockGen15_1, state15);
        }
        private void ReplaceState(string[] state)
        {
            ReplaceState(state[0], state[1], state[2], state[3], state[4], state[5], state[6], state[7], state[8], state[9], state[10], state[11], state[12], state[13], state[14], state[15]);
        }
        private void ReplaceState(
           string state0, string state1, string state2, string state3,
           string state4, string state5, string state6, string state7,
           string state8, string state9, string state10, string state11,
           string state12, string state13, string state14, string state15)
        {
            pres.Nav.Replace(pres.UIKeystreamBlockGen0, state0);
            pres.Nav.Replace(pres.UIKeystreamBlockGen1, state1);
            pres.Nav.Replace(pres.UIKeystreamBlockGen2, state2);
            pres.Nav.Replace(pres.UIKeystreamBlockGen3, state3);
            pres.Nav.Replace(pres.UIKeystreamBlockGen4, state4);
            pres.Nav.Replace(pres.UIKeystreamBlockGen5, state5);
            pres.Nav.Replace(pres.UIKeystreamBlockGen6, state6);
            pres.Nav.Replace(pres.UIKeystreamBlockGen7, state7);
            pres.Nav.Replace(pres.UIKeystreamBlockGen8, state8);
            pres.Nav.Replace(pres.UIKeystreamBlockGen9, state9);
            pres.Nav.Replace(pres.UIKeystreamBlockGen10, state10);
            pres.Nav.Replace(pres.UIKeystreamBlockGen11, state11);
            pres.Nav.Replace(pres.UIKeystreamBlockGen12, state12);
            pres.Nav.Replace(pres.UIKeystreamBlockGen13, state13);
            pres.Nav.Replace(pres.UIKeystreamBlockGen14, state14);
            pres.Nav.Replace(pres.UIKeystreamBlockGen15, state15);
        }
        private void ClearState()
        {
            pres.Nav.Clear(pres.UIKeystreamBlockGen0);
            pres.Nav.Clear(pres.UIKeystreamBlockGen1);
            pres.Nav.Clear(pres.UIKeystreamBlockGen2);
            pres.Nav.Clear(pres.UIKeystreamBlockGen3);
            pres.Nav.Clear(pres.UIKeystreamBlockGen4);
            pres.Nav.Clear(pres.UIKeystreamBlockGen5);
            pres.Nav.Clear(pres.UIKeystreamBlockGen6);
            pres.Nav.Clear(pres.UIKeystreamBlockGen7);
            pres.Nav.Clear(pres.UIKeystreamBlockGen8);
            pres.Nav.Clear(pres.UIKeystreamBlockGen9);
            pres.Nav.Clear(pres.UIKeystreamBlockGen10);
            pres.Nav.Clear(pres.UIKeystreamBlockGen11);
            pres.Nav.Clear(pres.UIKeystreamBlockGen12);
            pres.Nav.Clear(pres.UIKeystreamBlockGen13);
            pres.Nav.Clear(pres.UIKeystreamBlockGen14);
            pres.Nav.Clear(pres.UIKeystreamBlockGen15);
        }
        private void ClearStateSecondaryRow()
        {
            pres.Nav.Clear(pres.UIKeystreamBlockGen0_1);
            pres.Nav.Clear(pres.UIKeystreamBlockGen1_1);
            pres.Nav.Clear(pres.UIKeystreamBlockGen2_1);
            pres.Nav.Clear(pres.UIKeystreamBlockGen3_1);
            pres.Nav.Clear(pres.UIKeystreamBlockGen4_1);
            pres.Nav.Clear(pres.UIKeystreamBlockGen5_1);
            pres.Nav.Clear(pres.UIKeystreamBlockGen6_1);
            pres.Nav.Clear(pres.UIKeystreamBlockGen7_1);
            pres.Nav.Clear(pres.UIKeystreamBlockGen8_1);
            pres.Nav.Clear(pres.UIKeystreamBlockGen9_1);
            pres.Nav.Clear(pres.UIKeystreamBlockGen10_1);
            pres.Nav.Clear(pres.UIKeystreamBlockGen11_1);
            pres.Nav.Clear(pres.UIKeystreamBlockGen12_1);
            pres.Nav.Clear(pres.UIKeystreamBlockGen13_1);
            pres.Nav.Clear(pres.UIKeystreamBlockGen14_1);
            pres.Nav.Clear(pres.UIKeystreamBlockGen15_1);
        }
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

        public static void ShowQRButtons(ChaChaPresentation pres, int round)
        {
            for(int i = 1; i <= 8; ++i)
            {
                StackPanel qrButton = (StackPanel)GetIndexElement(pres, "QRound_Button", i);
                qrButton.Visibility = ((round % 2 == 1 || round == 0) && i <= 4) || (round % 2 == 0 && i > 4) ? Visibility.Visible : Visibility.Collapsed;
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

        private static int ResultIndex(int actionIndex, int qrIndex)
        {
            return (qrIndex - 1) * 4 + (actionIndex - 1);
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
                    ShowQRButtons(pres, round);
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
                ReplaceStateSecondaryRow(originalState.Select(x => $"+ {x}").ToArray());
            }, () =>
            {
                ClearStateSecondaryRow();
            });
            string[] previousState = GetMappedResult(ResultType.CHACHA_HASH_QUARTERROUND, pres.Rounds * 4 - 1).Select(s => ChaChaPresentation.HexString(s)).ToArray();
            PageAction addStates = new PageAction(() =>
            {
                ClearStateSecondaryRow();
                ReplaceState(GetMappedResult(ResultType.CHACHA_HASH_ADD_ORIGINAL_STATE, (int)keyBlockNr - 1).Select(u => ChaChaPresentation.HexString(u)).ToArray());
            }, () =>
            {
                ReplaceStateSecondaryRow(originalState.Select(x => $"+ {x}").ToArray());
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
            uint[] state = GetMappedResult(ResultType.CHACHA_HASH_ADD_ORIGINAL_STATE, (int)keyBlockNr - 1);
            string[] previousState = state.Select(u => ChaChaPresentation.HexString(u)).ToArray();
            string[] littleEndianState = state.Select(s => ChaChaPresentation.HexStringLittleEndian(s)).ToArray();
            PageAction convert = new PageAction(() =>
            {
                ReplaceState(littleEndianState);
            }, () =>
            {
                ReplaceState(previousState);
            });
            return new PageAction[] { updateDescription, convert };
        }
    }

    partial class Page
    {
        public static KeystreamBlockGenPage KeystreamBlockGenPage(ChaChaPresentation pres, ulong keyBlockNr)
        {
            return new KeystreamBlockGenPage(pres.UIKeystreamBlockGenPage, pres, keyBlockNr);
        }
    }
}