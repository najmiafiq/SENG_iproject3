using AutoMapper;

using TEMMU.API.Models;
using TEMMU.Core.Entities;

namespace TEMMU.API.Profiles
{
    public class FighterProfile : Profile
    {
        public FighterProfile()
        {
            // CreateMap<Source, Destination>();
            CreateMap<FighterCharacterCreationDTO, FighterCharacter>();

            // Map domain entity to Read DTO
            // Need custom logic to calculate winRate

            CreateMap<FighterCharacter, FighterCharacterReadDTO>()
                .ForMember(dest => dest.winRate,opt => opt.MapFrom(src => 
                    src.matchesPlayed > 0 ? (double)src.wins / src.matchesPlayed : 0.0));

            // Mapping for PUT updates
            CreateMap<FighterCharacterCreationDTO, FighterCharacter>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                srcMember != null)); //Skip null/empty fields
        }
    }
}
