using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minesweeper.Model;
using System;
using System.Drawing;
using System.Linq;

namespace Minesweeper.Data.Mapping
{
    public class GameEntityTypeConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.Property(o => o.Field)
                .HasConversion(new SizeConverter());  // This as a convention would be nice. *shrugs*

            builder.Property(o => o.Mines)
                .HasConversion(new PointsConverter());

            builder.Property(o => o.Moves)
                .HasConversion(new PointsConverter());

            builder.Property(o => o.Flags)
                .HasConversion(
                    /* model -> db */ value => string.Join(';', value.Select(p => $"{p.Key.X}x{p.Key.Y}x{(int)p.Value}")),
                    /* db -> model */ value => value.Split(';', StringSplitOptions.None).ToDictionary(p => new Point(int.Parse(p.Split('x', StringSplitOptions.None)[0]), int.Parse(p.Split('x', StringSplitOptions.None)[1])),
                                                                                                      p => (FlagKind)int.Parse(p.Split('x', StringSplitOptions.None)[2]))
                );

            // Because the game is deterministic, this is the only data required to recreate the entire game state
            // TODO: Actually do recreate the game after hidrating a new Game. Something like game.Replay(). We need
            //       someplace in EF to latch to.
        }
    }
}
