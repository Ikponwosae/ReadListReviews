using Application.DataTransferObjects;
using Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Application.Contracts
{
    public interface ISingleUserService
    {
        Task<SuccessResponse<UserReadListDTO>> CreateReadList(Guid userId, CreateReadListDTO model);
        Task<SuccessResponse<UserReadListDTO>> AddBookToLReadist(Guid userId, Guid bookId);
        Task RemoveBookFromReadList(Guid userId, Guid bookId);
        Task <SuccessResponse<UserReadListDTO>> RenameReadList(Guid readListId, Guid userId, CreateReadListDTO model);
        Task<SuccessResponse<UserReadListDTO>> GetUserReadList(Guid userId, Guid readListId);
        Task<SuccessResponse<AllReadListDTO>> GetUserReadLists(Guid userId);
        Task<SearchBooksDTO> SearchBookCategoriesAuthors(ResourceParameter search);
        Task<PagedResponse<IEnumerable<BookDTO>>> GetAllBooks(string actionName, ResourceParameter parameters, IUrlHelper urlHelper);
        Task<PagedResponse<IEnumerable<ReviewDTO>>> GetBookReviews(Guid bookId, string actionName, ResourceParameter parameter, IUrlHelper urlHelper);
        Task<SuccessResponse<ReviewDTO>> ReviewABook(Guid userId, Guid bookId, CreateReviewDTO model);
        Task<SuccessResponse<ReviewDTO>> GetReviewById(Guid reviewId);
        Task DeleteReview(Guid userId, Guid reviewId);
    }
}