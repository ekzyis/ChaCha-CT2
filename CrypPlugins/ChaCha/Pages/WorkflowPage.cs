using System.ComponentModel;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaCha
{
    public partial class ChaChaPresentation : UserControl, INotifyPropertyChanged
    {
        private Page WorkflowPage()
        {
            return new Page(UIWorkflowPage);
        }
    }
}