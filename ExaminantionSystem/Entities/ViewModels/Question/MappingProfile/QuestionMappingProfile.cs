using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Choice;
using ExaminantionSystem.Entities.Dtos.Ouestion;
using ExaminantionSystem.Entities.ViewModels.Choice;

namespace ExaminantionSystem.Entities.ViewModels.Question.MappingProfile
{
    public class QuestionMappingProfile :Profile
    {
        public QuestionMappingProfile()
        {
            // ViewModel to DTO mappings
            CreateMap<CreateQuestionVM, CreateQuestionDto>();
            CreateMap<UpdateQuestionVM, UpdateQuestionDto>();
            CreateMap<CreateChoiceVM, CreateChoiceDto>();

            // DTO to ViewModel mappings
            CreateMap<QuestionDto, QuestionVM>();
            CreateMap<ChoiceDto, ChoiceVM>();

            // Entity to ViewModel mappings
            CreateMap<Models.Question, QuestionVM>()
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Course.Instructor.User.FullName));

            CreateMap<Models.Choice, ChoiceVM>();
        }
    }
}
