using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Course;
using ExaminantionSystem.Entities.Dtos.Exam;
using ExaminantionSystem.Entities.Dtos.Student;
using ExaminantionSystem.Entities.ViewModels.Exam;
using ExaminantionSystem.Entities.ViewModels.Student;
using ExaminantionSystem.Entities.Wrappers;

namespace ExaminantionSystem.Entities.ViewModels.Instructor.MappingProfile
{
    public class InstructorMappingProfile :Profile
    {
        public InstructorMappingProfile() 
        {
            CreateMap<ApproveEnrollmentVM, StudentCourseDto>().ReverseMap();
            CreateMap<RejectEnrollmentVM, StudentCourseDto>().ReverseMap();

            CreateMap<StudentCourseDto, EnrollmentVM>();

            CreateMap<ManualExamDto, ManualExamVM>().ReverseMap();
            CreateMap<StudentExamResultDto, StudentResultVM>();
            CreateMap<StudentExamResultDto, StudentResultVM>();

            // Request to DTO mappings
            CreateMap<AddManualExamVM, AddManualExamDto>();




        }
    }
}
