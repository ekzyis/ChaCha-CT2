using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Cryptool.Plugins.ChaCha
{
    public partial class ChaChaPresentation : UserControl, INotifyPropertyChanged
    {
        private Page KeystreamBlockGenPage()
        {
            bool versionIsDJB = Version == ChaCha.Version.DJB;
            Page p = new Page(UIKeystreamBlockGenPage);
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
            Brush copyBrush = Brushes.AliceBlue;
            PageAction initFirstColumnRoundAction1 = new PageAction(() =>
            {
                SetBackground(UIKeystreamBlockGenCell0, copyBrush);
                SetBackground(UIKeystreamBlockGenCell4, copyBrush);
                SetBackground(UIKeystreamBlockGenCell8, copyBrush);
                SetBackground(UIKeystreamBlockGenCell12, copyBrush);
            }, Undo);
            PageAction initFirstColumnRoundAction2 = new PageAction(() =>
            {
                SetBackground(UIKeystreamBlockGenQRACell, copyBrush);
                SetBackground(UIKeystreamBlockGenQRBCell, copyBrush);
                SetBackground(UIKeystreamBlockGenQRCCell, copyBrush);
                SetBackground(UIKeystreamBlockGenQRDCell, copyBrush);
                CopyLastText(UIKeystreamBlockGenQRA, UIKeystreamBlockGen0);
                CopyLastText(UIKeystreamBlockGenQRB, UIKeystreamBlockGen4);
                CopyLastText(UIKeystreamBlockGenQRC, UIKeystreamBlockGen8);
                CopyLastText(UIKeystreamBlockGenQRD, UIKeystreamBlockGen12);
            }, Undo);
            p.AddAction(generalDescriptionAction);
            p.AddAction(firstColumnRoundDescriptionAction);
            p.AddAction(initFirstColumnRoundAction1);
            p.AddAction(initFirstColumnRoundAction2);
            return p;
        }
    }
}