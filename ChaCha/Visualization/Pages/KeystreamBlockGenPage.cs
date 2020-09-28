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
        ChaChaPresentation _pres;
        static string DESCRIPTION_1 = "To generate a keystream block, we apply the ChaCha Hash function to the state. "
                    + "The ChaCha hash function consists of X rounds. One round applies the quarterround function four times hence the name \"quarterround\". The quarterround function takes in 4 state entries and modifies them.";
        static string DESCRIPTION_2 = "The first round consists of 4 so called column rounds since we will first select all entries in a column as the input to the quarterround function. ";


        const string ACTIONLABEL_QR_START = "QUARTERROUND";
        const string ACTIONLABEL_ROUND_START = "ROUND";

        public KeystreamBlockGenPage(ContentControl pageElement, ChaChaPresentation pres) : base(pageElement, GenerateCache(pres))
        {
            _pres = pres;
            Init();
        }

        private void Init()
        {
            bool versionIsDJB = _pres.Version == ChaCha.Version.DJB;
            PageAction initAction = new PageAction(() =>
            {
                AddToState(
                    _pres.ConstantsLittleEndian[0], _pres.ConstantsLittleEndian[1], _pres.ConstantsLittleEndian[2], _pres.ConstantsLittleEndian[3],
                    _pres.KeyLittleEndian[0], _pres.KeyLittleEndian[1], _pres.KeyLittleEndian[2], _pres.KeyLittleEndian[3],
                    _pres.KeyLittleEndian[_pres.InputKey.Length == 32 ? 4 : 0], _pres.KeyLittleEndian[_pres.InputKey.Length == 32 ? 5 : 1], _pres.KeyLittleEndian[_pres.InputKey.Length == 32 ? 6 : 2], _pres.KeyLittleEndian[_pres.InputKey.Length == 32 ? 7 : 3],
                    _pres.InitialCounterLittleEndian[0], versionIsDJB ? _pres.InitialCounterLittleEndian[1] : _pres.IVLittleEndian[0], _pres.IVLittleEndian[versionIsDJB ? 0 : 1], _pres.IVLittleEndian[versionIsDJB ? 1 : 2]
                    );
                AssertStateAfterQuarterround(_pres, 0);
            }, () =>
            {
                ClearState();
            });
            AddInitAction(initAction);
            PageAction generalDescriptionAction = new PageAction(() =>
            {
                AddBoldToDescription(DESCRIPTION_1);
            }, () =>
            {
                ClearDescription();
            });
            PageAction firstColumnRoundDescriptionAction = new PageAction(() =>
            {
                UnboldLastFromDescription();
                AddBoldToDescription(DESCRIPTION_2);
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
            });
            AddAction(generalDescriptionAction);
            AddAction(firstColumnRoundDescriptionAction);

            for (int qrIndex = 1; qrIndex <= _pres.Rounds * 4; ++qrIndex)
            {
                int round = CalculateRoundFromQRIndex(qrIndex);
                AddAction(CopyFromStateTOQRInputActions(_pres, qrIndex, round));
                AddAction(QRExecActions(_pres, 1, qrIndex));
                AddAction(QRExecActions(_pres, 2, qrIndex));
                AddAction(QRExecActions(_pres, 3, qrIndex));
                AddAction(QRExecActions(_pres, 4, qrIndex));
                AddAction(QROutputActions(_pres));
                AddAction(ReplaceStateEntriesWithQROutput(_pres, qrIndex));
                AddAction(ClearQRDetail(_pres, qrIndex));
            }
        }

        private static ActionCache GenerateCache(ChaChaPresentation pres)
        {
            return GenerateQuarterroundCache(pres);
        }
        private static ActionCache GenerateQuarterroundCache(ChaChaPresentation pres)
        {
            CachePageAction GenerateQuarterroundCacheEntry(int qrIndex)
            {
                CachePageAction cache = new CachePageAction();
                cache.AddToExec(() =>
                {
                    ClearDescription(pres);
                    AddToDescription(pres, DESCRIPTION_1);
                    AddBoldToDescription(pres, DESCRIPTION_2);
                    ClearQRDetail(pres);
                    // update state matrix
                    TextBox[] stateTextBoxes = GetStateTextBoxes(pres);
                    uint[] stateEntries = pres.GetResult(ResultType.CHACHA_HASH_QUARTERROUND, qrIndex - 1);
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
                    pres.CurrentRoundIndex = CalculateRoundFromQRIndex(qrIndex);
                    pres.Nav.Replace(pres.CurrentRound, round.ToString());
                    HideAllQRArrowsExcept(pres, ((qrIndex - 1) % 8) + 1);
                });
                return cache;
            }
            int QUARTERROUND_ACTION_STEP = 70;
            int QUARTERRROUND_FIRST_ACTION_INDEX = 3;
            ActionCache qrCache = new ActionCache();
            for (int qrIndex = 1; qrIndex <= pres.Rounds * 4; ++qrIndex)
            {
                int actionIndex = QUARTERRROUND_FIRST_ACTION_INDEX + (qrIndex - 1) * QUARTERROUND_ACTION_STEP;
                qrCache.Set(GenerateQuarterroundCacheEntry(qrIndex), actionIndex);
            }
            return ActionCache.Empty;
            return qrCache;
        }
        private void AddToState(
                string state0, string state1, string state2, string state3,
                string state4, string state5, string state6, string state7,
                string state8, string state9, string state10, string state11,
                string state12, string state13, string state14, string state15)
        {
            _pres.Nav.Replace(_pres.UIKeystreamBlockGen0, state0);
            _pres.Nav.Replace(_pres.UIKeystreamBlockGen1, state1);
            _pres.Nav.Replace(_pres.UIKeystreamBlockGen2, state2);
            _pres.Nav.Replace(_pres.UIKeystreamBlockGen3, state3);
            _pres.Nav.Replace(_pres.UIKeystreamBlockGen4, state4);
            _pres.Nav.Replace(_pres.UIKeystreamBlockGen5, state5);
            _pres.Nav.Replace(_pres.UIKeystreamBlockGen6, state6);
            _pres.Nav.Replace(_pres.UIKeystreamBlockGen7, state7);
            _pres.Nav.Replace(_pres.UIKeystreamBlockGen8, state8);
            _pres.Nav.Replace(_pres.UIKeystreamBlockGen9, state9);
            _pres.Nav.Replace(_pres.UIKeystreamBlockGen10, state10);
            _pres.Nav.Replace(_pres.UIKeystreamBlockGen11, state11);
            _pres.Nav.Replace(_pres.UIKeystreamBlockGen12, state12);
            _pres.Nav.Replace(_pres.UIKeystreamBlockGen13, state13);
            _pres.Nav.Replace(_pres.UIKeystreamBlockGen14, state14);
            _pres.Nav.Replace(_pres.UIKeystreamBlockGen15, state15);
        }
        private void ClearState()
        {
            _pres.Nav.Clear(_pres.UIKeystreamBlockGen0);
            _pres.Nav.Clear(_pres.UIKeystreamBlockGen1);
            _pres.Nav.Clear(_pres.UIKeystreamBlockGen2);
            _pres.Nav.Clear(_pres.UIKeystreamBlockGen3);
            _pres.Nav.Clear(_pres.UIKeystreamBlockGen4);
            _pres.Nav.Clear(_pres.UIKeystreamBlockGen5);
            _pres.Nav.Clear(_pres.UIKeystreamBlockGen6);
            _pres.Nav.Clear(_pres.UIKeystreamBlockGen7);
            _pres.Nav.Clear(_pres.UIKeystreamBlockGen8);
            _pres.Nav.Clear(_pres.UIKeystreamBlockGen9);
            _pres.Nav.Clear(_pres.UIKeystreamBlockGen10);
            _pres.Nav.Clear(_pres.UIKeystreamBlockGen11);
            _pres.Nav.Clear(_pres.UIKeystreamBlockGen12);
            _pres.Nav.Clear(_pres.UIKeystreamBlockGen13);
            _pres.Nav.Clear(_pres.UIKeystreamBlockGen14);
            _pres.Nav.Clear(_pres.UIKeystreamBlockGen15);
        }
        private static void AddBoldToDescription(ChaChaPresentation _pres, string descToAdd)
        {
            _pres.Nav.AddBold(_pres.UIKeystreamBlockGenStepDescription, descToAdd);
        }
        private void AddBoldToDescription(string descToAdd)
        {
            AddBoldToDescription(_pres, descToAdd);
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
            ClearDescription(_pres);
        }
        private void UnboldLastFromDescription()
        {

            _pres.Nav.UnboldLast(_pres.UIKeystreamBlockGenStepDescription);
        }
        void MakeLastBoldInDescription()
        {
            _pres.Nav.MakeBoldLast(_pres.UIKeystreamBlockGenStepDescription);
        }
        void RemoveLastFromDescription()
        {
            _pres.Nav.RemoveLast(_pres.UIKeystreamBlockGenStepDescription);
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
            for(int x = 0; x < 16; ++x)
            {
                pres.Nav.UnsetBackground((Border)GetIndexElement(pres, "UIKeystreamBlockGenCell", x));
            }
            pres.Nav.SetCopyBackground((Border)GetIndexElement(pres, "UIKeystreamBlockGenCell", i));
            pres.Nav.SetCopyBackground((Border)GetIndexElement(pres, "UIKeystreamBlockGenCell", j));
            pres.Nav.SetCopyBackground((Border)GetIndexElement(pres, "UIKeystreamBlockGenCell", k));
            pres.Nav.SetCopyBackground((Border)GetIndexElement(pres, "UIKeystreamBlockGenCell", l));
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
        public static void ClearQRDetail(ChaChaPresentation pres)
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
        public static PageAction ClearQRDetail(ChaChaPresentation pres, int qrIndex)
        {
            string qrInA = pres.GetHexResult(ResultType.QR_INPUT_A, qrIndex - 1);
            string qrInB = pres.GetHexResult(ResultType.QR_INPUT_B, qrIndex - 1);
            string qrInC = pres.GetHexResult(ResultType.QR_INPUT_C, qrIndex - 1);
            string qrInD = pres.GetHexResult(ResultType.QR_INPUT_D, qrIndex - 1);
            string[,] qrDetailValues = new string[4, 3];
            for (int i = 0; i < 4; ++i)
            {
                qrDetailValues[i, 0] = pres.GetHexResult(ResultType.QR_ADD, i + qrIndex - 1);
                qrDetailValues[i, 1] = pres.GetHexResult(ResultType.QR_XOR, i + qrIndex - 1);
                qrDetailValues[i, 2] = pres.GetHexResult(ResultType.QR_SHIFT, i + qrIndex - 1);
            }
            string qrOutA = pres.GetHexResult(ResultType.QR_OUTPUT_A, qrIndex - 1);
            string qrOutB = pres.GetHexResult(ResultType.QR_OUTPUT_B, qrIndex - 1);
            string qrOutC = pres.GetHexResult(ResultType.QR_OUTPUT_C, qrIndex - 1);
            string qrOutD = pres.GetHexResult(ResultType.QR_OUTPUT_D, qrIndex - 1);

            return new PageAction(() =>
            {
                ClearQRDetail(pres);
            }, () =>
            {
                pres.Nav.Replace(pres.QRInA, qrInA);
                pres.Nav.Replace(pres.QRInB, qrInB);
                pres.Nav.Replace(pres.QRInC, qrInC);
                pres.Nav.Replace(pres.QRInD, qrInD);
                for (int i = 1; i <= 4; ++i)
                {
                    pres.Nav.Replace((TextBox)GetIndexElement(pres, "QRAdd", i), qrDetailValues[i - 1, 0]);
                    pres.Nav.Replace((TextBox)GetIndexElement(pres, "QRXOR", i),   qrDetailValues[i - 1, 1]);
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

        private static PageAction[] QROutputActions(ChaChaPresentation pres)
        {
            return pres.Nav.CopyActions(
                new Border[] { (Border)GetIndexElement(pres, "QRAddCell", 3), (Border)GetIndexElement(pres, "QRShiftCell", 4), (Border)GetIndexElement(pres, "QRAddCell", 4), (Border)GetIndexElement(pres, "QRShiftCell", 3) },
                new Shape[] { pres.QROutputPath_A, pres.QROutputPath_B, pres.QROutputPath_C, pres.QROutputPath_D },
                new Border[] { pres.QROutACell, pres.QROutBCell, pres.QROutCCell, pres.QROutDCell },
                new string[] { "", "", "", "" }
                );
        }

        // TODO: Following three functions are very similar and can be refactored into one function
        private static PageAction[] ExecAddActions(ChaChaPresentation pres, int actionIndex, int qrIndex)
        {
            void Mark()
            {
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "QRAddPath", actionIndex));
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "QRAddCircle", actionIndex));
                pres.Nav.MarkBorder((Border)GetIndexElement(pres, "QRAddCell", actionIndex));
            }
            void Unmark()
            {
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "QRAddPath", actionIndex));
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "QRAddCircle", actionIndex));
                pres.Nav.UnmarkBorder((Border)GetIndexElement(pres, "QRAddCell", actionIndex));
            }
            PageAction mark = new PageAction(Mark, Unmark);
            PageAction exec = new PageAction(() =>
            {
                pres.Nav.Replace((TextBox)GetIndexElement(pres, "QRAdd", actionIndex), pres.GetHexResult(ResultType.QR_ADD, ResultIndex(actionIndex, qrIndex)));
            }, () =>
            {
                pres.Nav.Clear((TextBox)GetIndexElement(pres, "QRAdd", actionIndex));
            });
            PageAction unmark = new PageAction(Unmark, Mark);
            return new PageAction[] { mark, exec, unmark };
        }
        private static PageAction[] ExecXORActions(ChaChaPresentation pres, int actionIndex, int qrIndex)
        {
            void Mark()
            {
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "QRXORPath", actionIndex));
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "QRXORCircle", actionIndex));
                pres.Nav.MarkBorder((Border)GetIndexElement(pres, "QRXORCell", actionIndex));
            }
            void Unmark()
            {
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "QRXORPath", actionIndex));
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "QRXORCircle", actionIndex));
                pres.Nav.UnmarkBorder((Border)GetIndexElement(pres, "QRXORCell", actionIndex));
            }
            PageAction mark = new PageAction(Mark, Unmark);
            PageAction exec = new PageAction(() =>
            {
                pres.Nav.Replace((TextBox)GetIndexElement(pres, "QRXOR", actionIndex), pres.GetHexResult(ResultType.QR_XOR, ResultIndex(actionIndex, qrIndex)));
            }, () =>
            {
                pres.Nav.Clear((TextBox)GetIndexElement(pres, "QRXOR", actionIndex));
            });
            PageAction unmark = new PageAction(Unmark, Mark);
            return new PageAction[] { mark, exec, unmark };
        }
        private static PageAction[] ExecShiftActions(ChaChaPresentation pres, int actionIndex, int qrIndex)
        {
            void Mark()
            {
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "QRShiftPath", actionIndex));
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "QRShiftCircle", actionIndex));
                pres.Nav.MarkBorder((Border)GetIndexElement(pres, "QRShiftCell", actionIndex));
            }
            void Unmark()
            {
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "QRShiftPath", actionIndex));
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "QRShiftCircle", actionIndex));
                pres.Nav.UnmarkBorder((Border)GetIndexElement(pres, "QRShiftCell", actionIndex));
            }
            PageAction mark = new PageAction(Mark, Unmark);
            PageAction exec = new PageAction(() =>
            {
                pres.Nav.Replace((TextBox)GetIndexElement(pres, "QRShift", actionIndex), pres.GetHexResult(ResultType.QR_SHIFT, ResultIndex(actionIndex, qrIndex)));
            }, () =>
            {
                pres.Nav.Clear((TextBox)GetIndexElement(pres, "QRShift", actionIndex));
            });
            PageAction unmark = new PageAction(Unmark, Mark);
            return new PageAction[] { mark, exec, unmark };
        }

        private static void AssertQRExecActions(ChaChaPresentation pres, int actionIndex, int qrIndex)
        {
            int resultIndex = ResultIndex(actionIndex, qrIndex);
            string add = ((TextBox)GetIndexElement(pres, "QRAdd", actionIndex)).Text;
            string expectedAdd = pres.GetHexResult(ResultType.QR_ADD, resultIndex);
            string xor = ((TextBox)GetIndexElement(pres, "QRXOR", actionIndex)).Text;
            string expectedXor = pres.GetHexResult(ResultType.QR_XOR, resultIndex);
            string shift = ((TextBox)GetIndexElement(pres, "QRShift", actionIndex)).Text;
            string expectedShift = pres.GetHexResult(ResultType.QR_SHIFT, resultIndex);
            Debug.Assert(add == expectedAdd, $"Visual value after ADD execution does not match actual value! expected {expectedAdd}, got {add}");
            Debug.Assert(xor == expectedXor, $"Visual value after XOR execution does not match actual value! expected {expectedXor}, got {xor}");
            Debug.Assert(shift == expectedShift, $"Visual value after SHIFT execution does not match actual value! expected {expectedShift}, got {shift}");
        }

        private static PageAction[] QRExecActions(ChaChaPresentation pres, int actionIndex, int qrIndex)
        {
            List<PageAction> actions = new List<PageAction>();
            PageAction[] execAdd = ExecAddActions(pres, actionIndex, qrIndex);
            PageAction[] execXOR = ExecXORActions(pres, actionIndex, qrIndex);
            PageAction[] execShift = ExecShiftActions(pres, actionIndex, qrIndex);
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
            // Creates a label with quarterround indicator between 1-8.
            // For example, the label for the beginning of third quarterround in the secound round: *PREFIX*_3_2
            return $"{ACTIONLABEL_QR_START}_{((qrIndex - 1) % 4) + 1}_{round}";
        }
        public static string RoundStartLabel(int round)
        {
            return $"{ACTIONLABEL_ROUND_START}_{round}";
        }
        private static PageAction[] CopyFromStateTOQRInputActions(ChaChaPresentation pres, int qrIndex, int round)
        {
            uint[] state = pres.GetResult(ResultType.CHACHA_HASH_QUARTERROUND, (round * 4) - 1);
            (int i, int j, int k, int l) = GetStateIndicesFromQRIndex(qrIndex);
            Border[] stateCells = new Border[] { GetStateCell(pres, i), GetStateCell(pres, j), GetStateCell(pres, k), GetStateCell(pres, l) };
            PageAction[] copyActions = pres.Nav.CopyActions(stateCells, 
                new Border[] { pres.QRInACell, pres.QRInBCell, pres.QRInCCell, pres.QRInDCell },
                new string[] { "", "", "", "" }
                );
            if ((qrIndex - 1) % 4 == 0)
            {
                // new round begins
                PageAction updateRoundCount = new PageAction(() =>
                {
                    pres.CurrentRoundIndex = round;
                    pres.Nav.Replace(pres.CurrentRound, round.ToString());
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
                });
                copyActions[0].Add(updateRoundCount);
                copyActions[0].AddLabel(RoundStartLabel(round));
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
                (int qrArrowIndex, int qrPrevArrowIndex) = calculateQRArrowIndex(qrIndex);
                ((TextBox)GetIndexElement(pres, "ArrowQRRound", qrPrevArrowIndex)).Visibility = Visibility.Hidden;
                ((TextBox)GetIndexElement(pres, "ArrowQRRound", qrArrowIndex)).Visibility = Visibility.Visible;
            }, () =>
            {
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

        private static void AssertStateAfterQuarterround(ChaChaPresentation pres, int qrIndex)
        {
            // Check that the state entries in the state matrix visualizatoin are the same as the actual values in the uint[] array
            string[] expectedState = pres.GetResult(ResultType.CHACHA_HASH_QUARTERROUND, qrIndex).Select(s => ChaChaPresentation.HexString(s)).ToArray();
            string[] visualState = new string[16];
            for (int i = 0; i < 16; ++i)
            {
                visualState[i] = ((TextBox)GetIndexElement(pres, "UIKeystreamBlockGen", i, "")).Text;
                Debug.Assert(expectedState[i] == visualState[i], $"Visual state after quarterround {qrIndex} execution does not match actual state! expected {expectedState[i]} at index {i}, but got {visualState[i]}");
            }
        }

        private static PageAction[] ReplaceStateEntriesWithQROutput(ChaChaPresentation pres, int qrIndex)
        {
            (int i, int j, int k, int l) = GetStateIndicesFromQRIndex(qrIndex);
            Border[] stateCells = new Border[] { GetStateCell(pres, i), GetStateCell(pres, j), GetStateCell(pres, k), GetStateCell(pres, l) };
            string[] previousStateEntries = new string[4];
            previousStateEntries[0] = pres.GetHexResult(ResultType.QR_INPUT_A, qrIndex - 1);
            previousStateEntries[1] = pres.GetHexResult(ResultType.QR_INPUT_B, qrIndex - 1);
            previousStateEntries[2] = pres.GetHexResult(ResultType.QR_INPUT_C, qrIndex - 1);
            previousStateEntries[3] = pres.GetHexResult(ResultType.QR_INPUT_D, qrIndex - 1);
            PageAction[] copyActions = pres.Nav.CopyActions(new Border[] { pres.QROutACell, pres.QROutBCell, pres.QROutCCell, pres.QROutDCell }, stateCells, previousStateEntries, true);
            copyActions[2].AddToExec(() =>
            {
                AssertStateAfterQuarterround(pres, qrIndex);
            });
            return copyActions;
        }
    }
    partial class Page
    {
        public static Page KeystreamBlockGenPage(ChaChaPresentation pres)
        {
            return new KeystreamBlockGenPage(pres.UIKeystreamBlockGenPage, pres);
        }
    }
}