using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Application.DataTransferObjects;
using Application.Helpers;
using System.Net;

namespace API.Controllers
{
    [Authorize]
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
        //[AllowAnonymous]
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
        //[Authorize(Roles ="Admin")]
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
        /// Endpoint to get add a book
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles ="Admin")]
        [HttpPost]
        [Route("add")]
        [ProducesResponseType(typeof(SuccessResponse<ViewBookDTO>), 200)]
        public async Task<IActionResult> AddABook(Guid categoryId, [FromBody] AddBookDTO model)
        {
            var response = await _service.AdminUserService.AddBook(categoryId, model);
            return Ok(response);
        }
    }
}
