using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
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

        public Game()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Constructs a new game with mines deployed to the given locations.
        /// </summary>
        /// <remarks>
        /// This constructor is only meaningful for unit testing.
        /// </remarks>
        public Game(Size field, IEnumerable<Point> mines) : this()
        {
            if (field.Width <= 0 || field.Height <= 0) throw new ArgumentException("The field's size cannot be 0", nameof(field));
            if (field.Width >= 256 || field.Height >= 256) throw new ArgumentException("The field's side cannot exceed 255.", nameof(field));
            if (mines == null) throw new ArgumentNullException(nameof(mines));
            if (mines.Any(p => !(p.X >= 0 && p.X < field.Width && p.Y >= 0 && p.Y < field.Height))) throw new ArgumentException("There are mines outside the field.");


            Field = field;
            Mines = new ReadOnlyCollection<Point>(mines.ToList());
        }

        /// <summary>
        /// Configures a new game field and fills it with mines.
        /// </summary>
        /// <param name="field">Width and height of the field</param>
        /// <param name="mines">Number of mines to deploy in the field</param>
        static public Game Create(Size field, ushort mines)
        {
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

            return new Game(
                field,
                minePos.Select(p => new Point(p % field.Width, p / field.Width))    // Wrap the linear position in 2 dimensions
            );
        }

        /// <summary>
        /// Gets the primary key of the game.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; protected set; }

        /// <summary>
        /// Gets or sets the user that owns this game.
        /// </summary>
        public ApplicationUser Owner { get; set; }
        
        /// <summary>
        /// Gets the width and height of the game field.
        /// </summary>
        public Size Field { get; protected set; }

        /// <summary>
        /// Gets the number of moves the player took.
        /// </summary>
        public int Moves { get; protected set; }

        /// <summary>
        /// Gets the location of every mine in the field.
        /// </summary>
        public IEnumerable<Point> Mines { get; protected set; } // This is private data that the backend will *not* divulge

        /// <summary>
        /// Gets the positions the player flagged.
        /// </summary>
        public IReadOnlyDictionary<Point, FlagKind> Flags => (IReadOnlyDictionary<Point, FlagKind>)_flags;
        private IDictionary<Point, FlagKind> _flags = new Dictionary<Point, FlagKind>();

        /// <summary>
        /// Gets all the positions that are uncovered, as well as the number of mines adjacent for each.
        /// </summary>
        /// <remarks>
        /// A special value of <see cref="Mine"/> means that position is a mine that was exposed.
        /// </remarks>
        public IReadOnlyDictionary<Point, byte> Uncovered => (IReadOnlyDictionary<Point, byte>)_uncovered;
        private IDictionary<Point, byte> _uncovered = new Dictionary<Point, byte>();
        
        /// <summary>
        /// Represents the state of the minefield in a grid of chars for simple serialization.
        /// </summary>
        public char[,] FieldState
        {
            get
            {
                if (_fieldState == null) _fieldState = GenerateFieldState();
                return _fieldState;
            }
            protected set
            {
                SetGameState(value);
                _fieldState = value;
                // TODO: Refactor the private state to a separate class with testable input/output
            }
        }
        private char[,] _fieldState = null;

        protected void InvalidateFieldState()
        {
            _fieldState = null;
        }

        private char[,] GenerateFieldState()
        {
            /*
             * Given the minefield size, we can transform the internal list of points to a simple text grid with a char for each tile
             * ie.:
             * 
             * ..#..
             * ..1..
             * #202#
             * .#2#.
             * .....
             * 
             */

            // Key: 
            //  '.' covered tile
            //  '[0-8]' uncovered tile, with the given number of mines in the proximity 
            //  'X' uncovered mine
            //  '#' red flag
            //  '?' mark

            var result = new char[Field.Width, Field.Height];
            for (var y = 0; y < Field.Height; y++)
            {
                for (var x = 0; x < Field.Width; x++)
                {
                    var p = new Point(x, y);
                    if (Flags.TryGetValue(p, out var kind))
                    {
                        result[x, y] = kind == FlagKind.RedFlag ? '#' : '?';
                    }
                    else if (Uncovered.TryGetValue(p, out var value))
                    {
                        result[x, y] = value == Mine ? 'X' : $"{value}"[0];
                    }
                    else
                    {
                        result[x, y] = '.';
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Sets the internal game state based on the grid representation.
        /// </summary>
        private void SetGameState(char[,] fieldState)
        {
            if (fieldState == null) throw new ArgumentNullException(nameof(fieldState));
            // This is only intended to be used by EF
            if (!Field.IsEmpty) throw new InvalidOperationException();

            // Just in case
            _uncovered = new Dictionary<Point, byte>();
            _flags = new Dictionary<Point, FlagKind>();

            // Set the field size based on the grid dimensions
            Field = new Size(fieldState.GetLength(0), fieldState.GetLength(1));

            for (var y = 0; y < Field.Height; y++) for (var x = 0; x < Field.Width; x++)
                {
                    var p = new Point(x, y);
                    if (char.IsNumber(fieldState[x, y]))
                    {
                        // It's an uncovered tile
                        _uncovered[p] = byte.Parse($"{fieldState[x, y]}");
                    }
                    else if (fieldState[x, y] == '#')
                    {
                        _flags[p] = FlagKind.RedFlag;
                    }
                    else if (fieldState[x, y] == '?')
                    {
                        _flags[p] = FlagKind.Tentative;
                    }
                }
        }

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
            if (Uncovered.Keys.Contains(position)) throw new ArgumentException("Already uncovered this position.");
            if (Flags.Keys.Contains(position)) throw new ArgumentException("Already flagged this position.");
            GuardPosition(position);

            Moves++;
            InvalidateFieldState();

            if (Mines.Contains(position))
            {
                // boom
                Result = GameResult.Lose;
                foreach(var mine in Mines) _uncovered[mine] = Mine;
            }

            // Uncover and expand outwards until there's a warning
            Uncover(position);

            // The game continues until the the only covered positions are mines
            if (Field.Width * Field.Height - Mines.Count() == Uncovered.Count) Result = GameResult.Win;

        }

        /// <summary>
        /// Sets a flag on a position, so that position cannot be uncovered.
        /// </summary>
        public void Flag(Point position, FlagKind? kind)
        {
            if (Result.HasValue) throw new InvalidOperationException("The game already ended.");
            if (kind != null && Flags.ContainsKey(position) && Flags[position] == kind) throw new ArgumentException("Already flagged this position.");
            if (Uncovered.Keys.Contains(position)) throw new ArgumentException("Already uncovered this position.");
            GuardPosition(position);

            InvalidateFieldState();

            if (kind != null)
            {
                _flags[position] = kind.Value;
            }
            else if (Flags.ContainsKey(position))
            {
                _flags.Remove(position);
            }
        }

        /// <summary>
        /// Uncover and expand outwards until there's a warning in a position.
        /// </summary>
        private void Uncover(Point position)
        {
            if (Flags.Keys.Contains(position)) return;           // Don't uncover flagged positions.
            if (_uncovered.ContainsKey(position)) return;    // Don't repeat positions

            var warning = GetWarningCount(position);
            _uncovered[position] = warning;
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
