using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Course;
using ExaminantionSystem.Entities.Dtos.Exam;
using ExaminantionSystem.Entities.Dtos.Ouestion;
using ExaminantionSystem.Entities.Dtos.Student;
using ExaminantionSystem.Entities.ViewModels.Exam;

namespace ExaminantionSystem.Entities.ViewModels.Student.MappingProfile
{
    public class StudentMappingProfile : Profile
    {
        public StudentMappingProfile()
        {
            CreateMap<StudentEnrollmentDto, StudentEnrollmentVM>().ReverseMap();
            CreateMap<StudentExamVM, StudentExamDto>().ReverseMap();
            CreateMap<ExamResultDto, ExamResultVM>().ReverseMap();
            CreateMap<StudentExamInformationDto, StudentExamInformationVM>().ReverseMap();
            CreateMap<ExamInfomationDto, ExamInfomationVM>().ReverseMap();
            CreateMap<EvaluateQuestionResultDto, EvaluateQuestionResultVM>().ReverseMap();
            CreateMap<SubmitStudentAnswerVM, SubmitStudentAnswerDto>().ReverseMap();
            CreateMap<StudentAnswerDto, StudentAnswerVM>().ReverseMap();

        }
    }
}
