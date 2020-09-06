using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Documents;
using System.Diagnostics;
using System;

namespace Cryptool.Plugins.ChaCha
{
    public partial class ChaChaPresentation : UserControl, INotifyPropertyChanged
    {
        private int ResultIndex(int actionIndex, int qrIndex)
        {
            return (qrIndex - 1) * 4 + (actionIndex - 1);
        }
        private object GetIndexElement(string nameId, int index)
        {
            return FindName(string.Format("{0}_{1}", nameId, index));
        }
        private PageAction CreateQRInputAction(int index)
        {
            PageAction insertQRInput = new PageAction(() =>
            {
                nav.Replace(QRInA, GetHexResult(ResultType.QR_INPUT_A, index - 1));
                nav.Replace(QRInB, GetHexResult(ResultType.QR_INPUT_B, index - 1));
                nav.Replace(QRInC, GetHexResult(ResultType.QR_INPUT_C, index - 1));
                nav.Replace(QRInD, GetHexResult(ResultType.QR_INPUT_D, index - 1));
            }, nav.Undo);
            return insertQRInput;
        }

        private PageAction[] QROutputActions()
        {
            return CopyActions(
                new Border[] { (Border)GetIndexElement("QROutX1Cell", 3), (Border)GetIndexElement("QROutX3Cell", 4), (Border)GetIndexElement("QROutX1Cell", 4), (Border)GetIndexElement("QROutX2Cell", 4) },
                new Shape[] { QROutAPath, QROutBPath, QROutCPath, QROutDPath },
                new Border[] { QROutACell, QROutBCell, QROutCCell, QROutDCell }
                );
        }

        private PageAction[] X1OutActions(int actionIndex, int qrIndex)
        {
            PageAction markOutX1 = new PageAction(() =>
            {
                nav.MarkBorder((Border)GetIndexElement("QRInX1Cell", actionIndex));
                nav.MarkBorder((Border)GetIndexElement("QRInX2Cell", actionIndex));
                nav.MarkShape((Shape)GetIndexElement("AddInputPathX1", actionIndex));
                nav.MarkShape((Shape)GetIndexElement("AddInputPathX2", actionIndex));
                nav.MarkShape((Shape)GetIndexElement("OutputPathX1", actionIndex));
                nav.MarkShape((Shape)GetIndexElement("OutputPathX2_1", actionIndex));
                nav.MarkShape((Shape)GetIndexElement("AddCircle", actionIndex));
                nav.MarkBorder((Border)GetIndexElement("QROutX1Cell", actionIndex));
            }, nav.Undo);
            PageAction execOutX1 = new PageAction(() =>
            {
                nav.Replace((TextBox)GetIndexElement("QROutX1", actionIndex), GetHexResult(ResultType.QR_ADD_X1_X2, ResultIndex(actionIndex, qrIndex)));
            }, nav.Undo);
            PageAction unmarkOutX1 = new PageAction(() =>
            {
                nav.UnmarkBorder((Border)GetIndexElement("QRInX1Cell", actionIndex));
                nav.UnmarkBorder((Border)GetIndexElement("QRInX2Cell", actionIndex));
                nav.UnmarkShape((Shape)GetIndexElement("AddInputPathX1", actionIndex));
                nav.UnmarkShape((Shape)GetIndexElement("AddInputPathX2", actionIndex));
                nav.UnmarkShape((Shape)GetIndexElement("OutputPathX1", actionIndex));
                nav.UnmarkShape((Shape)GetIndexElement("OutputPathX2_1", actionIndex));
                nav.UnmarkShape((Shape)GetIndexElement("AddCircle", actionIndex));
                nav.UnmarkBorder((Border)GetIndexElement("QROutX1Cell", actionIndex));
            }, nav.Undo);
            return new PageAction[] { markOutX1, execOutX1, unmarkOutX1 };
        }

        private PageAction[] X2OutActions(int actionIndex, int qrIndex)
        {
            PageAction markOutX2 = new PageAction(() =>
            {
                nav.MarkBorder((Border)GetIndexElement("QRInX2Cell", actionIndex));
                nav.MarkShape((Shape)GetIndexElement("OutputPathX2_1", actionIndex));
                nav.MarkShape((Shape)GetIndexElement("OutputPathX2_2", actionIndex));
                nav.MarkBorder((Border)GetIndexElement("QROutX2Cell", actionIndex));
            }, nav.Undo);
            PageAction execOutX2 = new PageAction(() =>
            {
                nav.Replace((TextBox)GetIndexElement("QROutX2", actionIndex), GetHexResult(ResultType.QR_OUTPUT_X2, ResultIndex(actionIndex, qrIndex)));
            }, nav.Undo);
            PageAction unmarkOutX2 = new PageAction(() =>
            {
                nav.UnmarkBorder((Border)GetIndexElement("QRInX2Cell", actionIndex));
                nav.UnmarkShape((Shape)GetIndexElement("OutputPathX2_1", actionIndex));
                nav.UnmarkShape((Shape)GetIndexElement("OutputPathX2_2", actionIndex));
                nav.UnmarkBorder((Border)GetIndexElement("QROutX2Cell", actionIndex));
            }, nav.Undo);
            return new PageAction[] { markOutX2, execOutX2, unmarkOutX2 };
        }

        private PageAction[] XORActions(int actionIndex, int qrIndex)
        {
            PageAction markXOR = new PageAction(() =>
            {
                nav.MarkBorder((Border)GetIndexElement("QRInX3Cell", actionIndex));
                nav.MarkBorder((Border)GetIndexElement("QROutX1Cell", actionIndex));
                nav.MarkBorder((Border)GetIndexElement("QRXORCell", actionIndex));
                nav.MarkShape((Shape)GetIndexElement("OutputPathX3_1", actionIndex));
                nav.MarkShape((Shape)GetIndexElement("XORInputPathX1", actionIndex));
                nav.MarkShape((Shape)GetIndexElement("XORCircle", actionIndex));
            }, nav.Undo);
            PageAction execXOR = new PageAction(() =>
            {
                nav.Replace((TextBox)GetIndexElement("QRXOR", actionIndex), GetHexResult(ResultType.QR_XOR, ResultIndex(actionIndex, qrIndex)));
            }, nav.Undo);
            PageAction unmarkXOR = new PageAction(() =>
            {
                nav.UnmarkBorder((Border)GetIndexElement("QRInX3Cell", actionIndex));
                nav.UnmarkBorder((Border)GetIndexElement("QROutX1Cell", actionIndex));
                nav.UnmarkBorder((Border)GetIndexElement("QRXORCell", actionIndex));
                nav.UnmarkShape((Shape)GetIndexElement("OutputPathX3_1", actionIndex));
                nav.UnmarkShape((Shape)GetIndexElement("XORInputPathX1", actionIndex));
                nav.UnmarkShape((Shape)GetIndexElement("XORCircle", actionIndex));
            }, nav.Undo);
            return new PageAction[] { markXOR, execXOR, unmarkXOR };
        }

        private PageAction[] ShiftActions(int actionIndex, int qrIndex)
        {
            PageAction markShift = new PageAction(() =>
            {
                nav.MarkBorder((Border)GetIndexElement("QROutX3Cell", actionIndex));
                nav.MarkBorder((Border)GetIndexElement("QRXORCell", actionIndex));
                nav.MarkShape((Shape)GetIndexElement("OutputPathX3_2", actionIndex));
                nav.MarkShape((Shape)GetIndexElement("OutputPathX3_3", actionIndex));
                nav.MarkShape((Shape)GetIndexElement("ShiftCircle", actionIndex));
            }, nav.Undo);
            PageAction execShift = new PageAction(() =>
            {
                nav.Replace((TextBox)GetIndexElement("QROutX3", actionIndex), GetHexResult(ResultType.QR_OUTPUT_X3, ResultIndex(actionIndex, qrIndex)));
            }, nav.Undo);
            PageAction unmarkShift = new PageAction(() =>
            {
                nav.UnmarkBorder((Border)GetIndexElement("QROutX3Cell", actionIndex));
                nav.UnmarkBorder((Border)GetIndexElement("QRXORCell", actionIndex));
                nav.UnmarkShape((Shape)GetIndexElement("OutputPathX3_2", actionIndex));
                nav.UnmarkShape((Shape)GetIndexElement("OutputPathX3_3", actionIndex));
                nav.UnmarkShape((Shape)GetIndexElement("ShiftCircle", actionIndex));
            }, nav.Undo);
            return new PageAction[] { markShift, execShift, unmarkShift };
        }

        private PageAction[] X3OutActions(int actionIndex, int qrIndex)
        {
            PageAction[] xorActions = XORActions(actionIndex, qrIndex);
            PageAction[] shiftActions = ShiftActions(actionIndex, qrIndex);
            PageAction[] actions = new PageAction[xorActions.Length + shiftActions.Length];
            xorActions.CopyTo(actions, 0);
            shiftActions.CopyTo(actions, xorActions.Length);
            return actions;
        }

        private PageAction[] QRExecActions(int actionIndex, int qrIndex)
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

        private PageAction[] CopyToQRDetailInput(Border[] b, int actionIndex)
        {
            return CopyActions(
                b,
                new Shape[] { (Shape)GetIndexElement("QRInX1Path", actionIndex), (Shape)GetIndexElement("QRInX2Path", actionIndex), (Shape)GetIndexElement("QRInX3Path", actionIndex) },
                new Border[] { (Border)GetIndexElement("QRInX1Cell", actionIndex), (Border)GetIndexElement("QRInX2Cell", actionIndex), (Border)GetIndexElement("QRInX3Cell", actionIndex) }
                );
        }

        private Border GetStateCell(int stateIndex)
        {
            return (Border)GetIndexElement("UIKeystreamBlockGenCell", stateIndex);
        }

        private int[] GetStateIndices(int qrIndex)
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
        private PageAction[] CopyFromStateTOQRInputActions(int qrIndex)
        {
            int[] stateIndices = GetStateIndices(qrIndex);
            int i = stateIndices[0];
            int j = stateIndices[1];
            int k = stateIndices[2];
            int l = stateIndices[3];
            Border[] stateCells = new Border[] { GetStateCell(i), GetStateCell(j), GetStateCell(k), GetStateCell(l) };
            PageAction[] copyActions = CopyActions(stateCells, new Border[] { QRInACell, QRInBCell, QRInCCell, QRInDCell });
            int round = (int)Math.Floor((double)(qrIndex - 1) / 4) + 1;
            if ((qrIndex - 1) % 4 == 0)
            {
                // new round begins
                PageAction updateRoundCount = new PageAction(() =>
                {
                    nav.Replace(CurrentRound, round.ToString());
                }, () =>
                {
                    string text = "";
                    if(round > 1)
                    {
                        text = (round - 1).ToString();
                    }
                    nav.Replace(CurrentRound, text);
                });
                copyActions[0].Add(updateRoundCount);
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
                (int qrArrowIndex, int qrPrevArrowIndex)  = calculateQRArrowIndex(qrIndex);
                ((TextBox)GetIndexElement("ArrowQRRound", qrPrevArrowIndex)).Visibility = Visibility.Hidden;
                ((TextBox)GetIndexElement("ArrowQRRound", qrArrowIndex)).Visibility = Visibility.Visible;
            }, () =>
            {
                if(qrIndex > 1)
                {
                    (int qrArrowIndex, int qrPrevArrowIndex) = calculateQRArrowIndex(qrIndex);
                    ((TextBox)GetIndexElement("ArrowQRRound", qrPrevArrowIndex)).Visibility = Visibility.Visible;
                    ((TextBox)GetIndexElement("ArrowQRRound", qrArrowIndex)).Visibility = Visibility.Hidden;
                }
            });
            copyActions[0].Add(updateQRArrow);
            return copyActions;
        }

        private PageAction[] ReplaceStateEntriesWithQROutput(int qrIndex)
        {
            int[] stateIndices = GetStateIndices(qrIndex);
            int i = stateIndices[0];
            int j = stateIndices[1];
            int k = stateIndices[2];
            int l = stateIndices[3];
            Border[] stateCells = new Border[] { GetStateCell(i), GetStateCell(j), GetStateCell(k), GetStateCell(l) };
            return CopyActions(new Border[] { QROutACell, QROutBCell, QROutCCell, QROutDCell }, stateCells, true);
        }

        private PageAction ClearQRDetail()
        {
            return new PageAction(() =>
            {
                nav.Clear(QRInA);
                nav.Clear(QRInB, QRInC, QRInD);
                nav.Clear(QROutA, QROutB, QROutC, QROutD);
                for(int i = 1; i <= 4; ++i)
                {
                    nav.Clear((TextBox)GetIndexElement("QRInX1", i));
                    nav.Clear((TextBox)GetIndexElement("QRInX2", i));
                    nav.Clear((TextBox)GetIndexElement("QRInX3", i));
                    nav.Clear((TextBox)GetIndexElement("QROutX1", i));
                    nav.Clear((TextBox)GetIndexElement("QROutX2", i));
                    nav.Clear((TextBox)GetIndexElement("QROutX3", i));
                    nav.Clear((TextBox)GetIndexElement("QRXOR", i));
                }
            }, nav.Undo);
        }

        private Page KeystreamBlockGenPage()
        {
            Page p = new Page(UIKeystreamBlockGenPage);
            bool versionIsDJB = Version == ChaCha.Version.DJB;
            PageAction initAction = new PageAction(() =>
            {
                nav.Replace(UIKeystreamBlockGen0, ConstantsLittleEndian[0]);
                nav.Replace(UIKeystreamBlockGen1, ConstantsLittleEndian[1]);
                nav.Replace(UIKeystreamBlockGen2, ConstantsLittleEndian[2]);
                nav.Replace(UIKeystreamBlockGen3, ConstantsLittleEndian[3]);
                nav.Replace(UIKeystreamBlockGen4, KeyLittleEndian[0]);
                nav.Replace(UIKeystreamBlockGen5, KeyLittleEndian[1]);
                nav.Replace(UIKeystreamBlockGen6, KeyLittleEndian[2]);
                nav.Replace(UIKeystreamBlockGen7, KeyLittleEndian[3]);
                nav.Replace(UIKeystreamBlockGen8, KeyLittleEndian[InputKey.Length == 32 ? 4 : 0]);
                nav.Replace(UIKeystreamBlockGen9, KeyLittleEndian[InputKey.Length == 32 ? 5 : 1]);
                nav.Replace(UIKeystreamBlockGen10, KeyLittleEndian[InputKey.Length == 32 ? 6 : 2]);
                nav.Replace(UIKeystreamBlockGen11, KeyLittleEndian[InputKey.Length == 32 ? 7 : 3]);
                nav.Replace(UIKeystreamBlockGen12, InitialCounterLittleEndian[0]);
                if (versionIsDJB)
                {
                    nav.Replace(UIKeystreamBlockGen13, InitialCounterLittleEndian[1]);
                }
                else
                {
                    nav.Replace(UIKeystreamBlockGen13, IVLittleEndian[0]);
                }
                nav.Replace(UIKeystreamBlockGen14, IVLittleEndian[versionIsDJB ? 0 : 1]);
                nav.Replace(UIKeystreamBlockGen15, IVLittleEndian[versionIsDJB ? 1 : 2]);
            }, nav.Undo);
            p.AddInitAction(initAction);
            PageAction generalDescriptionAction = new PageAction(() =>
            {
                string desc = "To generate a keystream block, we apply the ChaCha Hash function to the state. "
                    + "The ChaCha hash function consists of X rounds. One round applies the quarterround function four times hence the name \"quarterround\". The quarterround function takes in 4 state entries and modifies them.";
                nav.AddBold(UIKeystreamBlockGenStepDescription, desc);
            }, nav.Undo);
            PageAction firstColumnRoundDescriptionAction = new PageAction(() =>
            {
                string desc = "The first round consists of 4 so called column rounds since we will first select all entries in a column as the input to the quarterround function. ";
                nav.UnboldLast(UIKeystreamBlockGenStepDescription);
                nav.AddBold(UIKeystreamBlockGenStepDescription, desc);
            }, nav.Undo);
            p.AddAction(generalDescriptionAction);
            p.AddAction(firstColumnRoundDescriptionAction);

            for (int qrIndex = 1; qrIndex <= Rounds * 4; ++qrIndex)
            {
                p.AddAction(CopyFromStateTOQRInputActions(qrIndex));
                p.AddAction(CopyToQRDetailInput(new Border[] { QRInACell, QRInBCell, QRInDCell }, 1));
                p.AddAction(QRExecActions(1, qrIndex));
                p.AddAction(CopyToQRDetailInput(new Border[] { QRInCCell, (Border)GetIndexElement("QROutX3Cell", 1), (Border)GetIndexElement("QROutX2Cell", 1) }, 2));
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
    }
}