using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Exam;

namespace ExaminantionSystem.Entities.ViewModels.Instructor.MappingProfile
{
    public class InstructorMappingProfile :Profile
    {
        public InstructorMappingProfile() 
        {
            CreateMap<Models.Exam, ExamDto>()
              .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ExamType));



        }
    }
}
