using Application.Contracts;
using Application.DataTransferObjects;
using Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    public class UserController : ControllerBase
    {
        private readonly IServiceManager _service;

        public UserController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Endpoint to get a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<UserDTO>), 200)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var response = await _service.UserService.GetUserById(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to register a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        [ProducesResponseType(typeof(SuccessResponse<UserDTO>), 200)]
        public async Task<IActionResult> UserRegistration([FromBody] UserCreateDTO model)
        {
            var response = await _service.UserService.UserRegistration(model);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get edit user's details
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<UserDTO>), 200)]
        public async Task<IActionResult> UpdateAUser(Guid id, [FromForm] UpdateUserDTO model)
        {
            var response = await _service.UserService.UpdateAUser(id, model);
            return Ok(response);
        }
    }
}
