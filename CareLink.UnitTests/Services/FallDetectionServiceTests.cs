using CareLink.Application.DTOs.Fall;
using CareLink.Application.Interfaces;
using CareLink.Application.Services;
using CareLink.Domain.Entities;
using CareLink.Domain.Enums;
using CareLink.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace CareLink.UnitTests.Services
{
    public class FallDetectionServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPatientProfileRepository> _patientRepositoryMock;
        private readonly Mock<IFallEventRepository> _fallRepositoryMock;
        private readonly Mock<IAlertRepository> _alertRepositoryMock;
        private readonly Mock<ICaregiverProfileRepository> _caregiverRepositoryMock;
        private readonly Mock<ICaregiverPatientLinkRepository> _linkRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserMock;
        private readonly Mock<IAlertNotifier> _alertNotifierMock;
        private readonly FallDetectionService _sut;

        private readonly Guid _patientProfileId = Guid.NewGuid();
        private readonly Guid _patientUserId = Guid.NewGuid();
        private readonly Mock<IAiServiceClient> _aiServiceClientMock;
        public FallDetectionServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _patientRepositoryMock = new Mock<IPatientProfileRepository>();
            _fallRepositoryMock = new Mock<IFallEventRepository>();
            _alertRepositoryMock = new Mock<IAlertRepository>();
            _caregiverRepositoryMock = new Mock<ICaregiverProfileRepository>();
            _linkRepositoryMock = new Mock<ICaregiverPatientLinkRepository>();
            _currentUserMock = new Mock<ICurrentUserService>();
            _alertNotifierMock = new Mock<IAlertNotifier>();
            _aiServiceClientMock = new Mock<IAiServiceClient>();

            _unitOfWorkMock.Setup(u => u.PatientProfiles).Returns(_patientRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.FallEvents).Returns(_fallRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.Alerts).Returns(_alertRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.CaregiverProfiles).Returns(_caregiverRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.CaregiverPatientLinks).Returns(_linkRepositoryMock.Object);


            var patient = new PatientProfile { Id = _patientProfileId, UserId = _patientUserId };
            _patientRepositoryMock.Setup(r => r.GetByIdAsync(_patientProfileId)).ReturnsAsync(patient);

            _currentUserMock.Setup(c => c.Role).Returns("Patient");
            _currentUserMock.Setup(c => c.UserId).Returns(_patientUserId);
            _sut = new FallDetectionService(_unitOfWorkMock.Object, _currentUserMock.Object, _alertNotifierMock.Object, _aiServiceClientMock.Object);
        }

        [Fact]
        public async Task RecordFallEventAsync_WhenIsFallTrue_CreatesHighSeverityAlert()
        {
            // Arrange
            _fallRepositoryMock
                .Setup(r => r.CountFallsInPeriodAsync(_patientProfileId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(1); // مرة واحدة بس، مش تكرار

            var request = new CreateFallEventDto
            {
                PatientProfileId = _patientProfileId,
                IsFall = true,
                Confidence = 0.9
            };

            // Act
            var result = await _sut.RecordFallEventAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            _fallRepositoryMock.Verify(r => r.AddAsync(It.IsAny<FallEvent>()), Times.Once);
            _alertRepositoryMock.Verify(r => r.AddAsync(It.Is<Alert>(a =>
                a.Type == AlertType.Fall &&
                a.Severity == AlertSeverity.High)), Times.Once);
        }

        [Fact]
        public async Task RecordFallEventAsync_WhenIsFallFalse_DoesNotCreateAlert()
        {
            // Arrange
            var request = new CreateFallEventDto
            {
                PatientProfileId = _patientProfileId,
                IsFall = false,
                Confidence = 0.3
            };

            // Act
            var result = await _sut.RecordFallEventAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            _fallRepositoryMock.Verify(r => r.AddAsync(It.IsAny<FallEvent>()), Times.Once);
            _alertRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Alert>()), Times.Never);
        }

        [Fact]
        public async Task RecordFallEventAsync_WhenThirdFallInAWeek_CreatesAdditionalCriticalAlert()
        {
            // Arrange
            _fallRepositoryMock
                .Setup(r => r.CountFallsInPeriodAsync(_patientProfileId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(3); // السقوط ده هو التالت في الأسبوع

            var request = new CreateFallEventDto
            {
                PatientProfileId = _patientProfileId,
                IsFall = true,
                Confidence = 0.85
            };

            // Act
            var result = await _sut.RecordFallEventAsync(request);

            // Assert
            Assert.True(result.Succeeded);

            // اتنين تنبيهات، واحد عادي للسقوط، وواحد إضافي للتكرار
            _alertRepositoryMock.Verify(r => r.AddAsync(It.Is<Alert>(a => a.Type == AlertType.Fall)), Times.Once);
            _alertRepositoryMock.Verify(r => r.AddAsync(It.Is<Alert>(a => a.Type == AlertType.RepeatedFalls)), Times.Once);
        }

        [Fact]
        public async Task RecordFallEventAsync_WithNonExistentPatient_ReturnsFailure()
        {
            // Arrange
            var unknownPatientId = Guid.NewGuid();
            _patientRepositoryMock.Setup(r => r.GetByIdAsync(unknownPatientId)).ReturnsAsync((PatientProfile?)null);

            var request = new CreateFallEventDto
            {
                PatientProfileId = unknownPatientId,
                IsFall = true,
                Confidence = 0.9
            };

            // Act
            var result = await _sut.RecordFallEventAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Patient profile not found.", result.Error);
        }
    }
}