using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Choice;

namespace ExaminantionSystem.Entities.ViewModels.Choice.MappingProfile
{
    public class ChoiceMappingProfile :Profile
    {
        public ChoiceMappingProfile()
        {
            // ViewModel to DTO mappings
            CreateMap<CreateChoiceVM, CreateChoiceDto>();
            CreateMap<UpdateChoiceVM, UpdateChoiceDto>();

            // DTO to ViewModel mappings
            CreateMap<ChoiceDto, ChoiceVM>();

            // Entity to ViewModel mappings
            CreateMap<Models.Choice, ChoiceVM>();
        }
    }
}
