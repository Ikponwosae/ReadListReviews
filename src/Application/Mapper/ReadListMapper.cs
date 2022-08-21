using AutoMapper;
using Application.DataTransferObjects;
using Domain.Entities;

namespace Application.Mapper
{
    public class ReadListMapper : Profile
    {
        public ReadListMapper()
        {
            CreateMap<CreateReadListDTO, ReadList>();
            CreateMap<ReadList, UserReadListDTO>()
                .ForMember(dest => dest.Books, opt => opt.MapFrom(src => src.Books))
                .ForMember(dest => dest.ReadListId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Owner.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Owner.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Owner.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Owner.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Owner.PhoneNumber))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Owner.Status))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Owner.Role))
                .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.Owner.LastLogin));
        }
    }
}
