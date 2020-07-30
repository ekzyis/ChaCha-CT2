using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Cryptool.Plugins.ChaCha
{
    public partial class ChaChaPresentation : UserControl, INotifyPropertyChanged
    {
        private Page QuarterroundPage()
        {
            #region QR input
            Page p = new Page(UIQuarterroundPage);
            PageAction insertQRInput = new PageAction()
            {
                exec = () =>
                {
                    Add(QRInA, GetHexResult(ResultType.QR_INPUT_A, 0));
                    Add(QRInB, GetHexResult(ResultType.QR_INPUT_B, 0));
                    Add(QRInC, GetHexResult(ResultType.QR_INPUT_C, 0));
                    Add(QRInD, GetHexResult(ResultType.QR_INPUT_D, 0));
                },
                undo = Undo
            };
            p.AddAction(insertQRInput);
            #endregion
            #region copy into QR detail
            p.AddAction(CreateCopyActions(new Border[] { QRInACell, QRInBCell, QRInDCell }, new Border[] { QRInX1Cell, QRInX2Cell, QRInX3Cell }));
            #endregion
            #region out x1
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
                    Add(QROutX1, GetHexResult(ResultType.QR_ADD_X1_X2, 0));
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
            p.AddAction(markOutX1);
            p.AddAction(execOutX1);
            p.AddAction(unmarkOutX1);
            #endregion
            #region out x2
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
                    Add(QROutX2, GetHexResult(ResultType.QR_OUTPUT_X2, 0));
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
            p.AddAction(markOutX2);
            p.AddAction(execOutX2);
            p.AddAction(unmarkOutX2);
            #endregion
            #region out x3
            #region out xor
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
                    Add(QRXOR, GetHexResult(ResultType.QR_XOR, 0));
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
            p.AddAction(markXOR);
            p.AddAction(execXOR);
            p.AddAction(unmarkXOR);
            #endregion
            #region out shift
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
                    Add(QROutX3, GetHexResult(ResultType.QR_OUTPUT_X3, 0));
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
            p.AddAction(markShift);
            p.AddAction(execShift);
            p.AddAction(unmarkShift);
            #endregion
            #endregion
            #region copy into QR diagram
            p.AddAction(CreateCopyActions(new Border[] { QROutX1Cell, QROutX2Cell, QROutX3Cell }, new Border[] { QRDiagramX1Out_1_Cell, QRDiagramX2Out_1_Cell, QRDiagramX3Out_1_Cell }));
            #endregion
            return p;
        }
    }
}