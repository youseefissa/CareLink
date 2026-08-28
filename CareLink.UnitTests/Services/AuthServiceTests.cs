using CareLink.Application.DTOs.Auth;
using CareLink.Application.Interfaces;
using CareLink.Application.Services;
using CareLink.Domain.Entities;
using CareLink.Domain.Enums;
using CareLink.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace CareLink.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<IPasswordResetTokenRepository> _passwordResetTokenRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _passwordResetTokenRepositoryMock = new Mock<IPasswordResetTokenRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _emailSenderMock = new Mock<IEmailSender>();

            _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.RefreshTokens).Returns(_refreshTokenRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.PasswordResetTokens).Returns(_passwordResetTokenRepositoryMock.Object);

            _sut = new AuthService(
                _unitOfWorkMock.Object,
                _passwordHasherMock.Object,
                _jwtTokenGeneratorMock.Object,
                _emailSenderMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_WithNewEmail_ReturnsSuccessWithToken()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                FullName = "Test User",
                Email = "newuser@test.com",
                Password = "Test@12345",
                PhoneNumber = "01000000000",
                Role = UserRole.Patient
            };

            _userRepositoryMock.Setup(r => r.EmailExistsAsync(request.Email)).ReturnsAsync(false);
            _passwordHasherMock.Setup(p => p.Hash(request.Password)).Returns("hashed_password");
            _jwtTokenGeneratorMock
                .Setup(j => j.GenerateToken(It.IsAny<User>()))
                .Returns(("fake_access_token", DateTime.UtcNow.AddMinutes(15)));
            _jwtTokenGeneratorMock.Setup(j => j.GenerateRefreshToken()).Returns("fake_refresh_token");

            // Act
            var result = await _sut.RegisterAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("fake_access_token", result.Data!.Token);
            Assert.Equal("fake_refresh_token", result.Data!.RefreshToken);
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ReturnsFailure()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                FullName = "Test User",
                Email = "existing@test.com",
                Password = "Test@12345",
                PhoneNumber = "01000000000",
                Role = UserRole.Patient
            };

            _userRepositoryMock.Setup(r => r.EmailExistsAsync(request.Email)).ReturnsAsync(true);

            // Act
            var result = await _sut.RegisterAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("already registered", result.Error);
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@test.com",
                PasswordHash = "hashed_password",
                IsActive = true,
                FullName = "Test User",
                Role = UserRole.Patient
            };

            var request = new LoginRequestDto { Email = "user@test.com", Password = "Test@12345" };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(existingUser);
            _passwordHasherMock.Setup(p => p.Verify(request.Password, existingUser.PasswordHash)).Returns(true);
            _jwtTokenGeneratorMock
                .Setup(j => j.GenerateToken(existingUser))
                .Returns(("fake_access_token", DateTime.UtcNow.AddMinutes(15)));
            _jwtTokenGeneratorMock.Setup(j => j.GenerateRefreshToken()).Returns("fake_refresh_token");

            // Act
            var result = await _sut.LoginAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(existingUser.Email, result.Data!.User.Email);
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ReturnsFailure()
        {
            // Arrange
            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@test.com",
                PasswordHash = "hashed_password",
                IsActive = true
            };

            var request = new LoginRequestDto { Email = "user@test.com", Password = "WrongPassword" };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(existingUser);
            _passwordHasherMock.Setup(p => p.Verify(request.Password, existingUser.PasswordHash)).Returns(false);

            // Act
            var result = await _sut.LoginAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Invalid email or password.", result.Error);
        }

        [Fact]
        public async Task LoginAsync_WithDeactivatedAccount_ReturnsFailure()
        {
            // Arrange
            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@test.com",
                PasswordHash = "hashed_password",
                IsActive = false
            };

            var request = new LoginRequestDto { Email = "user@test.com", Password = "Test@12345" };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(existingUser);
            _passwordHasherMock.Setup(p => p.Verify(request.Password, existingUser.PasswordHash)).Returns(true);

            // Act
            var result = await _sut.LoginAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("This account is deactivated.", result.Error);
        }

        [Fact]
        public async Task LoginAsync_WithNonExistentEmail_ReturnsFailure()
        {
            // Arrange
            var request = new LoginRequestDto { Email = "doesnotexist@test.com", Password = "Test@12345" };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((User?)null);

            // Act
            var result = await _sut.LoginAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Invalid email or password.", result.Error);
        }
    }
}