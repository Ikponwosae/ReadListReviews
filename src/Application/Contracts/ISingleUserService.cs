using Application.DataTransferObjects;
using Application.Helpers;

namespace Application.Contracts
{
    public interface ISingleUserService
    {
        Task<SuccessResponse<UserReadListDTO>> CreateReadList(Guid userId, CreateReadListDTO model);
        Task<SuccessResponse<UserReadListDTO>> AddBookToLReadist(Guid userId, Guid bookId);
        Task RemoveBookFromReadList(Guid userId, Guid bookId);
        Task <SuccessResponse<UserReadListDTO>> RenameReadList(Guid readListId, Guid userId, CreateReadListDTO model);
        Task<SuccessResponse<UserReadListDTO>> GetUserReadList(Guid userId, Guid readListId);
        Task<SearchBooksDTO> SearchBookCategoriesAuthors(ResourceParameter search);
    }
}