using Application.DataTransferObjects;
using Application.Helpers;

namespace Application.Contracts
{
    public interface ISingleUserService
    {
        Task<SuccessResponse<UserReadListDTO>> CreateReadList(Guid userId, CreateReadListDTO model);
         //Task<SuccessResponse<UserReadListDTO>> AddBookToLReadist(Guid bookId);
         //Task<SuccessResponse<UserReadListDTO>> RemoveBookFromReadList(UserReadListDTO model);
    }
}