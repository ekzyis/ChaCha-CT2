using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Shapes;

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
            PageAction insertQRInput = new PageAction()
            {
                exec = () =>
                {
                    Add(QRInA, GetHexResult(ResultType.QR_INPUT_A, index-1));
                    Add(QRInB, GetHexResult(ResultType.QR_INPUT_B, index-1));
                    Add(QRInC, GetHexResult(ResultType.QR_INPUT_C, index-1));
                    Add(QRInD, GetHexResult(ResultType.QR_INPUT_D, index-1));
                },
                undo = Undo
            };
            return insertQRInput;
        }

        private PageAction[] CreateX1OutActions(int index)
        {
            PageAction markOutX1 = new PageAction()
            {
                exec = () =>
                {
                    MarkBorder((Border)GetIndexElement("QRInX1Cell", index));
                    MarkBorder((Border)GetIndexElement("QRInX2Cell", index));
                    MarkShape((Shape)GetIndexElement("AddInputPathX1", index));
                    MarkShape((Shape)GetIndexElement("AddInputPathX2", index));
                    MarkShape((Shape)GetIndexElement("OutputPathX1", index));
                    MarkShape((Shape)GetIndexElement("OutputPathX2_1", index));
                    MarkShape((Shape)GetIndexElement("AddCircle", index));
                    MarkBorder((Border)GetIndexElement("QROutX1Cell", index));
                },
                undo = Undo
            };
            PageAction execOutX1 = new PageAction()
            {
                exec = () =>
                {
                    Add((TextBlock)GetIndexElement("QROutX1", index), GetHexResult(ResultType.QR_ADD_X1_X2, index-1));
                },
                undo = Undo
            };
            PageAction unmarkOutX1 = new PageAction()
            {
                exec = () =>
                {
                    UnmarkBorder((Border)GetIndexElement("QRInX1Cell", index));
                    UnmarkBorder((Border)GetIndexElement("QRInX2Cell", index));
                    UnmarkShape((Shape)GetIndexElement("AddInputPathX1", index));
                    UnmarkShape((Shape)GetIndexElement("AddInputPathX2", index));
                    UnmarkShape((Shape)GetIndexElement("OutputPathX1", index));
                    UnmarkShape((Shape)GetIndexElement("OutputPathX2_1", index));
                    UnmarkShape((Shape)GetIndexElement("AddCircle", index));
                    UnmarkBorder((Border)GetIndexElement("QROutX1Cell", index));
                },
                undo = Undo
            };
            return new PageAction[] { markOutX1, execOutX1, unmarkOutX1 };
        }

        private PageAction[] CreateX2OutActions(int index)
        {
            PageAction markOutX2 = new PageAction()
            {
                exec = () =>
                {
                    MarkBorder((Border)GetIndexElement("QRInX2Cell", index));
                    MarkShape((Shape)GetIndexElement("OutputPathX2_1", index));
                    MarkShape((Shape)GetIndexElement("OutputPathX2_2", index));
                    MarkBorder((Border)GetIndexElement("QROutX2Cell", index));
                },
                undo = Undo
            };
            PageAction execOutX2 = new PageAction()
            {
                exec = () =>
                {
                    Add((TextBlock)GetIndexElement("QROutX2", index), GetHexResult(ResultType.QR_OUTPUT_X2, index-1));
                },
                undo = Undo
            };
            PageAction unmarkOutX2 = new PageAction()
            {
                exec = () =>
                {
                    UnmarkBorder((Border)GetIndexElement("QRInX2Cell", index));
                    UnmarkShape((Shape)GetIndexElement("OutputPathX2_1", index));
                    UnmarkShape((Shape)GetIndexElement("OutputPathX2_2", index));
                    UnmarkBorder((Border)GetIndexElement("QROutX2Cell", index));
                },
                undo = Undo
            };
            return new PageAction[] { markOutX2, execOutX2, unmarkOutX2 };
        }

        private PageAction[] CreateXORActions(int index)
        {
            PageAction markXOR = new PageAction()
            {
                exec = () =>
                {
                    MarkBorder((Border)GetIndexElement("QRInX3Cell", index));
                    MarkBorder((Border)GetIndexElement("QROutX1Cell", index));
                    MarkBorder((Border)GetIndexElement("QRXORCell", index));
                    MarkShape((Shape)GetIndexElement("OutputPathX3_1", index));
                    MarkShape((Shape)GetIndexElement("XORInputPathX1", index));
                    MarkShape((Shape)GetIndexElement("XORCircle", index));
                },
                undo = Undo
            };
            PageAction execXOR = new PageAction()
            {
                exec = () =>
                {
                    Add((TextBlock)GetIndexElement("QRXOR", index), GetHexResult(ResultType.QR_XOR, index-1));
                },
                undo = Undo
            };
            PageAction unmarkXOR = new PageAction()
            {
                exec = () =>
                {
                    UnmarkBorder((Border)GetIndexElement("QRInX3Cell", index));
                    UnmarkBorder((Border)GetIndexElement("QROutX1Cell", index));
                    UnmarkBorder((Border)GetIndexElement("QRXORCell", index));
                    UnmarkShape((Shape)GetIndexElement("OutputPathX3_1", index));
                    UnmarkShape((Shape)GetIndexElement("XORInputPathX1", index));
                    UnmarkShape((Shape)GetIndexElement("XORCircle", index));
                },
                undo = Undo
            };
            return new PageAction[] { markXOR, execXOR, unmarkXOR };
        }

        private PageAction[] CreateShiftActions(int index)
        {
            PageAction markShift = new PageAction()
            {
                exec = () =>
                {
                    MarkBorder((Border)GetIndexElement("QROutX3Cell", index));
                    MarkBorder((Border)GetIndexElement("QRXORCell", index));
                    MarkShape((Shape)GetIndexElement("OutputPathX3_2", index));
                    MarkShape((Shape)GetIndexElement("OutputPathX3_3", index));
                    MarkShape((Shape)GetIndexElement("ShiftCircle", index));
                },
                undo = Undo
            };
            PageAction execShift = new PageAction()
            {
                exec = () =>
                {
                    Add((TextBlock)GetIndexElement("QROutX3", index), GetHexResult(ResultType.QR_OUTPUT_X3, index-1));
                },
                undo = Undo
            };
            PageAction unmarkShift = new PageAction()
            {
                exec = () =>
                {
                    UnmarkBorder((Border)GetIndexElement("QROutX3Cell", index));
                    UnmarkBorder((Border)GetIndexElement("QRXORCell", index));
                    UnmarkShape((Shape)GetIndexElement("OutputPathX3_2", index));
                    UnmarkShape((Shape)GetIndexElement("OutputPathX3_3", index));
                    UnmarkShape((Shape)GetIndexElement("ShiftCircle", index));
                },
                undo = Undo
            };
            return new PageAction[] { markShift, execShift, unmarkShift };
        }

        private PageAction[] CreateX3OutActions(int index)
        {
            PageAction[] xorActions = CreateXORActions(index);
            PageAction[] shiftActions = CreateShiftActions(index);
            PageAction[] actions = new PageAction[xorActions.Length + shiftActions.Length];
            xorActions.CopyTo(actions, 0);
            shiftActions.CopyTo(actions, xorActions.Length);
            return actions;
        }

        private PageAction[] CreateQRExecActions(int index)
        {
            List<PageAction> actions = new List<PageAction>();
            PageAction[] x1OutActions = CreateX1OutActions(index);
            PageAction[] x2OutActions = CreateX2OutActions(index);
            PageAction[] x3OutActions = CreateX3OutActions(index);
            actions.AddRange(x1OutActions);
            actions.AddRange(x2OutActions);
            actions.AddRange(x3OutActions);
            return actions.ToArray();
        }

        private PageAction[] CreateCopyQRDetailOutToDiagramActions(Border[] b, int index)
        {
            return CreateCopyActions(new Border[] { (Border)GetIndexElement("QROutX1Cell", index), (Border)GetIndexElement("QROutX2Cell", index), (Border)GetIndexElement("QROutX3Cell", index)}, b);
        }

        private PageAction[] CreateCopyFromDiagramToQRDetailInActions(Border[] b, int index)
        {
            return CreateCopyActions(b, new Border[] { (Border)GetIndexElement("QRInX1Cell", index), (Border)GetIndexElement("QRInX2Cell", index), (Border)GetIndexElement("QRInX3Cell", index) });
        }

        private PageAction CreateClearQRDetailAction(int index)
        {
            PageAction clearQRDetail = new PageAction()
            {
                exec = () =>
                {
                    Clear((TextBlock)GetIndexElement("QRInX1", index));
                    Clear((TextBlock)GetIndexElement("QRInX2", index));
                    Clear((TextBlock)GetIndexElement("QRInX3", index));
                    Clear((TextBlock)GetIndexElement("QROutX1", index));
                    Clear((TextBlock)GetIndexElement("QROutX2", index));
                    Clear((TextBlock)GetIndexElement("QROutX3", index));
                    Clear((TextBlock)GetIndexElement("QRXOR", index));
                },
                undo = Undo
            };
            return clearQRDetail;
        }

        private Page QuarterroundPageV2()
        {
            Page p = new Page(UIQuarterroundPage);
            p.AddAction(CreateQRInputAction(1));

            p.AddAction(CreateCopyFromDiagramToQRDetailInActions(new Border[] { QRInACell, QRInBCell, QRInDCell }, 1));
            p.AddAction(CreateQRExecActions(1));
            p.AddAction(CreateCopyQRDetailOutToDiagramActions(new Border[] { (Border)GetIndexElement("QRDiagramX1Out_1_Cell", 1), (Border)GetIndexElement("QRDiagramX2Out_1_Cell", 1), (Border)GetIndexElement("QRDiagramX3Out_1_Cell", 1) }, 1));
            p.AddAction(CreateClearQRDetailAction(1));

            p.AddAction(CreateCopyFromDiagramToQRDetailInActions(new Border[] { QRInCCell, (Border)GetIndexElement("QRDiagramX3Out_1_Cell", 2), (Border)GetIndexElement("QRDiagramX2Out_1_Cell", 2) }, 2));
            p.AddAction(CreateQRExecActions(2));
            p.AddAction(CreateCopyQRDetailOutToDiagramActions(new Border[] { (Border)GetIndexElement("QRDiagramX1Out_2_Cell", 2), (Border)GetIndexElement("QRDiagramX2Out_2_Cell", 2), (Border)GetIndexElement("QRDiagramX3Out_2_Cell", 2) }, 2));
            p.AddAction(CreateClearQRDetailAction(2));
            return p;
        }
    }
}