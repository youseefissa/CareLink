using CareLink.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CareLink.Infrastructure.ExternalServices
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly string _rootPath;

        public LocalFileStorage(IConfiguration configuration)
        {
            _rootPath = configuration["FileStorage:ReportsPath"] ?? "GeneratedReports";

            if (!Directory.Exists(_rootPath))
            {
                Directory.CreateDirectory(_rootPath);
            }
        }

        public async Task<string> SaveAsync(string fileName, byte[] content)
        {
            var fullPath = Path.Combine(_rootPath, fileName);
            await File.WriteAllBytesAsync(fullPath, content);
            return fullPath;
        }

        public async Task<byte[]?> ReadAsync(string path)
        {
            if (!File.Exists(path))
                return null;

            return await File.ReadAllBytesAsync(path);
        }
    }
}