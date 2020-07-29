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
            #region Copy input values
            Page p = new Page(UIQuarterroundPage);
            PageAction showInput = new PageAction()
            {
                exec = () =>
                {
                    Add(QRInA, new Run(ConstantsLittleEndian.Replace(" ", "").Substring(0, 8)));
                    Add(QRInB, new Run(KeyLittleEndian.Replace(" ", "").Substring(0, 8)));
                    Add(QRInC, new Run(KeyLittleEndian.Replace(" ", "").Substring(InputKey.Length == 16 ? 0 : 32, 8)));
                    Add(QRInD, new Run(InitialCounterLittleEndian.Replace(" ", "").Substring(0, 8)));
                },
                undo = Undo
            };
            PageAction markQRInput = new PageAction()
            {
                exec = () =>
                {
                    SetBackground(QRInACell, copyBrush);
                    SetBackground(QRInBCell, copyBrush);
                    SetBackground(QRInDCell, copyBrush);
                },
                undo = Undo
            };
            PageAction copyMarkedQRInputToDetail = new PageAction()
            {
                exec = () =>
                {
                    SetBackground(QRInX1Cell, copyBrush);
                    SetBackground(QRInX2Cell, copyBrush);
                    SetBackground(QRInX3Cell, copyBrush);
                    Add(QRInX1, QRInA.Text);
                    Add(QRInX2, QRInB.Text);
                    Add(QRInX3, QRInD.Text);
                },
                undo = Undo
            };
            PageAction unmark = new PageAction()
            {
                exec = () =>
                {
                    UnsetBackground(QRInACell);
                    UnsetBackground(QRInBCell);
                    UnsetBackground(QRInDCell);
                    UnsetBackground(QRInX1Cell);
                    UnsetBackground(QRInX2Cell);
                    UnsetBackground(QRInX3Cell);
                },
                undo = Undo
            };
            p.AddAction(showInput);
            p.AddAction(markQRInput);
            p.AddAction(copyMarkedQRInputToDetail);
            p.AddAction(unmark);
            #endregion
            #region out x1
            PageAction prepareAddX1X2 = new PageAction()
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
            p.AddAction(prepareAddX1X2);
            PageAction execAddX1X2 = new PageAction()
            {
                exec = () =>
                {
                    Add(QROutX1, HexResultAddX1X2(0));
                },
                undo = Undo
            };
            PageAction unmarkX1X2 = new PageAction()
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
            p.AddAction(execAddX1X2);
            p.AddAction(unmarkX1X2);
            #endregion
            #region out x2
            PageAction prepareOutX2 = new PageAction()
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
                    Add(QROutX2, HexResultOutX2(0));
                },
                undo = Undo
            };
            p.AddAction(prepareOutX2);
            p.AddAction(execOutX2);
            #endregion
            #region out x3
            #endregion
            return p;
        }
    }
}