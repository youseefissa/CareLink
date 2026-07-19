using CareLink.Application.Common;
using CareLink.Application.DTOs.Auth;
using CareLink.Application.Interfaces;
using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IEmailSender _emailSender;

        private const int RefreshTokenExpiryDays = 30;
        private const int PasswordResetTokenExpiryMinutes = 60;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _emailSender = emailSender;
        }

        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            var emailExists = await _unitOfWork.Users.EmailExistsAsync(request.Email);
            if (emailExists)
                return Result<AuthResponseDto>.Failure("Email already registered.");

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = _passwordHasher.Hash(request.Password),
                PhoneNumber = request.PhoneNumber,
                Role = request.Role
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return await IssueTokensAsync(user);
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
                return Result<AuthResponseDto>.Failure("Invalid email or password.");

            if (!user.IsActive)
                return Result<AuthResponseDto>.Failure("This account is deactivated.");

            return await IssueTokensAsync(user);
        }

        public async Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var storedToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);

            if (storedToken is null || !storedToken.IsActive)
                return Result<AuthResponseDto>.Failure("Invalid or expired refresh token.");

            var user = await _unitOfWork.Users.GetByIdAsync(storedToken.UserId);
            if (user is null || !user.IsActive)
                return Result<AuthResponseDto>.Failure("User not found or deactivated.");

            storedToken.RevokedAt = DateTime.UtcNow;
            _unitOfWork.RefreshTokens.Update(storedToken);

            return await IssueTokensAsync(user);
        }

        public async Task<Result> LogoutAsync(RefreshTokenRequestDto request)
        {
            var storedToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);

            if (storedToken is null)
                return Result.Failure("Refresh token not found.");

            storedToken.RevokedAt = DateTime.UtcNow;
            _unitOfWork.RefreshTokens.Update(storedToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

            if (user is not null)
            {
                var tokenValue = _jwtTokenGenerator.GenerateRefreshToken();

                var resetToken = new Domain.Entities.PasswordResetToken
                {
                    UserId = user.Id,
                    Token = tokenValue,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(PasswordResetTokenExpiryMinutes)
                };

                await _unitOfWork.PasswordResetTokens.AddAsync(resetToken);
                await _unitOfWork.SaveChangesAsync();

                var emailBody = $"Use this code to reset your CareLink AI password: {tokenValue}. This code expires in {PasswordResetTokenExpiryMinutes} minutes.";
                await _emailSender.SendAsync(user.Email, "CareLink AI - Password Reset", emailBody);
            }

            // Always return success, whether or not the email exists,
            // to avoid revealing which emails are registered in the system.
            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var resetToken = await _unitOfWork.PasswordResetTokens.GetByTokenAsync(request.Token);

            if (resetToken is null || !resetToken.IsActive)
                return Result.Failure("Invalid or expired reset token.");

            var user = await _unitOfWork.Users.GetByIdAsync(resetToken.UserId);
            if (user is null)
                return Result.Failure("User not found.");

            user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
            _unitOfWork.Users.Update(user);

            resetToken.IsUsed = true;
            resetToken.UsedAt = DateTime.UtcNow;
            _unitOfWork.PasswordResetTokens.Update(resetToken);

            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        private async Task<Result<AuthResponseDto>> IssueTokensAsync(User user)
        {
            var (accessToken, expiresAt) = _jwtTokenGenerator.GenerateToken(user);
            var refreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays)
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = accessToken,
                ExpiresAt = expiresAt,
                RefreshToken = refreshTokenValue,
                User = MapToDto(user)
            });
        }

        private static UserDto MapToDto(User user) => new()
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role
        };
    }
}