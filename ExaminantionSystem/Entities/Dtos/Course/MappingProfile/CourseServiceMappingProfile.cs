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
                .ForMember(dest => dest.Hours, opt => opt.MapFrom(src => src.Hours.Value))
                .ForMember(dest => dest.Budget, opt => opt.MapFrom(src => src.Budget));

            CreateMap<Models.Course, CourseInformationDto>()
                .ForMember(dest => dest.Hours, opt => opt.MapFrom(src => src.Hours.Value))
                .ForMember(dest => dest.Budget, opt => opt.MapFrom(src => src.Budget))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Instructor.FullName));

            CreateMap<Models.Course, CourseDetailsDto>()
                .ForMember(dest => dest.Hours, opt => opt.MapFrom(src => src.Hours.Value))
                .ForMember(dest => dest.Budget, opt => opt.MapFrom(src => src.Budget))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Instructor.FullName));

            // Question and Choice mappings
            CreateMap<Models.Question, QuestionPoolDto>()
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.QuestionLevel))
                .ForMember(dest => dest.ChoiceCount, opt => opt.MapFrom(src => src.Choices.Count(c => !c.IsDeleted)));

            CreateMap<Models.Choice, QuestionChoicesPoolDto>();

            // Create DTO to Entity
            CreateMap<CreateCourseDto, Models.Course>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.Hours, opt => opt.MapFrom(src => (byte?)src.Hours));

            // Update DTO to Entity
            CreateMap<UpdateCourseDto, Models.Course>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.courseId))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Hours, opt => opt.MapFrom(src => (byte?)src.Hours));
        }

    }
}
