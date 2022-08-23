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
    [Route("api/v{version:apiVersion}/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly IServiceManager _service;

        public CategoryController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Endpoint to create a new book category
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(typeof(SuccessResponse<CategoryDTO>), 200)]
        public async Task<IActionResult> CreateNewCategory(Guid id, [FromBody] CreateCategoryDTO model)
        {
            var response = await _service.AdminUserService.CreateCategory(id, model);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get the list of categories
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("all")]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<CategoryDTO>>), 200)]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = await _service.AdminUserService.GetAllCategories();
            return Ok(response);
        }

        // <summary>
        /// Endpoint to get the list of books in a category
        ///</summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("{id}/books")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<BookDTO>>), 200)]
        public async Task<IActionResult> GetAllBooksInACategory(Guid id, [FromQuery] ResourceParameter parameters)
        {
            var response = await _service.AdminUserService.GetAllBooksInACategory(id, nameof(GetAllBooksInACategory), parameters, Url);
            return Ok(response);
        }

        // <summary>
        /// Endpoint to delete a category
        ///</summary>
        /// <returns></returns>
        /// 
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("delete/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteACategory(Guid id)
        {
            await _service.AdminUserService.DeleteCategory(id);
            return NoContent();
        }

    }
}
