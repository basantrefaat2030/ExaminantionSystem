using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Exam;

namespace ExaminantionSystem.Entities.ViewModels.Exam.MappingProfile
{
    public class ExamMappingProfile:Profile
    {
        public ExamMappingProfile()
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
            CreateMap<Models.Exam, ExamVM>()
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title));

            CreateMap<Models.Exam, ExamWithQuestionsVM>()
                .ForMember(dest => dest.ExamId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Course.Instructor.User.FullName))
                .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.NumberOfQuestion));

        }
    }
}
