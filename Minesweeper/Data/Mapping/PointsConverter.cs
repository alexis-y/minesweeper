using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Minesweeper.Data.Mapping
{
    public class PointsConverter: ValueConverter<IEnumerable<Point>, string>
    {
        // NOTE: We don't do sanitation of the values in the DB
        public PointsConverter() : base(
            /* model -> db */ value => string.Join(';', value.Select(p => $"{p.X}x{p.Y}")),
            /* db -> model */ value => value.Split(';', StringSplitOptions.None).Select(p => new Point(int.Parse(p.Split('x', StringSplitOptions.None)[0]), int.Parse(p.Split('x', StringSplitOptions.None)[1]))).ToList())
        {
        }

    }
}
