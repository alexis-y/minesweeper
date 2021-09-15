using System;
using System.Threading.Tasks;

namespace Minesweeper.Model
{
    public interface IGameRepository
    {
        ValueTask<Game> GetAsync(Guid id);

        Task SaveAsync(Game game);
    }
}
