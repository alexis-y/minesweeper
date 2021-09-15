using System;
using System.Threading.Tasks;

namespace Minesweeper.Model
{
    public class DbGameRepository : IGameRepository
    {
        public DbGameRepository(MinesweeperContext db)
        {
            Db = db ?? throw new ArgumentNullException(nameof(db));
        }

        protected MinesweeperContext Db { get; }

        public async ValueTask<Game> GetAsync(Guid id)
        {
            return await Db.Games.FindAsync(id);
        }

        public async Task SaveAsync(Game game)
        {
            if (!Db.Games.Local.Contains(game)) Db.Games.Add(game);
            await Db.SaveChangesAsync();
        }
    }
}
