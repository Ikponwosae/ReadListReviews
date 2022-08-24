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
            CreateMap<Book, BookDTO>()
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews));
            //.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.CategoryId));
            CreateMap<AddBookDTO, Book>();
            CreateMap<UpdateBookDTO, Book>()
                .ForMember(dest => dest.BookImage, opt => opt.Ignore());
            CreateMap<Photo, BookImageDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            CreateMap<Review, ViewReviewDTO>();
            CreateMap<ICollection<Review>, ICollection<ViewReviewDTO>>();
            CreateMap<Review, ReviewDTO>();
            CreateMap<CreateReviewDTO, Review>();
        }
    }
}
