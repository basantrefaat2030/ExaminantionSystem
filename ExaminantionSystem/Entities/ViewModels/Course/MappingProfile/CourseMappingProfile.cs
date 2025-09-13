using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Choice;
using ExaminantionSystem.Entities.Dtos.Course;
using ExaminantionSystem.Entities.Dtos.Ouestion;
using ExaminantionSystem.Entities.ViewModels.Question;
using ExaminantionSystem.Entities.Wrappers;

namespace ExaminantionSystem.Entities.ViewModels.Course.MappingProfile
{
    public class CourseMappingProfile : Profile
    {
        public CourseMappingProfile()
        {
            // ViewModel to DTO mappings
            CreateMap<CreateCourseVM, CreateCourseDto>();
            CreateMap<UpdateCourseVM, UpdateCourseDto>();

            // DTO to ViewModel mappings
            CreateMap<CourseDto, CourseVM>();
            CreateMap<CourseInformationDto, CourseInformationVM>();
            CreateMap<CourseDetailsDto, CourseDetailsVM>();
            CreateMap<QuestionPoolDto, QuestionPoolVM>();
            CreateMap<QuestionChoicesPoolDto, QuestionChoicesPoolVM>();
        }
    }
}
