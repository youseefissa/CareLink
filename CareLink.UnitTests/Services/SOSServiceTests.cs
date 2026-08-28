using CareLink.Application.DTOs.SOS;
using CareLink.Application.Interfaces;
using CareLink.Application.Services;
using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace CareLink.UnitTests.Services
{
    public class SOSServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPatientProfileRepository> _patientRepositoryMock;
        private readonly Mock<ISOSEventRepository> _sosRepositoryMock;
        private readonly Mock<IAlertRepository> _alertRepositoryMock;
        private readonly Mock<ICaregiverProfileRepository> _caregiverRepositoryMock;
        private readonly Mock<ICaregiverPatientLinkRepository> _linkRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserMock;
        private readonly Mock<IAlertNotifier> _alertNotifierMock;
        private readonly SOSService _sut;

        private readonly Guid _patientProfileId = Guid.NewGuid();
        private readonly Guid _patientUserId = Guid.NewGuid();

        public SOSServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _patientRepositoryMock = new Mock<IPatientProfileRepository>();
            _sosRepositoryMock = new Mock<ISOSEventRepository>();
            _alertRepositoryMock = new Mock<IAlertRepository>();
            _caregiverRepositoryMock = new Mock<ICaregiverProfileRepository>();
            _linkRepositoryMock = new Mock<ICaregiverPatientLinkRepository>();
            _currentUserMock = new Mock<ICurrentUserService>();
            _alertNotifierMock = new Mock<IAlertNotifier>();

            _unitOfWorkMock.Setup(u => u.PatientProfiles).Returns(_patientRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.SOSEvents).Returns(_sosRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.Alerts).Returns(_alertRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.CaregiverProfiles).Returns(_caregiverRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.CaregiverPatientLinks).Returns(_linkRepositoryMock.Object);

            _sut = new SOSService(_unitOfWorkMock.Object, _currentUserMock.Object, _alertNotifierMock.Object);
        }

        [Fact]
        public async Task TriggerAsync_WithValidPatient_CreatesSosEventAndCriticalAlert()
        {
            // Arrange
            var patient = new PatientProfile { Id = _patientProfileId, UserId = _patientUserId };
            _patientRepositoryMock.Setup(r => r.GetByIdAsync(_patientProfileId)).ReturnsAsync(patient);

            _currentUserMock.Setup(c => c.Role).Returns("Patient");
            _currentUserMock.Setup(c => c.UserId).Returns(_patientUserId);

            var request = new CreateSOSEventDto
            {
                PatientProfileId = _patientProfileId,
                TriggerSource = "Button"
            };

            // Act
            var result = await _sut.TriggerAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            _sosRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SOSEvent>()), Times.Once);
            _alertRepositoryMock.Verify(r => r.AddAsync(It.Is<Alert>(a =>
                a.Type == Domain.Enums.AlertType.SOS &&
                a.Severity == Domain.Enums.AlertSeverity.Critical)), Times.Once);
            _alertNotifierMock.Verify(n => n.NotifyNewAlertAsync(It.IsAny<Application.DTOs.Alert.AlertBroadcastDto>()), Times.Once);
        }

        [Fact]
        public async Task TriggerAsync_WhenPatientTriesToTriggerForSomeoneElse_ReturnsFailure()
        {
            // Arrange
            var patient = new PatientProfile { Id = _patientProfileId, UserId = _patientUserId };
            _patientRepositoryMock.Setup(r => r.GetByIdAsync(_patientProfileId)).ReturnsAsync(patient);

            _currentUserMock.Setup(c => c.Role).Returns("Patient");
            _currentUserMock.Setup(c => c.UserId).Returns(Guid.NewGuid()); // مريض مختلف تمامًا

            var request = new CreateSOSEventDto
            {
                PatientProfileId = _patientProfileId,
                TriggerSource = "Button"
            };

            // Act
            var result = await _sut.TriggerAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            _sosRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SOSEvent>()), Times.Never);
        }

        [Fact]
        public async Task TriggerAsync_WithNonExistentPatient_ReturnsFailure()
        {
            // Arrange
            _patientRepositoryMock.Setup(r => r.GetByIdAsync(_patientProfileId)).ReturnsAsync((PatientProfile?)null);

            var request = new CreateSOSEventDto
            {
                PatientProfileId = _patientProfileId,
                TriggerSource = "Button"
            };

            // Act
            var result = await _sut.TriggerAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Patient profile not found.", result.Error);
        }

        [Fact]
        public async Task ResolveAsync_WhenCalledByPatient_ReturnsFailure()
        {
            // Arrange
            var sosEvent = new SOSEvent { Id = Guid.NewGuid(), PatientProfileId = _patientProfileId };
            _sosRepositoryMock.Setup(r => r.GetByIdAsync(sosEvent.Id)).ReturnsAsync(sosEvent);

            _currentUserMock.Setup(c => c.Role).Returns("Patient");

            // Act
            var result = await _sut.ResolveAsync(sosEvent.Id);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Only a caregiver or admin can resolve an SOS event.", result.Error);
        }

        [Fact]
        public async Task ResolveAsync_WhenCalledByCaregiver_MarksEventAsResolved()
        {
            // Arrange
            var sosEvent = new SOSEvent { Id = Guid.NewGuid(), PatientProfileId = _patientProfileId, Resolved = false };
            _sosRepositoryMock.Setup(r => r.GetByIdAsync(sosEvent.Id)).ReturnsAsync(sosEvent);

            _currentUserMock.Setup(c => c.Role).Returns("Caregiver");

            // Act
            var result = await _sut.ResolveAsync(sosEvent.Id);

            // Assert
            Assert.True(result.Succeeded);
            Assert.True(sosEvent.Resolved);
            Assert.NotNull(sosEvent.ResolvedAt);
        }
    }
}