namespace CareLink.Application.Interfaces
{
    public interface IFileStorage
    {
        Task<string> SaveAsync(string fileName, byte[] content);
        Task<byte[]?> ReadAsync(string path);
    }
}