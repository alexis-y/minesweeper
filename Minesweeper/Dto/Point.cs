namespace Minesweeper.Dto
{
    /// <summary>
    /// Stores an integer point in two dimensions.
    /// </summary>
    public class Point
    {
        // We use our own type for the public API to better control docs and serialization.

        /// <summary>
        /// Gets or sets the horizontal value.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the vertical value.
        /// </summary>
        public int Y { get; set; }

        public static implicit operator System.Drawing.Point(Point p) => new System.Drawing.Point(p.X, p.Y);
        public static explicit operator Point(System.Drawing.Point p) => new Point() { X = p.X, Y = p.Y };

    }
}
