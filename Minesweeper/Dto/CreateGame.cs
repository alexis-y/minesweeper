using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Minesweeper.Dto
{
    public class CreateGame
    {
        public Size Field { get; set; }
        public ushort Mines { get; set; }
        public Point Move { get; set; }
    }
}
