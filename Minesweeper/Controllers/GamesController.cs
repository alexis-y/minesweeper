using System;
using System.Drawing;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.Model;

namespace Minesweeper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        public GamesController(IMapper mapper, IGameRepository db)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            Db = db ?? throw new ArgumentNullException(nameof(db)); // TODO: Persistance thru EF
        }

        protected IMapper Mapper { get; }

        protected IGameRepository Db { get; }

        // GET: api/games/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<Dto.Game> GetAsync(Guid id)
        {
            var game = await Db.GetAsync(id);
            return Mapper.Map<Dto.Game>(game);
        }

        // POST: api/games
        [HttpPost]
        public async Task<Dto.Game> Post([FromBody] Dto.CreateGame creation)
        {
            // Create a new game (and make the first move)
            var game = Game.Create(creation.Field, creation.Mines);

            // TODO: It's not good UX if the first move ends in gameover...
            await Db.SaveAsync(game);

            return Mapper.Map<Dto.Game>(game);
        }

        // POST: api/games/5/move
        [HttpPost("{id}/move")]
        public async Task<Dto.Game> Move(Guid id, [FromBody] Point position)
        {
            // Make a move on a game
            var game = await Db.GetAsync(id);
            
            // TODO: Turn InvalidOpEx into 400. Possible this is done as an app-wide filter.
            game.Move(position);
            await Db.SaveAsync(game);

            return Mapper.Map<Dto.Game>(game);
        }

    }
}
