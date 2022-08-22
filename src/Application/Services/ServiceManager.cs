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
        private readonly Lazy <ISingleUserService> _singleUserService;
        private readonly Lazy <IAdminUserService> _adminUserService;

        public ServiceManager(
            IRepositoryManager repositoryManager,
            IMapper mapper,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _authenticationService =
            new Lazy<IAuthenticationService>(
                () => new AuthenticationService(repositoryManager, userManager, mapper, configuration));

            _userService = new Lazy<IUserService>(
                    () => new UserService(repositoryManager, userManager, roleManager, mapper, configuration));

            _singleUserService = new Lazy<ISingleUserService>(
                () => new SingleUserService(repositoryManager, mapper, configuration));
            _adminUserService = new Lazy<IAdminUserService>(
                () => new AdminUserService(repositoryManager, mapper, configuration, httpClient));
        }


        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IUserService UserService => _userService.Value;
        public ISingleUserService SingleUserService => _singleUserService.Value;
        public IAdminUserService AdminUserService => _adminUserService.Value;
    }
}
