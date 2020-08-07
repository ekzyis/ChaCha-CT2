using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Documents;

namespace Cryptool.Plugins.ChaCha
{
    public partial class ChaChaPresentation : UserControl, INotifyPropertyChanged
    {
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

        private PageAction[] X1OutActions(int index)
        {
            PageAction markOutX1 = new PageAction(() =>
            {
                MarkBorder((Border)GetIndexElement("QRInX1Cell", index));
                MarkBorder((Border)GetIndexElement("QRInX2Cell", index));
                MarkShape((Shape)GetIndexElement("AddInputPathX1", index));
                MarkShape((Shape)GetIndexElement("AddInputPathX2", index));
                MarkShape((Shape)GetIndexElement("OutputPathX1", index));
                MarkShape((Shape)GetIndexElement("OutputPathX2_1", index));
                MarkShape((Shape)GetIndexElement("AddCircle", index));
                MarkBorder((Border)GetIndexElement("QROutX1Cell", index));
            }, Undo);
            PageAction execOutX1 = new PageAction(() =>
            {
                Add((TextBlock)GetIndexElement("QROutX1", index), GetHexResult(ResultType.QR_ADD_X1_X2, index - 1));
            }, Undo);
            PageAction unmarkOutX1 = new PageAction(() =>
            {
                UnmarkBorder((Border)GetIndexElement("QRInX1Cell", index));
                UnmarkBorder((Border)GetIndexElement("QRInX2Cell", index));
                UnmarkShape((Shape)GetIndexElement("AddInputPathX1", index));
                UnmarkShape((Shape)GetIndexElement("AddInputPathX2", index));
                UnmarkShape((Shape)GetIndexElement("OutputPathX1", index));
                UnmarkShape((Shape)GetIndexElement("OutputPathX2_1", index));
                UnmarkShape((Shape)GetIndexElement("AddCircle", index));
                UnmarkBorder((Border)GetIndexElement("QROutX1Cell", index));
            }, Undo);
            return new PageAction[] { markOutX1, execOutX1, unmarkOutX1 };
        }

        private PageAction[] X2OutActions(int index)
        {
            PageAction markOutX2 = new PageAction(() =>
            {
                MarkBorder((Border)GetIndexElement("QRInX2Cell", index));
                MarkShape((Shape)GetIndexElement("OutputPathX2_1", index));
                MarkShape((Shape)GetIndexElement("OutputPathX2_2", index));
                MarkBorder((Border)GetIndexElement("QROutX2Cell", index));
            }, Undo);
            PageAction execOutX2 = new PageAction(() =>
            {
                Add((TextBlock)GetIndexElement("QROutX2", index), GetHexResult(ResultType.QR_OUTPUT_X2, index - 1));
            }, Undo);
            PageAction unmarkOutX2 = new PageAction(() =>
            {
                UnmarkBorder((Border)GetIndexElement("QRInX2Cell", index));
                UnmarkShape((Shape)GetIndexElement("OutputPathX2_1", index));
                UnmarkShape((Shape)GetIndexElement("OutputPathX2_2", index));
                UnmarkBorder((Border)GetIndexElement("QROutX2Cell", index));
            }, Undo);
            return new PageAction[] { markOutX2, execOutX2, unmarkOutX2 };
        }

        private PageAction[] XORActions(int index)
        {
            PageAction markXOR = new PageAction(() =>
            {
                MarkBorder((Border)GetIndexElement("QRInX3Cell", index));
                MarkBorder((Border)GetIndexElement("QROutX1Cell", index));
                MarkBorder((Border)GetIndexElement("QRXORCell", index));
                MarkShape((Shape)GetIndexElement("OutputPathX3_1", index));
                MarkShape((Shape)GetIndexElement("XORInputPathX1", index));
                MarkShape((Shape)GetIndexElement("XORCircle", index));
            }, Undo);
            PageAction execXOR = new PageAction(() =>
            {
                Add((TextBlock)GetIndexElement("QRXOR", index), GetHexResult(ResultType.QR_XOR, index - 1));
            }, Undo);
            PageAction unmarkXOR = new PageAction(() =>
            {
                UnmarkBorder((Border)GetIndexElement("QRInX3Cell", index));
                UnmarkBorder((Border)GetIndexElement("QROutX1Cell", index));
                UnmarkBorder((Border)GetIndexElement("QRXORCell", index));
                UnmarkShape((Shape)GetIndexElement("OutputPathX3_1", index));
                UnmarkShape((Shape)GetIndexElement("XORInputPathX1", index));
                UnmarkShape((Shape)GetIndexElement("XORCircle", index));
            }, Undo);
            return new PageAction[] { markXOR, execXOR, unmarkXOR };
        }

        private PageAction[] ShiftActions(int index)
        {
            PageAction markShift = new PageAction(() =>
            {
                MarkBorder((Border)GetIndexElement("QROutX3Cell", index));
                MarkBorder((Border)GetIndexElement("QRXORCell", index));
                MarkShape((Shape)GetIndexElement("OutputPathX3_2", index));
                MarkShape((Shape)GetIndexElement("OutputPathX3_3", index));
                MarkShape((Shape)GetIndexElement("ShiftCircle", index));
            }, Undo);
            PageAction execShift = new PageAction(() =>
            {
                Add((TextBlock)GetIndexElement("QROutX3", index), GetHexResult(ResultType.QR_OUTPUT_X3, index - 1));
            }, Undo);
            PageAction unmarkShift = new PageAction(() =>
            {
                UnmarkBorder((Border)GetIndexElement("QROutX3Cell", index));
                UnmarkBorder((Border)GetIndexElement("QRXORCell", index));
                UnmarkShape((Shape)GetIndexElement("OutputPathX3_2", index));
                UnmarkShape((Shape)GetIndexElement("OutputPathX3_3", index));
                UnmarkShape((Shape)GetIndexElement("ShiftCircle", index));
            }, Undo);
            return new PageAction[] { markShift, execShift, unmarkShift };
        }

        private PageAction[] X3OutActions(int index)
        {
            PageAction[] xorActions = XORActions(index);
            PageAction[] shiftActions = ShiftActions(index);
            PageAction[] actions = new PageAction[xorActions.Length + shiftActions.Length];
            xorActions.CopyTo(actions, 0);
            shiftActions.CopyTo(actions, xorActions.Length);
            return actions;
        }

        private PageAction[] QRExecActions(int index)
        {
            List<PageAction> actions = new List<PageAction>();
            PageAction[] x1OutActions = X1OutActions(index);
            PageAction[] x2OutActions = X2OutActions(index);
            PageAction[] x3OutActions = X3OutActions(index);
            actions.AddRange(x1OutActions);
            actions.AddRange(x2OutActions);
            actions.AddRange(x3OutActions);
            return actions.ToArray();
        }

        private PageAction[] CopyToQRDetailInput(Border[] b, int index)
        {
            return CopyActions(
                b,
                new Shape[] { (Shape)GetIndexElement("QRInX1Path", index), (Shape)GetIndexElement("QRInX2Path", index), (Shape)GetIndexElement("QRInX3Path", index) },
                new Border[] { (Border)GetIndexElement("QRInX1Cell", index), (Border)GetIndexElement("QRInX2Cell", index), (Border)GetIndexElement("QRInX3Cell", index) }
                );
        }

        private Border GetStateCell(int stateIndex)
        {
            return (Border)GetIndexElement("UIKeystreamBlockGenCell", stateIndex);
        }
        private PageAction[] CopyFromStateTOQRInputActions(int index)
        {
            int i = 0, j = 0, k = 0, l = 0;
            switch(index)
            {
                case 1:
                    i = 0; j = 4; k = 8; l = 12;
                    break;
            }
            Border[] stateCells = new Border[] { GetStateCell(i), GetStateCell(j), GetStateCell(k), GetStateCell(l) };
            return CopyActions(stateCells, new Border[] { QRInACell, QRInBCell, QRInCCell, QRInDCell });
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

            p.AddAction(CopyFromStateTOQRInputActions(1));

            //p.AddAction(CreateQRInputAction(1));
            p.AddAction(CopyToQRDetailInput(new Border[] { QRInACell, QRInBCell, QRInDCell }, 1));
            p.AddAction(QRExecActions(1));

            p.AddAction(CopyToQRDetailInput(new Border[] { QRInCCell, (Border)GetIndexElement("QROutX3Cell", 1), (Border)GetIndexElement("QROutX2Cell", 1) }, 2));
            p.AddAction(QRExecActions(2));

            p.AddAction(CopyToQRDetailInput(new Border[] { (Border)GetIndexElement("QROutX1Cell", 1), (Border)GetIndexElement("QROutX3Cell", 2), (Border)GetIndexElement("QROutX2Cell", 2) }, 3));
            p.AddAction(QRExecActions(3));

            p.AddAction(CopyToQRDetailInput(new Border[] { (Border)GetIndexElement("QROutX1Cell", 2), (Border)GetIndexElement("QROutX3Cell", 3), (Border)GetIndexElement("QROutX2Cell", 3) }, 4));
            p.AddAction(QRExecActions(4));

            p.AddAction(QROutputActions());
            return p;
        }
    }
}