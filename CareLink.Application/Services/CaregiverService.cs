using CareLink.Application.Common;
using CareLink.Application.DTOs.Caregiver;
using CareLink.Application.Interfaces;
using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Application.Services
{
    public class CaregiverService : ICaregiverService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public CaregiverService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<CaregiverProfileDto>> CreateProfileAsync(CreateCaregiverProfileDto request)
        {
            if (_currentUser.Role == "Caregiver" && _currentUser.UserId != request.UserId)
                return Result<CaregiverProfileDto>.Failure("You can only create a profile for your own account.");

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user is null)
                return Result<CaregiverProfileDto>.Failure("User not found.");

            var existingProfile = await _unitOfWork.CaregiverProfiles.GetByUserIdAsync(request.UserId);
            if (existingProfile is not null)
                return Result<CaregiverProfileDto>.Failure("Caregiver profile already exists for this user.");

            var profile = new CaregiverProfile
            {
                UserId = request.UserId,
                RelationshipType = request.RelationshipType
            };


            await _unitOfWork.CaregiverProfiles.AddAsync(profile);
            await _unitOfWork.SaveChangesAsync();

            return Result<CaregiverProfileDto>.Success(MapToDto(profile, user.FullName));
        }

        public async Task<Result<CaregiverProfileDto>> GetByIdAsync(Guid caregiverProfileId)
        {
            var profile = await _unitOfWork.CaregiverProfiles.GetByIdAsync(caregiverProfileId);
            if (profile is null)
                return Result<CaregiverProfileDto>.Failure("Caregiver profile not found.");

            var isOwner = _currentUser.UserId == profile.UserId;
            var isAdmin = _currentUser.Role == "Admin";

            if (!isOwner && !isAdmin)
                return Result<CaregiverProfileDto>.Failure("You do not have permission to view this profile.");

            var user = await _unitOfWork.Users.GetByIdAsync(profile.UserId);

            return Result<CaregiverProfileDto>.Success(MapToDto(profile, user?.FullName ?? string.Empty));
        }
        public async Task<Result<CaregiverProfileDto>> GetMyProfileAsync()
        {
            if (!_currentUser.UserId.HasValue)
                return Result<CaregiverProfileDto>.Failure("User is not authenticated.");

            var profile = await _unitOfWork.CaregiverProfiles.GetByUserIdAsync(_currentUser.UserId.Value);
            if (profile is null)
                return Result<CaregiverProfileDto>.Failure("Caregiver profile not found for this user.");

            var user = await _unitOfWork.Users.GetByIdAsync(profile.UserId);

            return Result<CaregiverProfileDto>.Success(MapToDto(profile, user?.FullName ?? string.Empty));
        }
        private static CaregiverProfileDto MapToDto(CaregiverProfile profile, string fullName) => new()
        {
            Id = profile.Id,
            UserId = profile.UserId,
            FullName = fullName,
            RelationshipType = profile.RelationshipType
        };
    }
}