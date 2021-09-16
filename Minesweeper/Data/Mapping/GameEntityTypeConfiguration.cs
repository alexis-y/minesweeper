using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minesweeper.Model;

namespace Minesweeper.Data.Mapping
{
    public class GameEntityTypeConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.Ignore(o => o.Field);
            builder.Ignore(o => o.Flags);
            builder.Ignore(o => o.Uncovered);

            builder.Property(o => o.Mines)
                .HasConversion(new PointsConverter());

            // This guy is very particular. Internally it's a 2-dimensional array of chars, but it's persisted as multiline string and also compared as one.
            builder.Property(o => o.FieldState)
                .UsePropertyAccessMode(PropertyAccessMode.Property)     // Set thru the property so the rest of the state is also set
                .HasConversion(
                    /* model -> db */ value => FieldStateConverter.GetString(value),
                    /* db -> model */ value => FieldStateConverter.GetCharGrid(value))
                .Metadata.SetValueComparer(new ValueComparer<char[,]>(
                    /* equality */ (x, y) => FieldStateConverter.GetString(x) == FieldStateConverter.GetString(y), 
                    /* hashcode */ x => FieldStateConverter.GetString(x).GetHashCode()));

        }
    }
}
