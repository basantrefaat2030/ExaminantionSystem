using AutoMapper;
using ExaminantionSystem.Entities.ViewModels;
using ExaminantionSystem.Entities.Wrappers;

namespace ExaminantionSystem.MappingProfile
{
    public class GeneralMapping :Profile
    {
        public GeneralMapping()
        {
            CreateMap<ErrorDetail, ErrorDetailViewModel>();

            // Map ErrorResponse to ErrorResponseViewModel
            CreateMap<ErrorResponse, ErrorResponseViewModel>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeDescription))
                .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => src.StatusCode));

            // Map Response<T> to ResponseViewModel<T>
            CreateMap(typeof(Response<>), typeof(ResponseViewModel<>))
                .ForMember("Succeeded", opt => opt.MapFrom("Succeeded"))
                .ForMember("Message", opt => opt.MapFrom("Message"))
                .ForMember("Error", opt => opt.MapFrom("Error"))
                .ForMember("Data", opt => opt.MapFrom("Data"));

            // Map PagedResponse<T> to PagedResponseViewModel<T>
            CreateMap(typeof(PagedResponse<>), typeof(PagedResponseViewModel<>))
                .ForMember("Data", opt => opt.MapFrom("Data"))
                .ForMember("PageNumber", opt => opt.MapFrom("PageNumber"))
                .ForMember("PageSize", opt => opt.MapFrom("PageSize"))
                .ForMember("TotalPages", opt => opt.MapFrom("TotalPages"))
                .ForMember("TotalRecords", opt => opt.MapFrom("TotalRecords"));
        }
    }
}
