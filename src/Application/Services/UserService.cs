using Application.Contracts;
using Application.DataTransferObjects;
using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryManager _repository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(IRepositoryManager repository, UserManager<User> userManager,
        RoleManager<Role> roleManager, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<SuccessResponse<UserDTO>> UserRegistration(UserCreateDTO model)
        {
            var email = model.Email.Trim().ToLower();

            var userEntity = await _repository.User.Get(x => x.Email == email).FirstOrDefaultAsync();

            if (userEntity != null)
                throw new RestException(HttpStatusCode.BadRequest, "This email is already in use");

            var user = _mapper.Map<User>(model);
            user.UserName = model.Email;
            user.Password = _userManager.PasswordHasher.HashPassword(user, model.Password);
            user.EmailConfirmed = true;
            user.Status = EUserStatus.Active.ToString();
            user.CreatedAt = DateTime.UtcNow;
            user.Role = EUserRole.User.ToString();
            user.Status = EUserStatus.Active.ToString();
            user.LastLogin = DateTime.UtcNow;

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                throw new RestException(HttpStatusCode.InternalServerError, "Internal server Error");

            if (!await _userManager.IsInRoleAsync(user, user.Role))
                await _userManager.AddToRoleAsync(user, user.Role);

            var token = CustomToken.GenerateRandomString(128);
            var tokenEntity = new Token
            {
                UserId = user.Id,
                Value = token,
                TokenType = "User Registered"
            };

            await _repository.Token.AddAsync(tokenEntity);

            await _repository.SaveChangesAsync();

            var response = _mapper.Map<UserDTO>(user);

            return new SuccessResponse<UserDTO>
            {
                Data = response,
                Message = "User Successfully Registered"
            };
        }

        public async Task<SuccessResponse<UserDTO>> GetUserById(Guid userId)
        {
            var userEntity = await _repository.User.Get(x => x.Id == userId).FirstOrDefaultAsync();

            if (userEntity == null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            var user = _mapper.Map<UserDTO>(userEntity);

            return new SuccessResponse<UserDTO>
            {
                Data = user,
                Message = "User data retrieved"
            };
        }

        public async Task<SuccessResponse<UserDTO>> UpdateAUser(Guid userId, UpdateUserDTO model)
        {
            var userEntity = await _repository.User.Get(x => x.Id == userId).FirstOrDefaultAsync();

            if (userEntity == null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            var updatedUser = _mapper.Map<User>(model);
            updatedUser.UpdatedAt = DateTime.UtcNow;
            updatedUser.Role = EUserRole.User.ToString();

            _repository.User.Update(updatedUser);

            await _repository.SaveChangesAsync();

            var response = _mapper.Map<UserDTO>(userEntity);

            return new SuccessResponse<UserDTO>
            {
                Data = response,
                Message = "User Data Updated Successfully"
            };
        }
    }
}
