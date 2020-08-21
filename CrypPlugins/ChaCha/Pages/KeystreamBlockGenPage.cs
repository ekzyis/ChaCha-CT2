﻿using System.ComponentModel;
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
                Add(QRInA, GetHexResult(ResultType.QR_INPUT_A, index - 1));
                Add(QRInB, GetHexResult(ResultType.QR_INPUT_B, index - 1));
                Add(QRInC, GetHexResult(ResultType.QR_INPUT_C, index - 1));
                Add(QRInD, GetHexResult(ResultType.QR_INPUT_D, index - 1));
            }, Undo);
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
                MarkBorder((Border)GetIndexElement("QRInX1Cell", actionIndex));
                MarkBorder((Border)GetIndexElement("QRInX2Cell", actionIndex));
                MarkShape((Shape)GetIndexElement("AddInputPathX1", actionIndex));
                MarkShape((Shape)GetIndexElement("AddInputPathX2", actionIndex));
                MarkShape((Shape)GetIndexElement("OutputPathX1", actionIndex));
                MarkShape((Shape)GetIndexElement("OutputPathX2_1", actionIndex));
                MarkShape((Shape)GetIndexElement("AddCircle", actionIndex));
                MarkBorder((Border)GetIndexElement("QROutX1Cell", actionIndex));
            }, Undo);
            PageAction execOutX1 = new PageAction(() =>
            {
                Add((TextBlock)GetIndexElement("QROutX1", actionIndex), GetHexResult(ResultType.QR_ADD_X1_X2, ResultIndex(actionIndex, qrIndex)));
            }, Undo);
            PageAction unmarkOutX1 = new PageAction(() =>
            {
                UnmarkBorder((Border)GetIndexElement("QRInX1Cell", actionIndex));
                UnmarkBorder((Border)GetIndexElement("QRInX2Cell", actionIndex));
                UnmarkShape((Shape)GetIndexElement("AddInputPathX1", actionIndex));
                UnmarkShape((Shape)GetIndexElement("AddInputPathX2", actionIndex));
                UnmarkShape((Shape)GetIndexElement("OutputPathX1", actionIndex));
                UnmarkShape((Shape)GetIndexElement("OutputPathX2_1", actionIndex));
                UnmarkShape((Shape)GetIndexElement("AddCircle", actionIndex));
                UnmarkBorder((Border)GetIndexElement("QROutX1Cell", actionIndex));
            }, Undo);
            return new PageAction[] { markOutX1, execOutX1, unmarkOutX1 };
        }

        private PageAction[] X2OutActions(int actionIndex, int qrIndex)
        {
            PageAction markOutX2 = new PageAction(() =>
            {
                MarkBorder((Border)GetIndexElement("QRInX2Cell", actionIndex));
                MarkShape((Shape)GetIndexElement("OutputPathX2_1", actionIndex));
                MarkShape((Shape)GetIndexElement("OutputPathX2_2", actionIndex));
                MarkBorder((Border)GetIndexElement("QROutX2Cell", actionIndex));
            }, Undo);
            PageAction execOutX2 = new PageAction(() =>
            {
                Add((TextBlock)GetIndexElement("QROutX2", actionIndex), GetHexResult(ResultType.QR_OUTPUT_X2, ResultIndex(actionIndex, qrIndex)));
            }, Undo);
            PageAction unmarkOutX2 = new PageAction(() =>
            {
                UnmarkBorder((Border)GetIndexElement("QRInX2Cell", actionIndex));
                UnmarkShape((Shape)GetIndexElement("OutputPathX2_1", actionIndex));
                UnmarkShape((Shape)GetIndexElement("OutputPathX2_2", actionIndex));
                UnmarkBorder((Border)GetIndexElement("QROutX2Cell", actionIndex));
            }, Undo);
            return new PageAction[] { markOutX2, execOutX2, unmarkOutX2 };
        }

        private PageAction[] XORActions(int actionIndex, int qrIndex)
        {
            PageAction markXOR = new PageAction(() =>
            {
                MarkBorder((Border)GetIndexElement("QRInX3Cell", actionIndex));
                MarkBorder((Border)GetIndexElement("QROutX1Cell", actionIndex));
                MarkBorder((Border)GetIndexElement("QRXORCell", actionIndex));
                MarkShape((Shape)GetIndexElement("OutputPathX3_1", actionIndex));
                MarkShape((Shape)GetIndexElement("XORInputPathX1", actionIndex));
                MarkShape((Shape)GetIndexElement("XORCircle", actionIndex));
            }, Undo);
            PageAction execXOR = new PageAction(() =>
            {
                Add((TextBlock)GetIndexElement("QRXOR", actionIndex), GetHexResult(ResultType.QR_XOR, ResultIndex(actionIndex, qrIndex)));
            }, Undo);
            PageAction unmarkXOR = new PageAction(() =>
            {
                UnmarkBorder((Border)GetIndexElement("QRInX3Cell", actionIndex));
                UnmarkBorder((Border)GetIndexElement("QROutX1Cell", actionIndex));
                UnmarkBorder((Border)GetIndexElement("QRXORCell", actionIndex));
                UnmarkShape((Shape)GetIndexElement("OutputPathX3_1", actionIndex));
                UnmarkShape((Shape)GetIndexElement("XORInputPathX1", actionIndex));
                UnmarkShape((Shape)GetIndexElement("XORCircle", actionIndex));
            }, Undo);
            return new PageAction[] { markXOR, execXOR, unmarkXOR };
        }

        private PageAction[] ShiftActions(int actionIndex, int qrIndex)
        {
            PageAction markShift = new PageAction(() =>
            {
                MarkBorder((Border)GetIndexElement("QROutX3Cell", actionIndex));
                MarkBorder((Border)GetIndexElement("QRXORCell", actionIndex));
                MarkShape((Shape)GetIndexElement("OutputPathX3_2", actionIndex));
                MarkShape((Shape)GetIndexElement("OutputPathX3_3", actionIndex));
                MarkShape((Shape)GetIndexElement("ShiftCircle", actionIndex));
            }, Undo);
            PageAction execShift = new PageAction(() =>
            {
                Add((TextBlock)GetIndexElement("QROutX3", actionIndex), GetHexResult(ResultType.QR_OUTPUT_X3, ResultIndex(actionIndex, qrIndex)));
            }, Undo);
            PageAction unmarkShift = new PageAction(() =>
            {
                UnmarkBorder((Border)GetIndexElement("QROutX3Cell", actionIndex));
                UnmarkBorder((Border)GetIndexElement("QRXORCell", actionIndex));
                UnmarkShape((Shape)GetIndexElement("OutputPathX3_2", actionIndex));
                UnmarkShape((Shape)GetIndexElement("OutputPathX3_3", actionIndex));
                UnmarkShape((Shape)GetIndexElement("ShiftCircle", actionIndex));
            }, Undo);
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
                    ReplaceLast(CurrentRound.Inlines, round.ToString());
                }, () =>
                {
                    string text = "";
                    if(round > 1)
                    {
                        text = (round - 1).ToString();
                    }
                    ReplaceLast(CurrentRound.Inlines, text);
                });
                copyActions[0].Add(updateRoundCount);
            }
            PageAction updateQRArrow = new PageAction(() =>
            {
                if (qrIndex > 1)
                {
                    ((TextBlock)GetIndexElement("ArrowQRRound", qrIndex - 1)).Visibility = Visibility.Hidden;
                }
                ((TextBlock)GetIndexElement("ArrowQRRound", qrIndex)).Visibility = Visibility.Visible;
            }, () =>
            {
                if (qrIndex > 1)
                {
                    ((TextBlock)GetIndexElement("ArrowQRRound", qrIndex - 1)).Visibility = Visibility.Visible;
                }
                ((TextBlock)GetIndexElement("ArrowQRRound", qrIndex)).Visibility = Visibility.Hidden;
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
                Clear(QRInA, QRInB, QRInC, QRInD);
                Clear(QROutA, QROutB, QROutC, QROutD);
                for(int i = 1; i <= 4; ++i)
                {
                    Clear((TextBlock)GetIndexElement("QRInX1", i));
                    Clear((TextBlock)GetIndexElement("QRInX2", i));
                    Clear((TextBlock)GetIndexElement("QRInX3", i));
                    Clear((TextBlock)GetIndexElement("QROutX1", i));
                    Clear((TextBlock)GetIndexElement("QROutX2", i));
                    Clear((TextBlock)GetIndexElement("QROutX3", i));
                    Clear((TextBlock)GetIndexElement("QRXOR", i));
                }
            }, Undo);
        }

        private Page KeystreamBlockGenPage()
        {
            Page p = new Page(UIKeystreamBlockGenPage);
            bool versionIsDJB = Version == ChaCha.Version.DJB;
            PageAction initAction = new PageAction(() =>
            {
                Add(UIKeystreamBlockGen0, new Run(ConstantsLittleEndian.Replace(" ", "").Substring(0, 8)));
                Add(UIKeystreamBlockGen1, new Run(ConstantsLittleEndian.Replace(" ", "").Substring(8, 8)));
                Add(UIKeystreamBlockGen2, new Run(ConstantsLittleEndian.Replace(" ", "").Substring(16, 8)));
                Add(UIKeystreamBlockGen3, new Run(ConstantsLittleEndian.Replace(" ", "").Substring(24, 8)));
                Add(UIKeystreamBlockGen4, new Run(KeyLittleEndian.Replace(" ", "").Substring(0, 8)));
                Add(UIKeystreamBlockGen5, new Run(KeyLittleEndian.Replace(" ", "").Substring(8, 8)));
                Add(UIKeystreamBlockGen6, new Run(KeyLittleEndian.Replace(" ", "").Substring(16, 8)));
                Add(UIKeystreamBlockGen7, new Run(KeyLittleEndian.Replace(" ", "").Substring(24, 8)));
                Add(UIKeystreamBlockGen8, new Run(KeyLittleEndian.Replace(" ", "").Substring(InputKey.Length == 16 ? 0 : 32, 8)));
                Add(UIKeystreamBlockGen9, new Run(KeyLittleEndian.Replace(" ", "").Substring(InputKey.Length == 16 ? 8 : 40, 8)));
                Add(UIKeystreamBlockGen10, new Run(KeyLittleEndian.Replace(" ", "").Substring(InputKey.Length == 16 ? 16 : 48, 8)));
                Add(UIKeystreamBlockGen11, new Run(KeyLittleEndian.Replace(" ", "").Substring(InputKey.Length == 16 ? 24 : 56, 8)));
                Add(UIKeystreamBlockGen12, new Run(InitialCounterLittleEndian.Replace(" ", "").Substring(0, 8)));
                if (versionIsDJB)
                {
                    Add(UIKeystreamBlockGen13, new Run(InitialCounterLittleEndian.Replace(" ", "").Substring(8, 8)));
                }
                else
                {
                    Add(UIKeystreamBlockGen13, new Run(IVLittleEndian.Replace(" ", "").Substring(0, 8)));
                }
                Add(UIKeystreamBlockGen14, new Run(IVLittleEndian.Replace(" ", "").Substring(versionIsDJB ? 0 : 8, 8)));
                Add(UIKeystreamBlockGen15, new Run(IVLittleEndian.Replace(" ", "").Substring(versionIsDJB ? 8 : 16, 8)));
            }, Undo);
            p.AddInitAction(initAction);
            PageAction generalDescriptionAction = new PageAction(() =>
            {
                string desc = "To generate a keystream block, we apply the ChaCha Hash function to the state. "
                    + "The ChaCha hash function consists of X rounds. One round applies the quarterround function four times hence the name \"quarterround\". The quarterround function takes in 4 state entries and modifies them.";
                Add(UIKeystreamBlockGenStepDescription, MakeBold(new Run(desc)));
            }, Undo);
            PageAction firstColumnRoundDescriptionAction = new PageAction(() =>
            {
                string desc = "The first round consists of 4 so called column rounds since we will first select all entries in a column as the input to the quarterround function. ";
                UnboldLast(UIKeystreamBlockGenStepDescription);
                Add(UIKeystreamBlockGenStepDescription, MakeBold(new Run(desc)));
            }, Undo);
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