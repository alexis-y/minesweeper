using System;
using System.Collections.Generic;
using System.Drawing;

namespace Minesweeper.Dto
{
    public class Game
    {
        public Guid Id { get; set; }

        public Size Field { get; set; }

        public IDictionary<Point, byte> Uncovered { get; set; }
    }
}
