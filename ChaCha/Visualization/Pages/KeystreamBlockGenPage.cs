using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Cryptool.Plugins.ChaCha
{
    class KeystreamBlockGenPage : Page
    {
        ChaChaPresentation _pres;
        public KeystreamBlockGenPage(ContentControl pageElement, ChaChaPresentation pres) : base(pageElement)
        {
            _pres = pres;
            Init();
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
        }
        private void AddBoldToDescription(string descToAdd)
        {
            _pres.Nav.AddBold(_pres.UIKeystreamBlockGenStepDescription, descToAdd);
        }
        private void ClearDescription()
        {
            _pres.Nav.Clear(_pres.UIKeystreamBlockGenStepDescription);
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
        private void Init()
        {
            bool versionIsDJB = _pres.Version == ChaCha.Version.DJB;
            string DESCRIPTION_1 = "To generate a keystream block, we apply the ChaCha Hash function to the state. "
                    + "The ChaCha hash function consists of X rounds. One round applies the quarterround function four times hence the name \"quarterround\". The quarterround function takes in 4 state entries and modifies them.";
            string DESCRIPTION_2 = "The first round consists of 4 so called column rounds since we will first select all entries in a column as the input to the quarterround function. ";
            PageAction initAction = new PageAction(() =>
            {
                AddToState(
                    _pres.ConstantsLittleEndian[0], _pres.ConstantsLittleEndian[1], _pres.ConstantsLittleEndian[2], _pres.ConstantsLittleEndian[3],
                    _pres.KeyLittleEndian[0], _pres.KeyLittleEndian[1], _pres.KeyLittleEndian[2], _pres.KeyLittleEndian[3],
                    _pres.KeyLittleEndian[_pres.InputKey.Length == 32 ? 4 : 0], _pres.KeyLittleEndian[_pres.InputKey.Length == 32 ? 5 : 1], _pres.KeyLittleEndian[_pres.InputKey.Length == 32 ? 6 : 2], _pres.KeyLittleEndian[_pres.InputKey.Length == 32 ? 7 : 3],
                    _pres.InitialCounterLittleEndian[0], versionIsDJB ? _pres.InitialCounterLittleEndian[1] : _pres.IVLittleEndian[0], _pres.IVLittleEndian[versionIsDJB ? 0 : 1], _pres.IVLittleEndian[versionIsDJB ? 1 : 2]
                    );
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
                int round = calculateRoundFromQRIndex(qrIndex);
                AddAction(CopyFromStateTOQRInputActions(_pres, qrIndex, round));
                AddAction(CopyToQRDetailInput(_pres, new Border[] { _pres.QRInACell, _pres.QRInBCell, _pres.QRInDCell }, 1));
                AddAction(QRExecActions(_pres, 1, qrIndex));
                AddAction(CopyToQRDetailInput(_pres, new Border[] { _pres.QRInCCell, (Border)GetIndexElement(_pres, "QROutX3Cell", 1), (Border)GetIndexElement(_pres, "QROutX2Cell", 1) }, 2));
                AddAction(QRExecActions(_pres, 2, qrIndex));
                AddAction(CopyToQRDetailInput(_pres, new Border[] { (Border)GetIndexElement(_pres, "QROutX1Cell", 1), (Border)GetIndexElement(_pres, "QROutX3Cell", 2), (Border)GetIndexElement(_pres, "QROutX2Cell", 2) }, 3));
                AddAction(QRExecActions(_pres, 3, qrIndex));
                AddAction(CopyToQRDetailInput(_pres, new Border[] { (Border)GetIndexElement(_pres, "QROutX1Cell", 2), (Border)GetIndexElement(_pres, "QROutX3Cell", 3), (Border)GetIndexElement(_pres, "QROutX2Cell", 3) }, 4));
                AddAction(QRExecActions(_pres, 4, qrIndex));
                AddAction(QROutputActions(_pres));
                AddAction(ReplaceStateEntriesWithQROutput(_pres, qrIndex));
                AddAction(ClearQRDetail(_pres, qrIndex));
            }
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

        public static void HideAllQRArrowsExcept(ChaChaPresentation pres, int arrowIndex)
        {
            Enumerable.Range(1, 8).Select(i => ((TextBox)GetIndexElement(pres, "ArrowQRRound", i)).Visibility = Visibility.Hidden);
            ((TextBox)GetIndexElement(pres, "ArrowQRRound", arrowIndex)).Visibility = Visibility.Visible;
        }
        public static void ClearQRDetail(ChaChaPresentation pres)
        {
            pres.Nav.Clear(pres.QRInA, pres.QRInB, pres.QRInC, pres.QRInD);
            pres.Nav.Clear(pres.QROutA, pres.QROutB, pres.QROutC, pres.QROutD);
            for (int i = 1; i <= 4; ++i)
            {
                pres.Nav.Clear((TextBox)GetIndexElement(pres, "QRInX1", i));
                pres.Nav.Clear((TextBox)GetIndexElement(pres, "QRInX2", i));
                pres.Nav.Clear((TextBox)GetIndexElement(pres, "QRInX3", i));
                pres.Nav.Clear((TextBox)GetIndexElement(pres, "QROutX1", i));
                pres.Nav.Clear((TextBox)GetIndexElement(pres, "QROutX2", i));
                pres.Nav.Clear((TextBox)GetIndexElement(pres, "QROutX3", i));
                pres.Nav.Clear((TextBox)GetIndexElement(pres, "QRXOR", i));
            }
        }
        public static PageAction ClearQRDetail(ChaChaPresentation pres, int qrIndex)
        {
            string qrInA = pres.GetHexResult(ResultType.QR_INPUT_A, qrIndex - 1);
            string qrInB = pres.GetHexResult(ResultType.QR_INPUT_B, qrIndex - 1);
            string qrInC = pres.GetHexResult(ResultType.QR_INPUT_C, qrIndex - 1);
            string qrInD = pres.GetHexResult(ResultType.QR_INPUT_D, qrIndex - 1);
            string[,] qrDetailValues = new string[4, 7];
            for (int i = 0; i < 4; ++i)
            {
                qrDetailValues[i, 0] = pres.GetHexResult(ResultType.QR_INPUT_X1, i + qrIndex - 1);
                qrDetailValues[i, 1] = pres.GetHexResult(ResultType.QR_INPUT_X2, i + qrIndex - 1);
                qrDetailValues[i, 2] = pres.GetHexResult(ResultType.QR_INPUT_X3, i + qrIndex - 1);
                qrDetailValues[i, 3] = pres.GetHexResult(ResultType.QR_OUTPUT_X1, i + qrIndex - 1);
                qrDetailValues[i, 4] = pres.GetHexResult(ResultType.QR_OUTPUT_X2, i + qrIndex - 1);
                qrDetailValues[i, 5] = pres.GetHexResult(ResultType.QR_OUTPUT_X3, i + qrIndex - 1);
                qrDetailValues[i, 6] = pres.GetHexResult(ResultType.QR_XOR, i + qrIndex - 1);
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
                    pres.Nav.Replace((TextBox)GetIndexElement(pres, "QRInX1", i),  qrDetailValues[i - 1, 0]);
                    pres.Nav.Replace((TextBox)GetIndexElement(pres, "QRInX2", i),  qrDetailValues[i - 1, 1]);
                    pres.Nav.Replace((TextBox)GetIndexElement(pres, "QRInX3", i),  qrDetailValues[i - 1, 2]);
                    pres.Nav.Replace((TextBox)GetIndexElement(pres, "QROutX1", i), qrDetailValues[i - 1, 3]);
                    pres.Nav.Replace((TextBox)GetIndexElement(pres, "QROutX2", i), qrDetailValues[i - 1 , 4]);
                    pres.Nav.Replace((TextBox)GetIndexElement(pres, "QROutX3", i), qrDetailValues[i - 1, 5]);
                    pres.Nav.Replace((TextBox)GetIndexElement(pres, "QRXOR", i),   qrDetailValues[i - 1, 6]);
                }
            });
        }

        private static int ResultIndex(int actionIndex, int qrIndex)
        {
            return (qrIndex - 1) * 4 + (actionIndex - 1);
        }

        private static PageAction[] QROutputActions(ChaChaPresentation pres)
        {
            return pres.Nav.CopyActions(
                new Border[] { (Border)GetIndexElement(pres, "QROutX1Cell", 3), (Border)GetIndexElement(pres, "QROutX3Cell", 4), (Border)GetIndexElement(pres, "QROutX1Cell", 4), (Border)GetIndexElement(pres, "QROutX2Cell", 4) },
                new Shape[] { pres.QROutAPath, pres.QROutBPath, pres.QROutCPath, pres.QROutDPath },
                new Border[] { pres.QROutACell, pres.QROutBCell, pres.QROutCCell, pres.QROutDCell },
                new string[] { "", "", "", "" }
                );
        }

        private static PageAction[] X1OutActions(ChaChaPresentation pres, int actionIndex, int qrIndex)
        {
            void MarkX1Output()
            {
                pres.Nav.MarkBorder((Border)GetIndexElement(pres, "QRInX1Cell", actionIndex));
                pres.Nav.MarkBorder((Border)GetIndexElement(pres, "QRInX2Cell", actionIndex));
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "AddInputPathX1", actionIndex));
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "AddInputPathX2", actionIndex));
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "OutputPathX1", actionIndex));
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "OutputPathX2_1", actionIndex));
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "AddCircle", actionIndex));
                pres.Nav.MarkBorder((Border)GetIndexElement(pres, "QROutX1Cell", actionIndex));
            }
            void UnmarkX1Output()
            {
                pres.Nav.UnmarkBorder((Border)GetIndexElement(pres, "QRInX1Cell", actionIndex));
                pres.Nav.UnmarkBorder((Border)GetIndexElement(pres, "QRInX2Cell", actionIndex));
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "AddInputPathX1", actionIndex));
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "AddInputPathX2", actionIndex));
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "OutputPathX1", actionIndex));
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "OutputPathX2_1", actionIndex));
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "AddCircle", actionIndex));
                pres.Nav.UnmarkBorder((Border)GetIndexElement(pres, "QROutX1Cell", actionIndex));
            }
            PageAction markOutX1 = new PageAction(MarkX1Output, UnmarkX1Output);
            PageAction execOutX1 = new PageAction(() =>
            {
                pres.Nav.Replace((TextBox)GetIndexElement(pres, "QROutX1", actionIndex), pres.GetHexResult(ResultType.QR_ADD_X1_X2, ResultIndex(actionIndex, qrIndex)));
            }, () =>
            {
                pres.Nav.Clear((TextBox)GetIndexElement(pres, "QROutX1", actionIndex));
            });
            PageAction unmarkOutX1 = new PageAction(UnmarkX1Output, MarkX1Output);
            return new PageAction[] { markOutX1, execOutX1, unmarkOutX1 };
        }

        private static PageAction[] X2OutActions(ChaChaPresentation pres, int actionIndex, int qrIndex)
        {
            void MarkX2Output()
            {
                pres.Nav.MarkBorder((Border)GetIndexElement(pres, "QRInX2Cell", actionIndex));
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "OutputPathX2_1", actionIndex));
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "OutputPathX2_2", actionIndex));
                pres.Nav.MarkBorder((Border)GetIndexElement(pres, "QROutX2Cell", actionIndex));
            }
            void UnmarkX2Output()
            {
                pres.Nav.UnmarkBorder((Border)GetIndexElement(pres, "QRInX2Cell", actionIndex));
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "OutputPathX2_1", actionIndex));
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "OutputPathX2_2", actionIndex));
                pres.Nav.UnmarkBorder((Border)GetIndexElement(pres, "QROutX2Cell", actionIndex));
            }
            PageAction markOutX2 = new PageAction(MarkX2Output, UnmarkX2Output);
            PageAction execOutX2 = new PageAction(() =>
            {
                pres.Nav.Replace((TextBox)GetIndexElement(pres, "QROutX2", actionIndex), pres.GetHexResult(ResultType.QR_OUTPUT_X2, ResultIndex(actionIndex, qrIndex)));
            }, () =>
            {
                pres.Nav.Clear((TextBox)GetIndexElement(pres, "QROutX2", actionIndex));
            });
            PageAction unmarkOutX2 = new PageAction(UnmarkX2Output, MarkX2Output);
            return new PageAction[] { markOutX2, execOutX2, unmarkOutX2 };
        }

        private static PageAction[] XORActions(ChaChaPresentation pres, int actionIndex, int qrIndex)
        {
            void MarkXOR()
            {
                pres.Nav.MarkBorder((Border)GetIndexElement(pres, "QRInX3Cell", actionIndex));
                pres.Nav.MarkBorder((Border)GetIndexElement(pres, "QROutX1Cell", actionIndex));
                pres.Nav.MarkBorder((Border)GetIndexElement(pres, "QRXORCell", actionIndex));
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "OutputPathX3_1", actionIndex));
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "XORInputPathX1", actionIndex));
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "XORCircle", actionIndex));
            }
            void UnmarkXOR()
            {
                pres.Nav.UnmarkBorder((Border)GetIndexElement(pres, "QRInX3Cell", actionIndex));
                pres.Nav.UnmarkBorder((Border)GetIndexElement(pres, "QROutX1Cell", actionIndex));
                pres.Nav.UnmarkBorder((Border)GetIndexElement(pres, "QRXORCell", actionIndex));
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "OutputPathX3_1", actionIndex));
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "XORInputPathX1", actionIndex));
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "XORCircle", actionIndex));
            }
            PageAction markXOR = new PageAction(MarkXOR, UnmarkXOR);
            PageAction execXOR = new PageAction(() =>
            {
                pres.Nav.Replace((TextBox)GetIndexElement(pres, "QRXOR", actionIndex), pres.GetHexResult(ResultType.QR_XOR, ResultIndex(actionIndex, qrIndex)));
            }, () =>
            {
                pres.Nav.Clear((TextBox)GetIndexElement(pres, "QRXOR", actionIndex));
            });
            PageAction unmarkXOR = new PageAction(UnmarkXOR, MarkXOR);
            return new PageAction[] { markXOR, execXOR, unmarkXOR };
        }

        private static PageAction[] ShiftActions(ChaChaPresentation pres, int actionIndex, int qrIndex)
        {
            void MarkShift()
            {
                pres.Nav.MarkBorder((Border)GetIndexElement(pres, "QROutX3Cell", actionIndex));
                pres.Nav.MarkBorder((Border)GetIndexElement(pres, "QRXORCell", actionIndex));
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "OutputPathX3_2", actionIndex));
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "OutputPathX3_3", actionIndex));
                pres.Nav.MarkShape((Shape)GetIndexElement(pres, "ShiftCircle", actionIndex));
            }
            void UnmarkShift()
            {
                pres.Nav.UnmarkBorder((Border)GetIndexElement(pres, "QROutX3Cell", actionIndex));
                pres.Nav.UnmarkBorder((Border)GetIndexElement(pres, "QRXORCell", actionIndex));
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "OutputPathX3_2", actionIndex));
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "OutputPathX3_3", actionIndex));
                pres.Nav.UnmarkShape((Shape)GetIndexElement(pres, "ShiftCircle", actionIndex));
            }
            PageAction markShift = new PageAction(MarkShift, UnmarkShift);
            PageAction execShift = new PageAction(() =>
            {
                pres.Nav.Replace((TextBox)GetIndexElement(pres, "QROutX3", actionIndex), pres.GetHexResult(ResultType.QR_OUTPUT_X3, ResultIndex(actionIndex, qrIndex)));
            }, () =>
            {
                pres.Nav.Clear((TextBox)GetIndexElement(pres, "QROutX3", actionIndex));
            });
            PageAction unmarkShift = new PageAction(UnmarkShift, MarkShift);
            return new PageAction[] { markShift, execShift, unmarkShift };
        }

        private static PageAction[] X3OutActions(ChaChaPresentation pres, int actionIndex, int qrIndex)
        {
            PageAction[] xorActions = XORActions(pres, actionIndex, qrIndex);
            PageAction[] shiftActions = ShiftActions(pres, actionIndex, qrIndex);
            PageAction[] actions = new PageAction[xorActions.Length + shiftActions.Length];
            xorActions.CopyTo(actions, 0);
            shiftActions.CopyTo(actions, xorActions.Length);
            return actions;
        }

        private static PageAction[] QRExecActions(ChaChaPresentation pres, int actionIndex, int qrIndex)
        {
            List<PageAction> actions = new List<PageAction>();
            PageAction[] x1OutActions = X1OutActions(pres, actionIndex, qrIndex);
            PageAction[] x2OutActions = X2OutActions(pres, actionIndex, qrIndex);
            PageAction[] x3OutActions = X3OutActions(pres, actionIndex, qrIndex);
            actions.AddRange(x1OutActions);
            actions.AddRange(x2OutActions);
            actions.AddRange(x3OutActions);
            return actions.ToArray();
        }

        private static PageAction[] CopyToQRDetailInput(ChaChaPresentation pres, Border[] b, int actionIndex)
        {
            return pres.Nav.CopyActions(
                b,
                new Shape[] { (Shape)GetIndexElement(pres, "QRInX1Path", actionIndex), (Shape)GetIndexElement(pres, "QRInX2Path", actionIndex), (Shape)GetIndexElement(pres, "QRInX3Path", actionIndex) },
                new Border[] { (Border)GetIndexElement(pres, "QRInX1Cell", actionIndex), (Border)GetIndexElement(pres, "QRInX2Cell", actionIndex), (Border)GetIndexElement(pres, "QRInX3Cell", actionIndex) },
                new string[] { "", "", "" }
                );
        }

        private static (int, int, int, int) GetStateIndices(int qrIndex)
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
                    Debug.Assert(false, string.Format("No state indices found for round", qrIndex));
                    return (-1, -1, -1, -1);
            }
        }
        private int calculateRoundFromQRIndex(int qrIndex)
        {
            return (int)Math.Floor((double)(qrIndex - 1) / 4) + 1;
        }
        private static PageAction[] CopyFromStateTOQRInputActions(ChaChaPresentation pres, int qrIndex, int round)
        {
            uint[] state = pres.GetResult(ResultType.CHACHA_HASH_ROUND, round - 1);
            (int i, int j, int k, int l) = GetStateIndices(qrIndex);
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
                copyActions[0].AddLabel(string.Format("_ROUND_ACTION_LABEL_{0}", round));
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
            copyActions[0].AddLabel(string.Format("_QR_ACTION_LABEL_{0}_{1}", ((qrIndex - 1) % 4) + 1, round));
            return copyActions;
        }

        private static PageAction[] ReplaceStateEntriesWithQROutput(ChaChaPresentation pres, int qrIndex)
        {
            (int i, int j, int k, int l) = GetStateIndices(qrIndex);
            Border[] stateCells = new Border[] { GetStateCell(pres, i), GetStateCell(pres, j), GetStateCell(pres, k), GetStateCell(pres, l) };
            string[] previousStateEntries = new string[4];
            previousStateEntries[0] = pres.GetHexResult(ResultType.QR_INPUT_A, qrIndex - 1);
            previousStateEntries[1] = pres.GetHexResult(ResultType.QR_INPUT_A, qrIndex - 1);
            previousStateEntries[2] = pres.GetHexResult(ResultType.QR_INPUT_A, qrIndex - 1);
            previousStateEntries[3] = pres.GetHexResult(ResultType.QR_INPUT_A, qrIndex - 1);
            return pres.Nav.CopyActions(new Border[] { pres.QROutACell, pres.QROutBCell, pres.QROutCCell, pres.QROutDCell }, stateCells, previousStateEntries, true);
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