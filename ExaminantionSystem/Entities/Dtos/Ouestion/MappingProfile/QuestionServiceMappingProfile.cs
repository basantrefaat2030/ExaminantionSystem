using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Choice;
using ExaminantionSystem.Entities.Models;

namespace ExaminantionSystem.Entities.Dtos.Ouestion.MappingProfile
{
    public class QuestionServiceMappingProfile :Profile
    {
        public QuestionServiceMappingProfile() 
        {
            // Entity to DTO mappings
            CreateMap<Question, QuestionDto>()
                .ForMember(dest => dest.QuestionContent, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.QuestionLevel))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Course.Instructor.User.FullName)).ReverseMap();

            //CreateMap<Question, QuestionDto>()
            //    .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            //    .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.QuestionLevel))
            //    .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
            //    .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Course.Instructor.FullName));

            CreateMap<Models.Choice, ChoiceDto>();

            CreateMap<CreateQuestionDto, Question>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content.Trim()))
                .ForMember(dest => dest.QuestionLevel, opt => opt.MapFrom(src => src.Level));

            //CreateMap<CreateChoiceDto, Models.Choice>()
            //    .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text.Trim()))
            //    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            //    .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

        }
    }
}
