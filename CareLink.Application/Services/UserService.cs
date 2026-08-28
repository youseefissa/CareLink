using CareLink.Application.Common;
using CareLink.Application.DTOs.User;
using CareLink.Application.Interfaces;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public UserService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> RegisterDeviceTokenAsync(RegisterDeviceTokenDto request)
        {
            if (!_currentUser.UserId.HasValue)
                return Result.Failure("User is not authenticated.");

            var user = await _unitOfWork.Users.GetByIdAsync(_currentUser.UserId.Value);
            if (user is null)
                return Result.Failure("User not found.");

            user.FcmDeviceToken = request.DeviceToken;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}