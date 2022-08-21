using Application.DataTransferObjects;
using Application.Helpers;
using Application.Contracts;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contracts;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Application.Services
{
    public class SingleUserService : ISingleUserService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public SingleUserService(IRepositoryManager repository, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<SuccessResponse<UserReadListDTO>> CreateReadList(Guid userId, CreateReadListDTO model)
        {
            var user = await _repository.User.GetByIdAsync(userId);

            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            if (user.ReadList != null)
            {
                throw new RestException(HttpStatusCode.BadRequest, "User already has readlist");
            }
           
            var userReadList = _mapper.Map<ReadList>(model);
            userReadList.UserId = userId;
            await _repository.ReadList.AddAsync(userReadList);

            user.ReadList = userReadList;
            _repository.User.Update(user);
            await _repository.SaveChangesAsync();
            
            return new SuccessResponse<UserReadListDTO>
            {
                Data = _mapper.Map<UserReadListDTO>(userReadList),
                Message = "Read List Created"
            };
        }

        //public async Task<SuccessResponse<UserReadListDTO>> AddBookToLReadist(Guid bookId)
        //{
        //    var book = await _repository.Book.GetByIdAsync(bookId);

        //}

    }
}
