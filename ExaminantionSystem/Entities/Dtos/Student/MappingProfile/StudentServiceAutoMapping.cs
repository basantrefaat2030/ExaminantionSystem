using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Course;
using ExaminantionSystem.Entities.Dtos.Exam;
using ExaminantionSystem.Entities.Models;

namespace ExaminantionSystem.Entities.Dtos.Student.MappingProfile
{
    public class StudentServiceAutoMapping :Profile
    {
        public StudentServiceAutoMapping()
        {
            CreateMap<StudentCourse, StudentEnrollmentDto>()
           .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.FullName))
           .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
           .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Course.Instructor.User.FullName));

            CreateMap<ExamResult, StudentExamDto>()
                .ForMember(dest => dest.ExamResultId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ExamTitle, opt => opt.MapFrom(src => src.Exam.Title))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.FullName))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Exam.Course.Title))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Exam.Duration));

            CreateMap<ExamResult, ExamResultDto>()
                .ForMember(dest => dest.examInfomationDto, opt => opt.MapFrom(src => new ExamInfomationDto
                {
                    ExamId = src.Exam.Id,
                    ExamTitle = src.Exam.Title,
                    CourseName = src.Exam.Course.Title
                }))
                .ForMember(dest => dest.studentExamInformation, opt => opt.MapFrom(src => new StudentExamInformation
                {
                    StudentId = src.Student.Id,
                    StudentName = src.Student.User.FullName
                }))
                .ForMember(dest => dest.ExamStartAt, opt => opt.MapFrom(src => src.StartedAt.Value))
                .ForMember(dest => dest.ExamCompletedAt, opt => opt.MapFrom(src => src.SubmittedAt.Value))
                .ForMember(dest => dest.QuestionResults, opt => opt.Ignore()); // Will be set manually

            CreateMap<StudentAnswer, StudentAnswerDto>().ReverseMap();

        }
    }
}
