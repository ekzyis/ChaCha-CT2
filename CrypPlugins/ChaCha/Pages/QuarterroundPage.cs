using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Cryptool.Plugins.ChaCha
{
    public partial class ChaChaPresentation : UserControl, INotifyPropertyChanged
    {
        private PageAction CreateQRInputAction(int index)
        {
            PageAction insertQRInput = new PageAction()
            {
                exec = () =>
                {
                    Add(QRInA, GetHexResult(ResultType.QR_INPUT_A, index));
                    Add(QRInB, GetHexResult(ResultType.QR_INPUT_B, index));
                    Add(QRInC, GetHexResult(ResultType.QR_INPUT_C, index));
                    Add(QRInD, GetHexResult(ResultType.QR_INPUT_D, index));
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
                    MarkBorder(QRInX1Cell);
                    MarkBorder(QRInX2Cell);
                    MarkShape(AddInputPathX1);
                    MarkShape(AddInputPathX2);
                    MarkShape(OutputPathX1);
                    MarkShape(OutputPathX2_1);
                    MarkShape(AddCircle);
                    MarkBorder(QROutX1Cell);
                },
                undo = Undo
            };
            PageAction execOutX1 = new PageAction()
            {
                exec = () =>
                {
                    Add(QROutX1, GetHexResult(ResultType.QR_ADD_X1_X2, index));
                },
                undo = Undo
            };
            PageAction unmarkOutX1 = new PageAction()
            {
                exec = () =>
                {
                    UnmarkBorder(QRInX1Cell);
                    UnmarkBorder(QRInX2Cell);
                    UnmarkShape(AddInputPathX1);
                    UnmarkShape(AddInputPathX2);
                    UnmarkShape(OutputPathX1);
                    UnmarkShape(OutputPathX2_1);
                    UnmarkShape(AddCircle);
                    UnmarkBorder(QROutX1Cell);
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
                    MarkBorder(QRInX2Cell);
                    MarkShape(OutputPathX2_1);
                    MarkShape(OutputPathX2_2);
                    MarkBorder(QROutX2Cell);
                },
                undo = Undo
            };
            PageAction execOutX2 = new PageAction()
            {
                exec = () =>
                {
                    Add(QROutX2, GetHexResult(ResultType.QR_OUTPUT_X2, index));
                },
                undo = Undo
            };
            PageAction unmarkOutX2 = new PageAction()
            {
                exec = () =>
                {
                    UnmarkBorder(QRInX2Cell);
                    UnmarkShape(OutputPathX2_1);
                    UnmarkShape(OutputPathX2_2);
                    UnmarkBorder(QROutX2Cell);
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
                    MarkBorder(QRInX3Cell);
                    MarkBorder(QROutX1Cell);
                    MarkBorder(QRXORCell);
                    MarkShape(OutputPathX3_1);
                    MarkShape(XORInputPathX1);
                    MarkShape(XORCircle);
                },
                undo = Undo
            };
            PageAction execXOR = new PageAction()
            {
                exec = () =>
                {
                    Add(QRXOR, GetHexResult(ResultType.QR_XOR, index));
                },
                undo = Undo
            };
            PageAction unmarkXOR = new PageAction()
            {
                exec = () =>
                {
                    UnmarkBorder(QRInX3Cell);
                    UnmarkBorder(QROutX1Cell);
                    UnmarkBorder(QRXORCell);
                    UnmarkShape(OutputPathX3_1);
                    UnmarkShape(XORInputPathX1);
                    UnmarkShape(XORCircle);
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
                    MarkBorder(QROutX3Cell);
                    MarkBorder(QRXORCell);
                    MarkShape(OutputPathX3_2);
                    MarkShape(OutputPathX3_3);
                    MarkShape(ShiftCircle);
                },
                undo = Undo
            };
            PageAction execShift = new PageAction()
            {
                exec = () =>
                {
                    Add(QROutX3, GetHexResult(ResultType.QR_OUTPUT_X3, index));
                },
                undo = Undo
            };
            PageAction unmarkShift = new PageAction()
            {
                exec = () =>
                {
                    UnmarkBorder(QROutX3Cell);
                    UnmarkBorder(QRXORCell);
                    UnmarkShape(OutputPathX3_2);
                    UnmarkShape(OutputPathX3_3);
                    UnmarkShape(ShiftCircle);
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

        private PageAction CreateClearQRDetailAction()
        {
            PageAction clearQRDetail = new PageAction()
            {
                exec = () =>
                {
                    Clear(QRInX1);
                    Clear(QRInX2);
                    Clear(QRInX3);
                    Clear(QROutX1);
                    Clear(QROutX2);
                    Clear(QROutX3);
                    Clear(QRXOR);
                },
                undo = Undo
            };
            return clearQRDetail;
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

        private PageAction[] CreateCopyQRDetailOutToDiagramActions(Border[] b)
        {
            return CreateCopyActions(new Border[] { QROutX1Cell, QROutX2Cell, QROutX3Cell}, b);
        }

        private PageAction[] CreateCopyFromDiagramToQRDetailInActions(Border[] b)
        {
            return CreateCopyActions(b, new Border[] { QRInX1Cell, QRInX2Cell, QRInX3Cell });
        }

        private Page QuarterroundPage()
        {
            Page p = new Page(UIQuarterroundPage);
            p.AddAction(CreateQRInputAction(0));

            // copy A,B,D to X1,X2,X3
            p.AddAction(CreateCopyFromDiagramToQRDetailInActions(new Border[] { QRInACell, QRInBCell, QRInDCell }));
            p.AddAction(CreateQRExecActions(0));
            // copy X1,X2,X3 to diagram
            p.AddAction(CreateCopyQRDetailOutToDiagramActions(new Border[] { QRDiagramX1Out_1_Cell, QRDiagramX2Out_1_Cell, QRDiagramX3Out_1_Cell }));
            p.AddAction(CreateClearQRDetailAction());

            p.AddAction(CreateCopyFromDiagramToQRDetailInActions(new Border[] { QRInCCell, QRDiagramX3Out_1_Cell, QRDiagramX2Out_1_Cell }));
            p.AddAction(CreateQRExecActions(1));
            // copy X1,X2,X3 to diagram
            p.AddAction(CreateCopyQRDetailOutToDiagramActions(new Border[] { QRDiagramX1Out_2_Cell, QRDiagramX2Out_2_Cell, QRDiagramX3Out_2_Cell }));
            p.AddAction(CreateClearQRDetailAction());

            return p;
        }
    }
}
