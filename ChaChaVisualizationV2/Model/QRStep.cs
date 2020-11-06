namespace Cryptool.Plugins.ChaChaVisualizationV2.Model
{
    /// <summary>
    /// Class for each step during one quarterround.
    /// </summary>
    internal class QRStep
    {
        public QRValue Add { get; set; }
        public QRValue XOR { get; set; }
        public QRValue Shift { get; set; }
    }
}