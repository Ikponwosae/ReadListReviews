using Application.Helpers;
using Application.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace Application.Contracts
{
    public interface IAdminUserService
    {
       Task<SuccessResponse<CategoryDTO>> CreateCategory(Guid userId, CreateCategoryDTO model);
        Task<SuccessResponse<IEnumerable<CategoryDTO>>> GetAllCategories();
        Task<PagedResponse<IEnumerable<BookDTO>>> GetAllBooksInACategory(Guid categoryId, string actionName, ResourceParameter parameter, IUrlHelper urlHelper);
        Task<SuccessResponse<IEnumerable<FetchBook>>> FetchBooksExternal(Guid categoryId);
        Task<SuccessResponse<BookDTO>> ChangeBookCategory(Guid bookId, Guid categoryId);
        Task<SuccessResponse<BookDTO>> GetBookById(Guid bookId);
        Task<SuccessResponse<ViewBookDTO>> AddBook(Guid categoryId, AddBookDTO model); 
        Task<SuccessResponse<BookDTO>> UpdateBook(Guid bookId, UpdateBookDTO model);
        Task DeleteBook(Guid bookId);
        Task DeleteCategory(Guid categoryId);
    }
}
