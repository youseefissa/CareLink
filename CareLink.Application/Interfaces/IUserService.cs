using CareLink.Application.Common;
using CareLink.Application.DTOs.User;

namespace CareLink.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result> RegisterDeviceTokenAsync(RegisterDeviceTokenDto request);
    }
}