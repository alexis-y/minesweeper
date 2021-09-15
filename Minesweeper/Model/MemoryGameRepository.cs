using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Minesweeper.Model
{
    public class MemoryGameRepository : IGameRepository
    {

        protected KeyedCollection<Guid, Game> Games { get; } = new GameCollection();

        public ValueTask<Game> GetAsync(Guid id)
        {
            return new ValueTask<Game>(Games[id]);
        }

        public Task SaveAsync(Game game)
        {
            if (!Games.Contains(game.Id)) Games.Add(game);
            return Task.CompletedTask;
        }

        class GameCollection : KeyedCollection<Guid, Game>
        {
            protected override Guid GetKeyForItem(Game item)
            {
                return item.Id;
            }
        }
    }
}
