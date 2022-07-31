using Application.Contracts;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IUserService> _userService;

        public ServiceManager(
            IRepositoryManager repositoryManager,
            IMapper mapper,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IConfiguration configuration)
        {
            _authenticationService =
            new Lazy<IAuthenticationService>(
                () => new AuthenticationService(repositoryManager, userManager, mapper, configuration));

            _userService = new Lazy<IUserService>(
                    () => new UserService(repositoryManager, userManager, roleManager, mapper, configuration));
        }


        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IUserService UserService => _userService.Value;
    }
}
