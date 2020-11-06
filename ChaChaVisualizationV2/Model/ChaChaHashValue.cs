namespace Cryptool.Plugins.ChaChaVisualizationV2.Model
{
    /// <summary>
    /// Abstract class for all values which are calculated during the ChaCha hash function.
    /// </summary>
    internal abstract class ChaChaHashValue
    {
        public ChaChaHashValue(uint value)
        {
            Value = value;
        }

        /// <summary>
        /// The actual UInt32 value.
        /// </summary>
        public uint Value { get; set; }

        /// <summary>
        /// True if the element containing the value should be marked, for example by setting the background to a specific color.
        /// </summary>
        public bool Mark { get; set; }
    }
}