namespace Cryptool.Plugins.ChaChaVisualizationV2.Model
{
    /// <summary>
    /// Class for the values during a quarterround.
    /// </summary>
    internal class QRValue : ChaChaHashValue
    {
        public QRValue(uint value) : base(value)
        {
        }

        /// <summary>
        /// True if the input paths for this value should be marked.
        /// </summary>
        private bool _markInput; public bool MarkInput

        {
            get
            {
                return _markInput;
            }
            set
            {
                _markInput = value;
                OnPropertyChanged();
            }
        }
    }
}