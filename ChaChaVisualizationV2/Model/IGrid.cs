namespace Cryptool.Plugins.ChaChaVisualizationV2.Model
{
    /// <summary>
    /// Interface for classes which can be put into a grid
    /// </summary>
    internal interface IGrid<T>
    {
        /// <summary>
        /// The value of the cell.
        /// </summary>
        T Value { get; set; }

        int Row { get; set; }

        int Column { get; set; }
    }
}