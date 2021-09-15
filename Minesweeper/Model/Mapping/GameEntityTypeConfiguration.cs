using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minesweeper.Model.Mapping
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

            // Because the game is deterministic, this is the only data required to recreate the entire game state
        }
    }
}
