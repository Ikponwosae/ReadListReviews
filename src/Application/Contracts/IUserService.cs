using Application.DataTransferObjects;
using Application.Helpers;

namespace Application.Contracts
{
    public interface IUserService
    {
        Task<SuccessResponse<UserDTO>> UserRegistration(UserCreateDTO model);
        Task<SuccessResponse<UserDTO>> GetUserById(Guid userId);
        //Task<PagedResponse<IEnumerable<UserDTO>>> GetUsers(UserParameters parameters, string actionName, IUrlHelper urlHelper);
        Task<SuccessResponse<UserDTO>> UpdateAUser(Guid userId, UpdateUserDTO model);
    }
}
