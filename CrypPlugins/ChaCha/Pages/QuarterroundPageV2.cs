using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Cryptool.Plugins.ChaCha
{
    public partial class ChaChaPresentation : UserControl, INotifyPropertyChanged
    {
        private Page QuarterroundPageV2()
        {
            Page p = new Page(UIQuarterroundPage);
            return p;
        }
    }
}