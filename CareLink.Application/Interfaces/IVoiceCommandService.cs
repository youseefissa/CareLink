using CareLink.Application.Common;
using CareLink.Application.DTOs.Voice;

namespace CareLink.Application.Interfaces
{
    public interface IVoiceCommandService
    {
        Task<Result> ProcessCommandAsync(VoiceCommandDto request);
    }
}