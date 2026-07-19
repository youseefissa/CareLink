namespace CareLink.Application.Interfaces
{
    // Implemented in API layer, reads the JWT claims of the current HTTP request
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Email { get; }
        string? Role { get; }
    }
}