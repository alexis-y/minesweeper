using System;
using System.Collections.Generic;

namespace Minesweeper.Dto
{
    /// <summary>
    /// Describes the public-facing state of a game.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// The ID of the game.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The size of the minefield.
        /// </summary>
        public Size Field { get; set; }

        /// <summary>
        /// The minefield data represented as a grid in text form.
        /// </summary>
        public string Uncovered { get; set; }

        /// <summary>
        /// The result of the game if it's over.
        /// </summary>
        public Model.GameResult? Result { get; set; }
    }
}
