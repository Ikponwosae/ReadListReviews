using Application.Contracts;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryManager _repository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(IRepositoryManager repository, UserManager<User> userManager,
        RoleManager<Role> roleManager, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _configuration = configuration;
        }
    }
}
