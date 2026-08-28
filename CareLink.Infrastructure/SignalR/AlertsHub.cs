using CareLink.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CareLink.Infrastructure.SignalR
{
    [Authorize]
    public class AlertsHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;

        public AlertsHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task OnConnectedAsync()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (Guid.TryParse(userIdClaim, out var userId) && role == "Caregiver")
            {
                var caregiverProfile = await _unitOfWork.CaregiverProfiles.GetByUserIdAsync(userId);

                if (caregiverProfile is not null)
                {
                    var patients = await _unitOfWork.PatientProfiles.GetByCaregiverIdAsync(caregiverProfile.Id);

                    foreach (var patient in patients)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, patient.Id.ToString());
                    }
                }
            }

            await base.OnConnectedAsync();
        }
    }
}