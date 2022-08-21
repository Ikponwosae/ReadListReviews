using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Application.DataTransferObjects;
using Application.Helpers;

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
        /// Endpoint to invite a new user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("create/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<UserReadListDTO>), 200)]
        public async Task<IActionResult> CreateReadList(Guid id, [FromBody] CreateReadListDTO model)
        {
            var response = await _service.SingleUserService.CreateReadList(id, model);
            return Ok(response);
        }

    }
}
