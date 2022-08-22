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
    [Route("api/v{version:apiVersion}/readlists")]
    public class ReadListController : ControllerBase
    {
       private readonly IServiceManager _service;

        public ReadListController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Endpoint to create user's read list
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles ="User")]
        [HttpPost]
        [Route("create/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<UserReadListDTO>), 200)]
        public async Task<IActionResult> CreateReadList(Guid id, [FromBody] CreateReadListDTO model)
        {
            var response = await _service.SingleUserService.CreateReadList(id, model);
            return Ok(response);
        }


        /// <summary>
        /// Endpoint to add books to a user's read list
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles ="User")]
        [HttpPut]
        [Route("add-book/{bookId}")]
        [ProducesResponseType(typeof(SuccessResponse<UserReadListDTO>), 200)]
        public async Task<IActionResult> AddBookToReadList(Guid userid, Guid bookId)
        {
            var response = await _service.SingleUserService.AddBookToLReadist(userid, bookId);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete a book from a user's read list
        /// </summary>
        /// <returns></returns>
        /// 
        [Authorize(Roles ="User")]
        [HttpDelete]
        [Route("delete-book/{bookId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveBookFromReadList(Guid userId, Guid bookId)
        {
            await _service.SingleUserService.RemoveBookFromReadList(userId, bookId);
            return NoContent();
        }

        /// <summary>
        /// Endpoint to rename a user's read list
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles ="User")]
        [HttpPut]
        [Route("rename/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<UserReadListDTO>), 200)]
        public async Task<IActionResult> RenameReadList(Guid id, Guid userId, [FromBody] CreateReadListDTO model)
        {
            var response = await _service.SingleUserService.RenameReadList(id, userId, model);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get readlist by id
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles ="User")]
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<UserReadListDTO>), 200)]
        public async Task<IActionResult> ViewReadList(Guid id, Guid userId)
        {
            var response = await _service.SingleUserService.GetUserReadList(userId, id);
            return Ok(response);
        }

    }
}
