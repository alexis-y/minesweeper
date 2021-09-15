using Microsoft.EntityFrameworkCore;
using Minesweeper.Model.Mapping;

namespace Minesweeper.Model
{
    public class MinesweeperContext : DbContext
    {
        public MinesweeperContext() : base() { }
        public MinesweeperContext(DbContextOptions options) : base(options) { }

        public DbSet<Game> Games { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new GameEntityTypeConfiguration());

            // NOTE: Next version of EF has "ApplyConfigurationsFromAssembly" which does the same in one call
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
