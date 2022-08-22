using AutoMapper;
using Application.DataTransferObjects;
using Domain.Entities;

namespace Application.Mapper
{
    public class BookMapper : Profile
    {
        public BookMapper()
        {
            CreateMap<Book, ViewBookDTO>();
            CreateMap<FetchBook, Book>();
            CreateMap<FetchBooksDTO, FetchBook>();
            CreateMap<Book, BookDTO>();
            //.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.CategoryId));
            CreateMap<AddBookDTO, Book>();
        }
    }
}
