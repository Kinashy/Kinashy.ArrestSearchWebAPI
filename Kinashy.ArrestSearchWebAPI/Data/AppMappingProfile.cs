using AutoMapper;
using AutoMapper.Internal.Mappers;
using Kinashy.ArrestSearchWebAPI.Data.DTO;
using Kinashy.ArrestSearchWebAPI.Data.Library;

namespace Kinashy.ArrestSearchWebAPI.Data
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile() 
        {
            CreateMap<BatchProperty, PropertyDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            CreateMap<ComplectProperty, PropertyDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            CreateMap<PropertyDTO, BatchProperty>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));
                //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            CreateMap<PropertyDTO, ComplectProperty>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));
                //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            CreateMap<Batch, BatchMinimalDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ComplectCount, opt => opt.MapFrom(src => src.Complects.Count))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateUpload))
                .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => src.Properties));
            CreateMap<ComplectDTO, Complect>()
                //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.DocumentPath))
                //.ForMember(dest => dest.DateUpload, opt => opt.MapFrom(src => src.DateCreated))
                .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => src.Properties));
            CreateMap<Complect, ComplectDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DocumentPath, opt => opt.MapFrom(src => src.ImagePath))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateUpload))
                .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => src.Properties));
            CreateMap<BatchDTO, Batch>()
                //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                //.ForMember(dest => dest.DateUpload, opt => opt.MapFrom(src => src.DateCreated))
                .ForMember(dest => dest.Complects, opt => opt.MapFrom(src => src.Complects))
                .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => src.Properties));
            CreateMap<Batch, BatchDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateUpload))
                .ForMember(dest => dest.Complects, opt => opt.MapFrom(src => src.Complects))
                .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => src.Properties));
            CreateMap<RequiredProperties.RequiredProperty, PropertyDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));
        }
    }
}
