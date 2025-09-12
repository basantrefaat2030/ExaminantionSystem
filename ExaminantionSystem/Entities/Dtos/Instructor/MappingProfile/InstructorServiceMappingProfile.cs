using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Course;
using ExaminantionSystem.Entities.Dtos.Exam;
using ExaminantionSystem.Entities.Dtos.Student;
using ExaminantionSystem.Entities.Models;

namespace ExaminantionSystem.Entities.Dtos.Instructor.MappingProfile
{
    public class InstructorServiceMappingProfile :Profile
    {
        public InstructorServiceMappingProfile()
        {
            CreateMap<StudentCourse, StudentCourseDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.FullName))
            .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title));

            CreateMap<ExamResult, StudentExamResultDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.FullName))
                .ForMember(dest => dest.ExamTitle, opt => opt.MapFrom(src => src.Exam.Title))
                .ForMember(dest => dest.ExamType, opt => opt.MapFrom(src => src.Exam.ExamType.ToString()));

            CreateMap<Models.Exam, ManualExamDto>()
                .ForMember(dest => dest.ExamId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ExamType, opt => opt.MapFrom(src => src.ExamType.ToString()))
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.NumberOfQuestion))
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.ExamQuestions.Where(eq => !eq.IsDeleted && eq.IsActive).Select(eq => eq.Question)));

            // Question and Choice mappings (reuse from other profiles)
            CreateMap<Question, ExamQuestionDto>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.QuestionLevel))
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices.Where(ch=> ch.IsActive && !ch.IsDeleted)));

            CreateMap<Models.Choice, ExamWithQuestionsChoicesDto>();
        }
    }
}
