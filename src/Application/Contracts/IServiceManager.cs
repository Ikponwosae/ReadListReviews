using Application.Contracts;

namespace Application.Contracts
{
    public interface IServiceManager
    {
        IAuthenticationService AuthenticationService { get; }
        IUserService UserService { get; }
    }
}
