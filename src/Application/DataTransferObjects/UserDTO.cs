using System;
//using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.DataTransferObjects
{
    public record UserCreateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string Password { get; set; }
    }
    public record UserLoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public record RefreshTokenDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
    public record ResetPasswordDTO
    {
        public string Email { get; set; }
    }

    public record SetPasswordDTO
    {
        public string Password { get; set; }
        public string Token { get; set; }
    }

    public record AuthDTO
    {
        public string AccessToken { get; set; }
        public DateTime? ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }

    public record UserDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? Gender { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public DateTime LastLogin { get; set; }
    }

    public record UpdateUserDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
    }

    //public record UserResetPasswordDTO
    //{
    //    public Token Token { get; set; }
    //}
}
