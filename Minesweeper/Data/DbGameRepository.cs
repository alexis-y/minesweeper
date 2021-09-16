using Microsoft.EntityFrameworkCore;
using Minesweeper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minesweeper.Data
{
    public class DbGameRepository : IGameRepository
    {
        public DbGameRepository(ApplicationDbContext db)
        {
            Db = db ?? throw new ArgumentNullException(nameof(db));
        }

        protected ApplicationDbContext Db { get; }

        public async ValueTask<Game> GetAsync(Guid id, ApplicationUser owner)
        {
            var game = await Db.Games.FindAsync(id);

            // Public games are only accesible by anonymous, and owned games by the owner
            if (game.Owner != owner) return null;

            return game;
        }

        public async ValueTask<IEnumerable<Game>> GetByOwnerIdAsync(ApplicationUser owner)
        {
            return await Db.Games.Where(g => g.Owner == owner).ToArrayAsync();
        }

        public async Task SaveAsync(Game game)
        {
            if (!Db.Games.Local.Contains(game)) Db.Games.Add(game);
            await Db.SaveChangesAsync();
        }
    }
}
