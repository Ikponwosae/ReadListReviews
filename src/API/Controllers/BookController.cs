using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Application.DataTransferObjects;
using Application.Helpers;
using System.Net;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/books")]
    public class BookController : ControllerBase
    {
        private readonly IServiceManager _service;

        public BookController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Endpoint to fetch book from external api and add to a category
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles ="Admin")]
        [HttpPost]
        [Route("fetch")]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<FetchBook>>), 200)]
        public async Task<IActionResult> FromBooksFromExternalList(Guid categoryId)
        {
            var response = await _service.AdminUserService.FetchBooksExternal(categoryId);
            return Ok(response);
        }


        /// <summary>
        /// Endpoint to change a book category
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("change-category")]
        [ProducesResponseType(typeof(SuccessResponse<BookDTO>), 200)]
        public async Task<IActionResult> ChangeBookCategory(Guid bookId, Guid categoryId)
        {
            var response = await _service.AdminUserService.ChangeBookCategory(bookId, categoryId);
            return Ok(response);
        }
        
        /// <summary>
        /// Endpoint to get a book by id
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<BookDTO>), 200)]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var response = await _service.AdminUserService.GetBookById(id);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to add a book
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("add")]
        [ProducesResponseType(typeof(SuccessResponse<ViewBookDTO>), 200)]
        public async Task<IActionResult> AddABook(Guid categoryId, [FromBody] AddBookDTO model)
        {
            var response = await _service.AdminUserService.AddBook(categoryId, model);
            return Ok(response);
        }
        
        /// <summary>
        /// Endpoint to update a book's details
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("update")]
        [ProducesResponseType(typeof(SuccessResponse<BookDTO>), 200)]
        public async Task<IActionResult> UpdateABook(Guid bookId, [FromForm] UpdateBookDTO model)
        {
            var response = await _service.AdminUserService.UpdateBook(bookId, model);
            return Ok(response);
        }

        // <summary>
        /// Endpoint to delete a book
        ///</summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("delete-book/{bookId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteABook(Guid bookId)
        {
            await _service.AdminUserService.DeleteBook(bookId);
            return NoContent();
        }

        /// <summary>
        /// Endpoint to search for book and categories and authors
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("search")]
        [ProducesResponseType(typeof(SearchBooksDTO), 200)]
        public async Task<IActionResult> Search([FromQuery] ResourceParameter search)
        {
            var response = await _service.SingleUserService.SearchBookCategoriesAuthors(search);
            return Ok(response);
        }

        // <summary>
        /// Endpoint to get all the books available
        ///</summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("all")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<BookDTO>>), 200)]
        public async Task<IActionResult> GetAllBooks([FromQuery] ResourceParameter parameters)
        {
            var response = await _service.SingleUserService.GetAllBooks(nameof(GetAllBooks), parameters, Url);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to review a book
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        [Route("reviews/{bookId}")]
        [ProducesResponseType(typeof(SuccessResponse<ReviewDTO>), 200)]
        public async Task<IActionResult> ReviewABook(Guid userId, Guid bookId, [FromBody] CreateReviewDTO model)
        {
            var response = await _service.SingleUserService.ReviewABook(userId, bookId, model);
            return Ok(response);
        }

        // <summary>
        /// Endpoint to get all the reviews for a book
        ///</summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("reviews/{bookId}/all")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<ReviewDTO>>), 200)]
        public async Task<IActionResult> GetBookReviews(Guid bookId, [FromQuery] ResourceParameter parameters)
        {
            var response = await _service.SingleUserService.GetBookReviews(bookId, nameof(GetBookReviews), parameters, Url);
            return Ok(response);
        }
        
        // <summary>
        /// Endpoint to get a review for a book
        ///</summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("reviews/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<ReviewDTO>), 200)]
        public async Task<IActionResult> GetAReviewById(Guid id)
        {
            var response = await _service.SingleUserService.GetReviewById(id);
            return Ok(response);
        }

        // <summary>
        /// Endpoint to delete a user's review
        ///</summary>
        /// <returns></returns>
        [Authorize(Roles = "User, Admin")]
        [HttpDelete]
        [Route("reviews/delete/{reviewId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteReview(Guid userId, Guid reviewId)
        {
            await _service.SingleUserService.DeleteReview(userId, reviewId);
            return NoContent();
        }
    }
}
