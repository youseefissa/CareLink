using CareLink.Application.Common;
using CareLink.Application.DTOs.Auth;

namespace CareLink.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
        Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request);
        Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<Result> LogoutAsync(RefreshTokenRequestDto request);
        Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<Result> ResetPasswordAsync(ResetPasswordRequestDto request);
    }
}