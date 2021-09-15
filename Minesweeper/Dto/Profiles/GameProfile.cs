using AutoMapper;
using System.Collections.Generic;

namespace Minesweeper.Dto.Profiles
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            CreateMap<Model.Game, Dto.Game>()
                .ForMember(o => o.Uncovered, map => map.MapFrom<FieldDataResolver, IReadOnlyDictionary<System.Drawing.Point, byte>>(o => o.Uncovered));
        }
    }
}
