namespace Cryptool.Plugins.ChaChaVisualizationV2.Model
{
    internal class StateMatrixValue : IGrid<uint>
    {
        public StateMatrixValue(uint value, int row, int column)
        {
            this.Value = value;
            this.Row = row;
            this.Column = column;
        }

        public uint Value
        {
            get; set;
        }

        public int Row
        {
            get; set;
        }

        public int Column
        {
            get; set;
        }
    }
}