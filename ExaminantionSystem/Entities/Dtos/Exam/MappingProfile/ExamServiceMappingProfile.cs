using AutoMapper;
using ExaminantionSystem.Entities.Models;

namespace ExaminantionSystem.Entities.Dtos.Exam.MappingProfile
{
    public class ExamServiceMappingProfile:Profile
    {

        public ExamServiceMappingProfile()
        {
            // Entity to DTO mappings
            CreateMap<Models.Exam, ExamDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ExamType))
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title));

            CreateMap<Models.Exam, ExamWithQuestionsDto>()
                .ForMember(dest => dest.ExamId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ExamType))
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Course.Instructor.User.FullName))
                .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.NumberOfQuestion));

            CreateMap<Question, ExamQuestionDto>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.QuestionLevel));

            CreateMap<Models.Choice, ExamWithQuestionsChoicesDto>();

            // DTO to Entity mappings
            CreateMap<CreateExamDto, Models.Exam>()
                .ForMember(dest => dest.ExamType, opt => opt.MapFrom(src => src.Type));

            CreateMap<UpdateExamDto, Models.Exam>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExamId));
        }
    }
}
