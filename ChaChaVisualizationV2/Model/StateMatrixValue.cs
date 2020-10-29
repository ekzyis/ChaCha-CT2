using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cryptool.Plugins.ChaChaVisualizationV2.Model
{
    internal class StateMatrixValue : IGrid<uint>, INotifyPropertyChanged
    {
        public StateMatrixValue(uint value, int row, int column)
        {
            this.Value = value;
            this.Row = row;
            this.Column = column;
        }

        private uint _value; public uint Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Row
        {
            get; set;
        }

        public int Column
        {
            get; set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
    }
}