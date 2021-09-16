using AutoMapper;
using Minesweeper.Model;
using System.Linq;

namespace Minesweeper.Dto.Profiles
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            CreateMap<Model.Game, Dto.Game>()
                .ForMember(o => o.Mines, map => map.MapFrom(src => src.Mines.Count()))
                .ForMember(o => o.FieldState, map => map.MapFrom(src => FieldStateConverter.GetString(src.FieldState)));
        }

    }
}
