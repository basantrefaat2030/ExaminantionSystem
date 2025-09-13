using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Exam;
using ExaminantionSystem.Entities.ViewModels.Exam;

namespace ExaminantionSystem.Entities.Models.MappingProfile
{
    public class ExamAutoMapperProfile : Profile
    {
        public ExamAutoMapperProfile() 
        {
            // ViewModel to DTO mappings
            CreateMap<CreateExamVM, CreateExamDto>();
            CreateMap<UpdateExamVM, UpdateExamDto>();

            // DTO to ViewModel mappings
            CreateMap<ExamDto, ExamVM>();
            CreateMap<ExamWithQuestionsDto, ExamWithQuestionsVM>();
            CreateMap<ExamQuestionDto, ExamQuestionVM>();
            CreateMap<ExamWithQuestionsChoicesDto, ExamChoiceVM>();

            // Entity to ViewModel mappings
            CreateMap<Exam, ExamVM>()
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title));

            CreateMap<Exam, ExamWithQuestionsVM>()
                .ForMember(dest => dest.ExamId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Course.Instructor.User.FullName))
                .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.ExamQuestions.Count(eq => !eq.IsDeleted && eq.IsActive)));


        }
    }
}
