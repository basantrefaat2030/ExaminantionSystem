using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Choice;
using ExaminantionSystem.Entities.Dtos.Ouestion;

namespace ExaminantionSystem.Entities.Dtos.Course.MappingProfile
{
    public class CourseServiceMappingProfile : Profile
    {

        public CourseServiceMappingProfile()
        {
            // Entity to DTO mappings
            CreateMap<Models.Course, CourseDto>()
                .ForMember(dest => dest.Hours, opt => opt.MapFrom(src => src.Hours.Value)).ReverseMap();

            CreateMap<Models.Course, CourseInformationDto>()
                .ForMember(dest => dest.Hours, opt => opt.MapFrom(src => src.Hours.Value))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Instructor.FullName)).ReverseMap();

            CreateMap<Models.Course, CourseDetailsDto>()
                .ForMember(dest => dest.Hours, opt => opt.MapFrom(src => src.Hours.Value))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Instructor.FullName)).ReverseMap();

            // Question and Choice mappings
            CreateMap<Models.Question, QuestionPoolDto>()
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.QuestionLevel)).ReverseMap();

            CreateMap<Models.Choice, QuestionChoicesPoolDto>().ReverseMap();

            CreateMap<CreateCourseDto, Models.Course>()
                .ForMember(dest => dest.Hours.Value, opt => opt.MapFrom(src => src.Hours));

            CreateMap<UpdateCourseDto, Models.Course>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.courseId)).ReverseMap();
        }

    }
}
