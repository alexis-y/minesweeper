using System;
using System.Threading.Tasks;

namespace Minesweeper.Model
{
    public interface IGameRepository
    {
        Task<Game> GetAsync(Guid id);

        Task SaveAsync(Game game);
    }
}
