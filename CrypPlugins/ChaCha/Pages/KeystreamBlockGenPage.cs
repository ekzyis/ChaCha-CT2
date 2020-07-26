using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Cryptool.Plugins.ChaCha
{
    public partial class ChaChaPresentation : UserControl, INotifyPropertyChanged
    {
        private Page KeystreamBlockGenPage()
        {
            bool versionIsDJB = Version == ChaCha.Version.DJB;
            Page p = new Page(UIKeystreamBlockGenPage);
            PageAction initAction = new PageAction()
            {
                exec = () =>
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
                },
                undo = Undo
            };
            p.AddInitAction(initAction);
            return p;
        }
    }
}