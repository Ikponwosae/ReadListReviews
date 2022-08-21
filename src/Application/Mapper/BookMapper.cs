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
        }
    }
}
