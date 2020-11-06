namespace Cryptool.Plugins.ChaChaVisualizationV2.Model
{
    /// <summary>
    /// Class for the values inside the state matrix.
    /// </summary>
    internal class StateValue : ChaChaHashValue
    {
        public StateValue(uint value) : base(value)
        {
        }
    }
}