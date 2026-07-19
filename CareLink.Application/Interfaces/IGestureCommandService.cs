using CareLink.Application.Common;
using CareLink.Application.DTOs.Gesture;

namespace CareLink.Application.Interfaces
{
    public interface IGestureCommandService
    {
        Task<Result> ProcessGestureAsync(GestureCommandDto request);
    }
}