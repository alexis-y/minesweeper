namespace Minesweeper.Dto
{
    /// <summary>
    /// Stores an integer size of two dimensions.
    /// </summary>
    public class Size
    {
        // We use our own type for the public API to better control docs and serialization.

        /// <summary>
        /// Gets or sets the vertical component of the size.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the horizontal component of the size.
        /// </summary>
        public int Width { get; set; }

        public static implicit operator System.Drawing.Size(Size s) => new System.Drawing.Size(s.Width, s.Height);
        public static explicit operator Size(System.Drawing.Size s) => new Size() { Width = s.Width, Height = s.Height };

    }
}
