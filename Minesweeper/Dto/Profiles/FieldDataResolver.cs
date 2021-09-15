using AutoMapper;
using System.Collections.Generic;
using System.Text;

namespace Minesweeper.Dto.Profiles
{
    /// <summary>
    /// Converts the internal representation of the uncovered minefield into a compact string representation.
    /// </summary>
    public class FieldDataResolver : IMemberValueResolver<Model.Game, Dto.Game, IReadOnlyDictionary<System.Drawing.Point, byte>, string>
    {
        public string Resolve(Model.Game source, Dto.Game destination, IReadOnlyDictionary<System.Drawing.Point, byte> sourceMember, string destMember, ResolutionContext context)
        {
            /*
             * Given the minefield size, we can transform the internal list of points to a simple text grid with a char for each tile
             * ie.:
             * 
             * .....
             * ..1..
             * .202.
             * ..2..
             * .....
             * 
             */

            // Key: 
            //  '.' covered tile
            //  '[0-8]' uncovered tile, with the given number of mines in the proximity 
            //  'X' uncovered mine

            var sb = new StringBuilder(source.Field.Height * source.Field.Width);
            for (var y = 0; y < source.Field.Height; y++)
            {
                for (var x = 0; x < source.Field.Width; x++)
                {
                    var p = new System.Drawing.Point(x, y);
                    if (sourceMember.TryGetValue(p, out var value))
                    {
                        if (value == Model.Game.Mine)
                        {
                            sb.Append("X");
                        }
                        else
                        {
                            sb.Append(value);
                        }
                    }
                    else
                    {
                        sb.Append(".");
                    }
                }
                if (y < source.Field.Height - 1) sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
