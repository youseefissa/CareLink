using CareLink.Application.Interfaces;
using CareLink.Application.Services;
using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace CareLink.UnitTests.Services
{
    public class PatientServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPatientProfileRepository> _patientRepositoryMock;
        private readonly Mock<ICaregiverProfileRepository> _caregiverRepositoryMock;
        private readonly Mock<ICaregiverPatientLinkRepository> _linkRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserMock;
        private readonly PatientService _sut;

        private readonly Guid _patientProfileId = Guid.NewGuid();
        private readonly Guid _patientUserId = Guid.NewGuid();

        public PatientServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _patientRepositoryMock = new Mock<IPatientProfileRepository>();
            _caregiverRepositoryMock = new Mock<ICaregiverProfileRepository>();
            _linkRepositoryMock = new Mock<ICaregiverPatientLinkRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _currentUserMock = new Mock<ICurrentUserService>();

            _unitOfWorkMock.Setup(u => u.PatientProfiles).Returns(_patientRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.CaregiverProfiles).Returns(_caregiverRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.CaregiverPatientLinks).Returns(_linkRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepositoryMock.Object);

            _sut = new PatientService(_unitOfWorkMock.Object, _currentUserMock.Object);
        }

        private PatientProfile BuildPatientProfile()
        {
            return new PatientProfile
            {
                Id = _patientProfileId,
                UserId = _patientUserId,
                User = new User { FullName = "Test Patient" }
            };
        }

        [Fact]
        public async Task GetByIdAsync_WhenCurrentUserIsThePatientHimself_ReturnsSuccess()
        {
            // Arrange
            var profile = BuildPatientProfile();
            _patientRepositoryMock.Setup(r => r.GetWithDetailsAsync(_patientProfileId)).ReturnsAsync(profile);

            _currentUserMock.Setup(c => c.Role).Returns("Patient");
            _currentUserMock.Setup(c => c.UserId).Returns(_patientUserId);

            // Act
            var result = await _sut.GetByIdAsync(_patientProfileId);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCurrentUserIsAnUnrelatedPatient_ReturnsFailure()
        {
            // Arrange
            var profile = BuildPatientProfile();
            _patientRepositoryMock.Setup(r => r.GetWithDetailsAsync(_patientProfileId)).ReturnsAsync(profile);

            _currentUserMock.Setup(c => c.Role).Returns("Patient");
            _currentUserMock.Setup(c => c.UserId).Returns(Guid.NewGuid()); // مريض تاني تمامًا

            // Act
            var result = await _sut.GetByIdAsync(_patientProfileId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("You do not have permission to view this profile.", result.Error);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCurrentUserIsALinkedCaregiver_ReturnsSuccess()
        {
            // Arrange
            var profile = BuildPatientProfile();
            var caregiverUserId = Guid.NewGuid();
            var caregiverProfileId = Guid.NewGuid();

            _patientRepositoryMock.Setup(r => r.GetWithDetailsAsync(_patientProfileId)).ReturnsAsync(profile);

            _currentUserMock.Setup(c => c.Role).Returns("Caregiver");
            _currentUserMock.Setup(c => c.UserId).Returns(caregiverUserId);

            _caregiverRepositoryMock
                .Setup(r => r.GetByUserIdAsync(caregiverUserId))
                .ReturnsAsync(new CaregiverProfile { Id = caregiverProfileId, UserId = caregiverUserId });

            _linkRepositoryMock
                .Setup(r => r.LinkExistsAsync(caregiverProfileId, _patientProfileId))
                .ReturnsAsync(true);

            // Act
            var result = await _sut.GetByIdAsync(_patientProfileId);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCurrentUserIsAnUnlinkedCaregiver_ReturnsFailure()
        {
            // Arrange
            var profile = BuildPatientProfile();
            var caregiverUserId = Guid.NewGuid();
            var caregiverProfileId = Guid.NewGuid();

            _patientRepositoryMock.Setup(r => r.GetWithDetailsAsync(_patientProfileId)).ReturnsAsync(profile);

            _currentUserMock.Setup(c => c.Role).Returns("Caregiver");
            _currentUserMock.Setup(c => c.UserId).Returns(caregiverUserId);

            _caregiverRepositoryMock
                .Setup(r => r.GetByUserIdAsync(caregiverUserId))
                .ReturnsAsync(new CaregiverProfile { Id = caregiverProfileId, UserId = caregiverUserId });

            _linkRepositoryMock
                .Setup(r => r.LinkExistsAsync(caregiverProfileId, _patientProfileId))
                .ReturnsAsync(false); // مش مرتبط بيه

            // Act
            var result = await _sut.GetByIdAsync(_patientProfileId);

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCurrentUserIsAdmin_ReturnsSuccessRegardlessOfOwnership()
        {
            // Arrange
            var profile = BuildPatientProfile();
            _patientRepositoryMock.Setup(r => r.GetWithDetailsAsync(_patientProfileId)).ReturnsAsync(profile);

            _currentUserMock.Setup(c => c.Role).Returns("Admin");
            _currentUserMock.Setup(c => c.UserId).Returns(Guid.NewGuid());

            // Act
            var result = await _sut.GetByIdAsync(_patientProfileId);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task GetByIdAsync_WhenPatientProfileDoesNotExist_ReturnsFailure()
        {
            // Arrange
            _patientRepositoryMock
                .Setup(r => r.GetWithDetailsAsync(_patientProfileId))
                .ReturnsAsync((PatientProfile?)null);

            // Act
            var result = await _sut.GetByIdAsync(_patientProfileId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Patient profile not found.", result.Error);
        }
    }
}