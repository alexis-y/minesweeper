namespace Minesweeper.Dto
{
    /// <summary>
    /// Parameters to start a new game.
    /// </summary>
    public class GameCreation
    {
        /// <summary>
        /// Sets the minefield size.
        /// </summary>
        /// <remarks>
        /// Each dimension must be in the range 0-255 (exclusive).
        /// </remarks>
        public Size Field { get; set; }

        /// <summary>
        /// Number of mines to place in the field.
        /// </summary>
        /// <remarks>
        /// The number must be lower than <see cref="Field"/>.Width * <see cref="Field"/>.Height - 1.
        /// </remarks>
        public ushort Mines { get; set; }

        /// <summary>
        /// The first position played.
        /// </summary>
        public Point Move { get; set; }
    }
}
