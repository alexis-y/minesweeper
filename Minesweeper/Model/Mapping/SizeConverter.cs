using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Drawing;

namespace Minesweeper.Model.Mapping
{
    public class SizeConverter : ValueConverter<Size, string>
    {
        // NOTE: We don't do sanitation of the values in the DB
        public SizeConverter() : base(
            /* model -> db */ value => $"{value.Width}x{value.Height}",
            /* db -> model */ value => new Size(int.Parse(value.Split('x', StringSplitOptions.None)[0]), int.Parse(value.Split('x', StringSplitOptions.None)[1])),
            new ConverterMappingHints(7))
        {
        }
    }
}
