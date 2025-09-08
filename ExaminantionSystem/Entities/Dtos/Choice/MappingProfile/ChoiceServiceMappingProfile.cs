using AutoMapper;

namespace ExaminantionSystem.Entities.Dtos.Choice.MappingProfile
{
    public class ChoiceServiceMappingProfile:Profile
    {
        public ChoiceServiceMappingProfile()
        {
            // Entity to DTO mappings
            CreateMap<Models.Choice, ChoiceDto>().ReverseMap();

            // DTO to Entity mappings
            CreateMap<CreateChoiceDto, Models.Choice>().ReverseMap();

            CreateMap<UpdateChoiceDto, Models.Choice>().ReverseMap();
        }
    }
}
