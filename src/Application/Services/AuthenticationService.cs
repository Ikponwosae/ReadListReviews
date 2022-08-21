using Application.Contracts;
using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.DataTransferObjects;

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepositoryManager _repository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthenticationService(IRepositoryManager repository, UserManager<User> userManager, 
            IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<SuccessResponse<AuthDTO>> Login(UserLoginDTO model)
        {
            var email = model.Email.Trim().ToLower();
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, "Wrong email or password.");

            var validatePassword = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!validatePassword)
                throw new RestException(HttpStatusCode.Unauthorized, "Authentication failed, wrong email or password.");

            if (user.Status == EUserStatus.Disabled.ToString())
                throw new RestException(HttpStatusCode.Unauthorized, "This is a disabled account.");

            user.LastLogin = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            var token = await CreateToken(user, true);
            var registerToken = new Token
            {
                UserId = user.Id,
                Value = token.AccessToken,
                TokenType = "User Logged In"
            };

            await _repository.Token.AddAsync(registerToken);
            await _repository.SaveChangesAsync();

            return new SuccessResponse<AuthDTO>
            {
                Data = token
            };
        }

        public async Task<SuccessResponse<AuthDTO>> RefreshToken(RefreshTokenDTO model)
        {
            var principal = GetPrincipalFromExpiredToken(model.RefreshToken);
            if (principal.Identity != null)
            {
                var user = await _userManager.FindByNameAsync(principal.Identity.Name);
                if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime < DateTime.Now)
                    throw new RestException(HttpStatusCode.InternalServerError, "Invalid Token");
                return new SuccessResponse<AuthDTO>
                {
                    Data = await CreateToken(user, populateExp: false)
                };
            }
            throw new RestException(HttpStatusCode.Unauthorized, "This specified token has expired, please login");
        }

        public async Task<SuccessResponse<object>> ResetPassword(ResetPasswordDTO model)
        {
            var email = model.Email.Trim().ToUpper();
            var user = await _repository.User.Get(x => x.NormalizedEmail == email).FirstOrDefaultAsync();

            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, "User with this email cannot be found");

            var token = new Token
            {
                UserId = user.Id,
                Value = CustomToken.GenerateRandomString(128),
                TokenType = "Reset Password"
            };

            await _repository.Token.AddAsync(token);

            await _repository.SaveChangesAsync();

            return new SuccessResponse<object>
            {
                Data = token
            };
        }

        public async Task<TokenResponse<object>> VerifyToken(string token)
        {
            var tokenEntity = await _repository.Token.FirstOrDefaultAsync(x => x.Value == token);
            if (tokenEntity == null)
                return new TokenResponse<object>
                {
                    Message = "Invalid Token"
                };

            if (DateTime.Now >= tokenEntity.ExpiresAt)
            {
                _repository.Token.Remove(tokenEntity);
                await _repository.SaveChangesAsync();
                return new TokenResponse<object>
                {
                    Message = "Invalid Token"
                };
            }

            var user = await _repository.User.FirstOrDefaultAsync(x => x.Id == tokenEntity.UserId);
            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, "Invalid User");

            return new TokenResponse<object>
            {
                Message = "Valid Token",
                Data = _mapper.Map<UserDTO>(user),
                IsValid = true
            };
        }

        public async Task<SuccessResponse<object>> SetPassword(SetPasswordDTO model)
        {
            var tokenEntity = await _repository.Token.FirstOrDefaultAsync(x => x.Value == model.Token);

            if (tokenEntity == null)
                throw new RestException(HttpStatusCode.BadRequest, "Invalid token");

            var isTokenValid = CustomToken.IsTokenValid(tokenEntity);
            if (!isTokenValid)
                throw new RestException(HttpStatusCode.BadRequest, "Invalid Token");

            var user = await _repository.User.GetByIdAsync(tokenEntity.UserId);
            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, "User cannot be found");

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
            user.EmailConfirmed = true;
            user.Status = EUserStatus.Active.ToString();
            user.UpdatedAt = DateTime.UtcNow;

            _repository.Token.Remove(tokenEntity);
            _repository.User.Update(user);

            await _repository.SaveChangesAsync();
            return new SuccessResponse<object>
            {
                Message = "Password set successfully"
            };
        }

        #region private methods
        private async Task<AuthDTO> CreateToken(User user, bool populateExp)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            var refreshToken = GenerateRefreshToken();
            if (populateExp)
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return new AuthDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = user.RefreshTokenExpiryTime
            };
        }

        private SigningCredentials GetSigningCredentials()
        {
            var jwtSecret = _configuration.GetSection("JwtSettings")["secret"];
            var key = Encoding.UTF8.GetBytes(jwtSecret);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user!.Email),
            new Claim("Email", user.Email),
            new Claim("UserId", user.Id.ToString()),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim("PhoneNumber", user.PhoneNumber?? ""),
        };

            var roles = await _userManager.GetRolesAsync(user);
            var userRoles = new List<string>();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                userRoles.Add(role);
            }
            claims.Add(new Claim("RolesStr", string.Join(",", userRoles)));

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddDays(Convert.ToDouble(jwtSettings["ExpiresIn"])),
                signingCredentials: signingCredentials);
            return tokenOptions;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"])),
                ValidateLifetime = false,
                ValidAudience = jwtSettings["ValidAudience"],
                ValidIssuer = jwtSettings["ValidIssuer"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                   StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
        #endregion
    }
}
