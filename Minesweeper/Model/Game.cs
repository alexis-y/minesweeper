using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Minesweeper.Model
{
    /// <summary>
    /// Persisted data for a single game of Minesweeper
    /// </summary>
    public class Game
    {
        public const byte Mine = byte.MaxValue;

        public Game(Size field, IEnumerable<Point> mines)
        {
            Id = Guid.NewGuid();
            Field = field;
            Mines = mines ?? throw new ArgumentNullException(nameof(mines));
            if (Mines.Any(p => !(p.X >= 0 && p.X < Field.Width && p.Y >= 0 && p.Y < Field.Height))) throw new ArgumentException("There are mines outside the field.");
        }

        /// <summary>
        /// Configures a new game field and fills it with mines.
        /// </summary>
        /// <param name="field">Width and height of the field</param>
        /// <param name="mines">Number of mines to deploy in the field</param>
        static public Game Create(Size field, ushort mines)
        {
            if (field.Width <= 0 || field.Height <= 0) throw new ArgumentException("The field's size cannot be 0", nameof(field));
            if (field.Width >= 256 || field.Height >= 256) throw new ArgumentException("The field's side cannot exceed 255.", nameof(field));
            if (mines >= field.Width * field.Height) throw new ArgumentException("The number of mines is higher than the field size.", nameof(mines));

            // Deploy the mines randomly
            var rnd = new Random();
            // We'll roll dices for the linear positions
            var minePos = (from n in Enumerable.Range(0, mines)
                           let pos = (short)rnd.Next(field.Width * field.Height - mines)
                           orderby pos
                           select pos).ToArray();
            // Make the positions unique
            for (short n = 1, offset = 0; n < minePos.Length; n++)
            {
                if (minePos[n] + offset <= minePos[n - 1])
                {
                    offset = (short)(minePos[n - 1] - minePos[n] + 1);
                }
                minePos[n] += offset;
            }

            // IDEA: Also persist the mines a single short instead of coordinates

            return new Game(
                field,
                minePos.Select(p => new Point(p % field.Width, p / field.Width))    // Wrap the linear position in 2 dimensions
            );
        }

        /// <summary>
        /// Gets the primary key of the game.
        /// </summary>
        public Guid Id { get; protected set; }

        // TODO: Owner (thru ASP.NET Identity).
        //       For the time being, all games are public (if you know the ID)
        
        /// <summary>
        /// Gets the width and height of the game field.
        /// </summary>
        public Size Field { get; protected set; }

        /// <summary>
        /// Gets the location of every mine in the field.
        /// </summary>
        public IEnumerable<Point> Mines { get; protected set; } // This is private data that the backend will *not* divulge
        
        /// <summary>
        /// Gets the positions the player clicked.
        /// </summary>
        public IEnumerable<Point> Moves {
            get
            {
                return moves;
            }
        }
        private ICollection<Point> moves = new List<Point>();    // This is useful to recreate the gamestate from persistance

        // TODO: Flags, same as Mines

        /// <summary>
        /// Gets all the positions that are uncovered, as well as the number of mines adjacent for each.
        /// </summary>
        /// <remarks>
        /// A special value of <see cref="Mine"/> means that position is a mine that was exposed.
        /// </remarks>
        public IReadOnlyDictionary<Point, byte> Uncovered
        {
            get
            {
                return (IReadOnlyDictionary<Point, byte>)uncovered;
            }
        }
        private IDictionary<Point, byte> uncovered = new Dictionary<Point, byte>();
        
        /// <summary>
        /// If the game is over, gets the result.
        /// </summary>
        public GameResult? Result { get; protected set; }

        /// <summary>
        /// Gets the number of mines adjacent to a given position.
        /// </summary>
        public byte GetWarningCount(Point position)
        {
            GuardPosition(position);
            return (byte)Mines.Count(m => m.X >= position.X - 1 && m.X <= position.X + 1 && m.Y >= position.Y - 1 && m.Y <= position.Y + 1);
        }

        /// <summary>
        /// Steps on a position, uncovering part of the field or ending the game if the position is a mine.
        /// </summary>
        public void Move(Point position)
        {
            if (Result.HasValue) throw new InvalidOperationException("The game already ended.");
            if (Uncovered.Keys.Contains(position)) throw new InvalidOperationException("Already uncovered this position.");
            GuardPosition(position);

            // Store the movement
            moves.Add(position);

            if (Mines.Contains(position))
            {
                // boom
                Result = GameResult.Lose;
                foreach(var mine in Mines) uncovered[mine] = Mine;
            }

            // Uncover and expand outwards until there's a warning
            Uncover(position);

            // The game continues until the the only covered positions are mines
            if (Field.Width * Field.Height - Mines.Count() == Uncovered.Count) Result = GameResult.Win;

        }

        /// <summary>
        /// Uncover and expand outwards until there's a warning in a position.
        /// </summary>
        private void Uncover(Point position)
        {
            if (uncovered.ContainsKey(position)) return;    // Don't repeat positions

            var warning = GetWarningCount(position);
            uncovered[position] = warning;
            if (warning > 0) return;    // Don't expand if has a mine close by

            // Expand outwards
            var neighbours = from dir in new[] { new Size(1, 0), new Size(0, 1), new Size(-1, 0), new Size(0, -1) }
                             let p = position + dir
                             where IsInField(p)
                             select p;
            foreach (var p in neighbours) Uncover(p);
        }

        private bool IsInField(Point position)
        {
            return position.X >= 0 && position.Y >= 0 && position.X < Field.Width && position.Y < Field.Height;
        }

        private void GuardPosition(Point position)
        {
            if (!IsInField(position)) throw new ArgumentException("The position is outside the field", nameof(position));
        }
    }
}
