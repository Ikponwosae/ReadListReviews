using AutoMapper;
using Application.DataTransferObjects;
using Domain.Entities;

namespace Application.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<UserCreateDTO, User>();
            CreateMap<User, UserDTO>();
            CreateMap<UpdateUserDTO, User>();
        }
    }
}
