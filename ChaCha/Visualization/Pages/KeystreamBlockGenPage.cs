using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Cryptool.Plugins.ChaCha
{
    partial class Page
    {
        public static Page KeystreamBlockGenPage(ChaChaPresentation pres)
        {
            int ResultIndex(int actionIndex, int qrIndex)
            {
                return (qrIndex - 1) * 4 + (actionIndex - 1);
            }
            object GetIndexElement(string nameId, int index)
            {
                return pres.FindName(string.Format("{0}_{1}", nameId, index));
            }

            PageAction[] QROutputActions()
            {
                return pres.Nav.CopyActions(
                    new Border[] { (Border)GetIndexElement("QROutX1Cell", 3), (Border)GetIndexElement("QROutX3Cell", 4), (Border)GetIndexElement("QROutX1Cell", 4), (Border)GetIndexElement("QROutX2Cell", 4) },
                    new Shape[] { pres.QROutAPath, pres.QROutBPath, pres.QROutCPath, pres.QROutDPath },
                    new Border[] { pres.QROutACell, pres.QROutBCell, pres.QROutCCell, pres.QROutDCell }
                    );
            }

            PageAction[] X1OutActions(int actionIndex, int qrIndex)
            {
                PageAction markOutX1 = new PageAction(() =>
                {
                    pres.Nav.MarkBorder((Border)GetIndexElement("QRInX1Cell", actionIndex));
                    pres.Nav.MarkBorder((Border)GetIndexElement("QRInX2Cell", actionIndex));
                    pres.Nav.MarkShape((Shape)GetIndexElement("AddInputPathX1", actionIndex));
                    pres.Nav.MarkShape((Shape)GetIndexElement("AddInputPathX2", actionIndex));
                    pres.Nav.MarkShape((Shape)GetIndexElement("OutputPathX1", actionIndex));
                    pres.Nav.MarkShape((Shape)GetIndexElement("OutputPathX2_1", actionIndex));
                    pres.Nav.MarkShape((Shape)GetIndexElement("AddCircle", actionIndex));
                    pres.Nav.MarkBorder((Border)GetIndexElement("QROutX1Cell", actionIndex));
                }, pres.Nav.Undo);
                PageAction execOutX1 = new PageAction(() =>
                {
                    pres.Nav.Replace((TextBox)GetIndexElement("QROutX1", actionIndex), pres.GetHexResult(ResultType.QR_ADD_X1_X2, ResultIndex(actionIndex, qrIndex)));
                }, pres.Nav.Undo);
                PageAction unmarkOutX1 = new PageAction(() =>
                {
                    pres.Nav.UnmarkBorder((Border)GetIndexElement("QRInX1Cell", actionIndex));
                    pres.Nav.UnmarkBorder((Border)GetIndexElement("QRInX2Cell", actionIndex));
                    pres.Nav.UnmarkShape((Shape)GetIndexElement("AddInputPathX1", actionIndex));
                    pres.Nav.UnmarkShape((Shape)GetIndexElement("AddInputPathX2", actionIndex));
                    pres.Nav.UnmarkShape((Shape)GetIndexElement("OutputPathX1", actionIndex));
                    pres.Nav.UnmarkShape((Shape)GetIndexElement("OutputPathX2_1", actionIndex));
                    pres.Nav.UnmarkShape((Shape)GetIndexElement("AddCircle", actionIndex));
                    pres.Nav.UnmarkBorder((Border)GetIndexElement("QROutX1Cell", actionIndex));
                }, pres.Nav.Undo);
                return new PageAction[] { markOutX1, execOutX1, unmarkOutX1 };
            }

            PageAction[] X2OutActions(int actionIndex, int qrIndex)
            {
                PageAction markOutX2 = new PageAction(() =>
                {
                    pres.Nav.MarkBorder((Border)GetIndexElement("QRInX2Cell", actionIndex));
                    pres.Nav.MarkShape((Shape)GetIndexElement("OutputPathX2_1", actionIndex));
                    pres.Nav.MarkShape((Shape)GetIndexElement("OutputPathX2_2", actionIndex));
                    pres.Nav.MarkBorder((Border)GetIndexElement("QROutX2Cell", actionIndex));
                }, pres.Nav.Undo);
                PageAction execOutX2 = new PageAction(() =>
                {
                    pres.Nav.Replace((TextBox)GetIndexElement("QROutX2", actionIndex), pres.GetHexResult(ResultType.QR_OUTPUT_X2, ResultIndex(actionIndex, qrIndex)));
                }, pres.Nav.Undo);
                PageAction unmarkOutX2 = new PageAction(() =>
                {
                    pres.Nav.UnmarkBorder((Border)GetIndexElement("QRInX2Cell", actionIndex));
                    pres.Nav.UnmarkShape((Shape)GetIndexElement("OutputPathX2_1", actionIndex));
                    pres.Nav.UnmarkShape((Shape)GetIndexElement("OutputPathX2_2", actionIndex));
                    pres.Nav.UnmarkBorder((Border)GetIndexElement("QROutX2Cell", actionIndex));
                }, pres.Nav.Undo);
                return new PageAction[] { markOutX2, execOutX2, unmarkOutX2 };
            }

            PageAction[] XORActions(int actionIndex, int qrIndex)
            {
                PageAction markXOR = new PageAction(() =>
                {
                    pres.Nav.MarkBorder((Border)GetIndexElement("QRInX3Cell", actionIndex));
                    pres.Nav.MarkBorder((Border)GetIndexElement("QROutX1Cell", actionIndex));
                    pres.Nav.MarkBorder((Border)GetIndexElement("QRXORCell", actionIndex));
                    pres.Nav.MarkShape((Shape)GetIndexElement("OutputPathX3_1", actionIndex));
                    pres.Nav.MarkShape((Shape)GetIndexElement("XORInputPathX1", actionIndex));
                    pres.Nav.MarkShape((Shape)GetIndexElement("XORCircle", actionIndex));
                }, pres.Nav.Undo);
                PageAction execXOR = new PageAction(() =>
                {
                    pres.Nav.Replace((TextBox)GetIndexElement("QRXOR", actionIndex), pres.GetHexResult(ResultType.QR_XOR, ResultIndex(actionIndex, qrIndex)));
                }, pres.Nav.Undo);
                PageAction unmarkXOR = new PageAction(() =>
                {
                    pres.Nav.UnmarkBorder((Border)GetIndexElement("QRInX3Cell", actionIndex));
                    pres.Nav.UnmarkBorder((Border)GetIndexElement("QROutX1Cell", actionIndex));
                    pres.Nav.UnmarkBorder((Border)GetIndexElement("QRXORCell", actionIndex));
                    pres.Nav.UnmarkShape((Shape)GetIndexElement("OutputPathX3_1", actionIndex));
                    pres.Nav.UnmarkShape((Shape)GetIndexElement("XORInputPathX1", actionIndex));
                    pres.Nav.UnmarkShape((Shape)GetIndexElement("XORCircle", actionIndex));
                }, pres.Nav.Undo);
                return new PageAction[] { markXOR, execXOR, unmarkXOR };
            }

            PageAction[] ShiftActions(int actionIndex, int qrIndex)
            {
                PageAction markShift = new PageAction(() =>
                {
                    pres.Nav.MarkBorder((Border)GetIndexElement("QROutX3Cell", actionIndex));
                    pres.Nav.MarkBorder((Border)GetIndexElement("QRXORCell", actionIndex));
                    pres.Nav.MarkShape((Shape)GetIndexElement("OutputPathX3_2", actionIndex));
                    pres.Nav.MarkShape((Shape)GetIndexElement("OutputPathX3_3", actionIndex));
                    pres.Nav.MarkShape((Shape)GetIndexElement("ShiftCircle", actionIndex));
                }, pres.Nav.Undo);
                PageAction execShift = new PageAction(() =>
                {
                    pres.Nav.Replace((TextBox)GetIndexElement("QROutX3", actionIndex), pres.GetHexResult(ResultType.QR_OUTPUT_X3, ResultIndex(actionIndex, qrIndex)));
                }, pres.Nav.Undo);
                PageAction unmarkShift = new PageAction(() =>
                {
                    pres.Nav.UnmarkBorder((Border)GetIndexElement("QROutX3Cell", actionIndex));
                    pres.Nav.UnmarkBorder((Border)GetIndexElement("QRXORCell", actionIndex));
                    pres.Nav.UnmarkShape((Shape)GetIndexElement("OutputPathX3_2", actionIndex));
                    pres.Nav.UnmarkShape((Shape)GetIndexElement("OutputPathX3_3", actionIndex));
                    pres.Nav.UnmarkShape((Shape)GetIndexElement("ShiftCircle", actionIndex));
                }, pres.Nav.Undo);
                return new PageAction[] { markShift, execShift, unmarkShift };
            }

            PageAction[] X3OutActions(int actionIndex, int qrIndex)
            {
                PageAction[] xorActions = XORActions(actionIndex, qrIndex);
                PageAction[] shiftActions = ShiftActions(actionIndex, qrIndex);
                PageAction[] actions = new PageAction[xorActions.Length + shiftActions.Length];
                xorActions.CopyTo(actions, 0);
                shiftActions.CopyTo(actions, xorActions.Length);
                return actions;
            }

            PageAction[] QRExecActions(int actionIndex, int qrIndex)
            {
                List<PageAction> actions = new List<PageAction>();
                PageAction[] x1OutActions = X1OutActions(actionIndex, qrIndex);
                PageAction[] x2OutActions = X2OutActions(actionIndex, qrIndex);
                PageAction[] x3OutActions = X3OutActions(actionIndex, qrIndex);
                actions.AddRange(x1OutActions);
                actions.AddRange(x2OutActions);
                actions.AddRange(x3OutActions);
                return actions.ToArray();
            }

            PageAction[] CopyToQRDetailInput(Border[] b, int actionIndex)
            {
                return pres.Nav.CopyActions(
                    b,
                    new Shape[] { (Shape)GetIndexElement("QRInX1Path", actionIndex), (Shape)GetIndexElement("QRInX2Path", actionIndex), (Shape)GetIndexElement("QRInX3Path", actionIndex) },
                    new Border[] { (Border)GetIndexElement("QRInX1Cell", actionIndex), (Border)GetIndexElement("QRInX2Cell", actionIndex), (Border)GetIndexElement("QRInX3Cell", actionIndex) }
                    );
            }

            Border GetStateCell(int stateIndex)
            {
                return (Border)GetIndexElement("UIKeystreamBlockGenCell", stateIndex);
            }

            int[] GetStateIndices(int qrIndex)
            {
                switch (((qrIndex - 1) % 8) + 1)
                {
                    case 1:
                        return new int[] { 0, 4, 8, 12 };
                    case 2:
                        return new int[] { 1, 5, 9, 13 };
                    case 3:
                        return new int[] { 2, 6, 10, 14 };
                    case 4:
                        return new int[] { 3, 7, 11, 15 };
                    case 5:
                        return new int[] { 0, 5, 10, 15 };
                    case 6:
                        return new int[] { 1, 6, 11, 12 };
                    case 7:
                        return new int[] { 2, 7, 8, 13 };
                    case 8:
                        return new int[] { 3, 4, 9, 14 };
                    default:
                        Debug.Assert(false, string.Format("No state indices found for round", qrIndex));
                        return new int[0];
                }
            }
            PageAction[] CopyFromStateTOQRInputActions(int qrIndex)
            {
                int[] stateIndices = GetStateIndices(qrIndex);
                int i = stateIndices[0];
                int j = stateIndices[1];
                int k = stateIndices[2];
                int l = stateIndices[3];
                Border[] stateCells = new Border[] { GetStateCell(i), GetStateCell(j), GetStateCell(k), GetStateCell(l) };
                PageAction[] copyActions = pres.Nav.CopyActions(stateCells, new Border[] { pres.QRInACell, pres.QRInBCell, pres.QRInCCell, pres.QRInDCell });
                int round = (int)Math.Floor((double)(qrIndex - 1) / 4) + 1;
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
                    ((TextBox)GetIndexElement("ArrowQRRound", qrPrevArrowIndex)).Visibility = Visibility.Hidden;
                    ((TextBox)GetIndexElement("ArrowQRRound", qrArrowIndex)).Visibility = Visibility.Visible;
                }, () =>
                {
                    if (qrIndex > 1)
                    {
                        (int qrArrowIndex, int qrPrevArrowIndex) = calculateQRArrowIndex(qrIndex);
                        ((TextBox)GetIndexElement("ArrowQRRound", qrPrevArrowIndex)).Visibility = Visibility.Visible;
                        ((TextBox)GetIndexElement("ArrowQRRound", qrArrowIndex)).Visibility = Visibility.Hidden;
                    }
                });
                copyActions[0].Add(updateQRArrow);
                copyActions[0].AddLabel(string.Format("_QR_ACTION_LABEL_{0}_{1}", ((qrIndex - 1) % 4) + 1, round));
                return copyActions;
            }

            PageAction[] ReplaceStateEntriesWithQROutput(int qrIndex)
            {
                int[] stateIndices = GetStateIndices(qrIndex);
                int i = stateIndices[0];
                int j = stateIndices[1];
                int k = stateIndices[2];
                int l = stateIndices[3];
                Border[] stateCells = new Border[] { GetStateCell(i), GetStateCell(j), GetStateCell(k), GetStateCell(l) };
                return pres.Nav.CopyActions(new Border[] { pres.QROutACell, pres.QROutBCell, pres.QROutCCell, pres.QROutDCell }, stateCells, true);
            }

            PageAction ClearQRDetail()
            {
                return new PageAction(() =>
                {
                    pres.Nav.Clear(pres.QRInA);
                    pres.Nav.Clear(pres.QRInB, pres.QRInC, pres.QRInD);
                    pres.Nav.Clear(pres.QROutA, pres.QROutB, pres.QROutC, pres.QROutD);
                    for (int i = 1; i <= 4; ++i)
                    {
                        pres.Nav.Clear((TextBox)GetIndexElement("QRInX1", i));
                        pres.Nav.Clear((TextBox)GetIndexElement("QRInX2", i));
                        pres.Nav.Clear((TextBox)GetIndexElement("QRInX3", i));
                        pres.Nav.Clear((TextBox)GetIndexElement("QROutX1", i));
                        pres.Nav.Clear((TextBox)GetIndexElement("QROutX2", i));
                        pres.Nav.Clear((TextBox)GetIndexElement("QROutX3", i));
                        pres.Nav.Clear((TextBox)GetIndexElement("QRXOR", i));
                    }
                }, pres.Nav.Undo);
            }

            Page KeystreamBlockGenPage()
            {
                Page p = new Page(pres.UIKeystreamBlockGenPage);
                bool versionIsDJB = pres.Version == ChaCha.Version.DJB;
                PageAction initAction = new PageAction(() =>
                {
                    pres.Nav.Replace(pres.UIKeystreamBlockGen0, pres.ConstantsLittleEndian[0]);
                    pres.Nav.Replace(pres.UIKeystreamBlockGen1, pres.ConstantsLittleEndian[1]);
                    pres.Nav.Replace(pres.UIKeystreamBlockGen2, pres.ConstantsLittleEndian[2]);
                    pres.Nav.Replace(pres.UIKeystreamBlockGen3, pres.ConstantsLittleEndian[3]);
                    pres.Nav.Replace(pres.UIKeystreamBlockGen4, pres.KeyLittleEndian[0]);
                    pres.Nav.Replace(pres.UIKeystreamBlockGen5, pres.KeyLittleEndian[1]);
                    pres.Nav.Replace(pres.UIKeystreamBlockGen6, pres.KeyLittleEndian[2]);
                    pres.Nav.Replace(pres.UIKeystreamBlockGen7, pres.KeyLittleEndian[3]);
                    pres.Nav.Replace(pres.UIKeystreamBlockGen8, pres.KeyLittleEndian[pres.InputKey.Length == 32 ? 4 : 0]);
                    pres.Nav.Replace(pres.UIKeystreamBlockGen9, pres.KeyLittleEndian[pres.InputKey.Length == 32 ? 5 : 1]);
                    pres.Nav.Replace(pres.UIKeystreamBlockGen10, pres.KeyLittleEndian[pres.InputKey.Length == 32 ? 6 : 2]);
                    pres.Nav.Replace(pres.UIKeystreamBlockGen11, pres.KeyLittleEndian[pres.InputKey.Length == 32 ? 7 : 3]);
                    pres.Nav.Replace(pres.UIKeystreamBlockGen12, pres.InitialCounterLittleEndian[0]);
                    if (versionIsDJB)
                    {
                        pres.Nav.Replace(pres.UIKeystreamBlockGen13, pres.InitialCounterLittleEndian[1]);
                    }
                    else
                    {
                        pres.Nav.Replace(pres.UIKeystreamBlockGen13, pres.IVLittleEndian[0]);
                    }
                    pres.Nav.Replace(pres.UIKeystreamBlockGen14, pres.IVLittleEndian[versionIsDJB ? 0 : 1]);
                    pres.Nav.Replace(pres.UIKeystreamBlockGen15, pres.IVLittleEndian[versionIsDJB ? 1 : 2]);
                }, pres.Nav.Undo);
                p.AddInitAction(initAction);
                PageAction generalDescriptionAction = new PageAction(() =>
                {
                    string desc = "To generate a keystream block, we apply the ChaCha Hash function to the state. "
                        + "The ChaCha hash function consists of X rounds. One round applies the quarterround function four times hence the name \"quarterround\". The quarterround function takes in 4 state entries and modifies them.";
                    pres.Nav.AddBold(pres.UIKeystreamBlockGenStepDescription, desc);
                }, pres.Nav.Undo);
                PageAction firstColumnRoundDescriptionAction = new PageAction(() =>
                {
                    string desc = "The first round consists of 4 so called column rounds since we will first select all entries in a column as the input to the quarterround function. ";
                    pres.Nav.UnboldLast(pres.UIKeystreamBlockGenStepDescription);
                    pres.Nav.AddBold(pres.UIKeystreamBlockGenStepDescription, desc);
                }, pres.Nav.Undo);
                p.AddAction(generalDescriptionAction);
                p.AddAction(firstColumnRoundDescriptionAction);

                for (int qrIndex = 1; qrIndex <= pres.Rounds * 4; ++qrIndex)
                {
                    p.AddAction(CopyFromStateTOQRInputActions(qrIndex));
                    p.AddAction(CopyToQRDetailInput(new Border[] { pres.QRInACell, pres.QRInBCell, pres.QRInDCell }, 1));
                    p.AddAction(QRExecActions(1, qrIndex));
                    p.AddAction(CopyToQRDetailInput(new Border[] { pres.QRInCCell, (Border)GetIndexElement("QROutX3Cell", 1), (Border)GetIndexElement("QROutX2Cell", 1) }, 2));
                    p.AddAction(QRExecActions(2, qrIndex));
                    p.AddAction(CopyToQRDetailInput(new Border[] { (Border)GetIndexElement("QROutX1Cell", 1), (Border)GetIndexElement("QROutX3Cell", 2), (Border)GetIndexElement("QROutX2Cell", 2) }, 3));
                    p.AddAction(QRExecActions(3, qrIndex));
                    p.AddAction(CopyToQRDetailInput(new Border[] { (Border)GetIndexElement("QROutX1Cell", 2), (Border)GetIndexElement("QROutX3Cell", 3), (Border)GetIndexElement("QROutX2Cell", 3) }, 4));
                    p.AddAction(QRExecActions(4, qrIndex));
                    p.AddAction(QROutputActions());
                    p.AddAction(ReplaceStateEntriesWithQROutput(qrIndex));
                    p.AddAction(ClearQRDetail());
                }

                return p;
            }

            return KeystreamBlockGenPage();
        }
        
    }
}