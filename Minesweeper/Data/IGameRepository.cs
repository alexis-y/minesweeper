using Minesweeper.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Minesweeper.Data
{
    public interface IGameRepository
    {
        ValueTask<Game> GetAsync(Guid id, ApplicationUser owner);

        ValueTask<IEnumerable<Game>> GetByOwnerIdAsync(ApplicationUser owner);

        Task SaveAsync(Game game);
    }
}
