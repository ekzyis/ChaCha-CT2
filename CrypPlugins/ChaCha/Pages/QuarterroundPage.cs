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
            PageAction detailFirstQRBox = new PageAction()
            {
                exec = () =>
                {
                    SetBackground(QRInACell, Brushes.AliceBlue);
                    SetBackground(QRInBCell, Brushes.AliceBlue);
                    SetBackground(QRInCCell, Brushes.AliceBlue);
                    SetBackground(QRInDCell, Brushes.AliceBlue);
                }
            };
            p.AddAction(showInput);
            return p;
        }
    }
}