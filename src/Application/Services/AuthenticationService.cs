using Application.Contracts;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepositoryManager _repository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthenticationService(IRepositoryManager repository, UserManager<User> userManager, 
            IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
        }
    }
}
