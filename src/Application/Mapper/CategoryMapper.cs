using AutoMapper;
using Application.DataTransferObjects;
using Domain.Entities;

namespace Application.Mapper
{
    public class CategoryMapper : Profile
    {
        public CategoryMapper()
        {
            CreateMap<CreateCategoryDTO, Category>().ReverseMap();
            CreateMap<Category, CategoryDTO>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Books, opt => opt.MapFrom(src => src.Books));
        }
    }
}
