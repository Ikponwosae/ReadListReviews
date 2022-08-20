using Application.DataTransferObjects;
using Application.Helpers;

namespace Application.Contracts;

public interface IAuthenticationService
{
    Task<SuccessResponse<AuthDTO>> Login(UserLoginDTO model);
    Task<SuccessResponse<AuthDTO>> RefreshToken(RefreshTokenDTO model);
    Task<SuccessResponse<object>> ResetPassword(ResetPasswordDTO model);
    Task<TokenResponse<object>> VerifyToken(string token);
    //Task<SuccessResponse<object>> SetPassword(SetPasswordDTO model);
}