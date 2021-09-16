using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Fetches all the unfinished <see cref="Dto.Game"/> of the user.
        /// </summary>
        [Authorize]
        [HttpGet(Name = "GetAll")]
        public async Task<IEnumerable<Dto.Game>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fetches a <see cref="Dto.Game"/> by it's ID.
        /// </summary>
        /// <param name="id">The ID of the game looked for.</param>
        [HttpGet("{id}", Name = "GetById")]
        public async Task<Dto.Game> GetByIdAsync(Guid id)
        {
            var game = await Db.GetAsync(id);
            return Mapper.Map<Dto.Game>(game);
        }

        /// <summary>
        /// Starts a new <see cref="Dto.Game"/>.
        /// </summary>
        /// <param name="creation">Parameters of the new game.</param>
        [HttpPost(Name = "Post")]
        public async Task<Dto.Game> PostAsync([FromBody] Dto.GameCreation creation)
        {
            // Create a new game (and make the first move)
            var game = Game.Create(creation.Field, creation.Mines);

            // TODO: It's not good UX if the first move ends in gameover...
            game.Move(creation.Move);
            await Db.SaveAsync(game);

            return Mapper.Map<Dto.Game>(game);
        }

        /// <summary>
        /// Makes a move on a <see cref="Dto.Game"/>.
        /// </summary>
        /// <param name="id">The ID of the game looked for.</param>
        /// <param name="position">The position played.</param>
        [HttpPost("{id}/move")]
        public async Task<Dto.Game> MoveAsync(Guid id, [FromBody] Dto.Point position)
        {
            // Make a move on a game
            var game = await Db.GetAsync(id);
            if (game == null) return null;

            // TODO: Turn InvalidOpEx into 400. Possible this is done as an app-wide filter.
            game.Move(position);
            await Db.SaveAsync(game);

            return Mapper.Map<Dto.Game>(game);
        }

        /// <summary>
        /// Flags a position on a <see cref="Dto.Game"/>.
        /// </summary>
        /// <param name="id">The ID of the game looked for.</param>
        /// <param name="kind">"red-flag", "question" or "clear".</param>
        /// <param name="position">The position flagged.</param>
        [HttpPost("{id}/flag/{kind}")]
        public async Task<Dto.Game> FlagAsync(Guid id, string kind, [FromBody] Dto.Point position)
        {
            FlagKind? flagKind = null;
            if (StringComparer.InvariantCultureIgnoreCase.Equals(kind, "red-flag"))
            {
                flagKind = FlagKind.RedFlag;
            }
            else if (StringComparer.InvariantCultureIgnoreCase.Equals(kind, "question"))
            {
                flagKind = FlagKind.Tentative;
            }
            else if (!StringComparer.InvariantCultureIgnoreCase.Equals(kind, "clear"))
            {
                return null;
            }

            // Plant a flag
            var game = await Db.GetAsync(id);
            if (game == null) return null;

            game.Flag(position, flagKind);
            await Db.SaveAsync(game);

            return Mapper.Map<Dto.Game>(game);
        }
    }
}
