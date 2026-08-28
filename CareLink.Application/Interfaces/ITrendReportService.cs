using CareLink.Application.Common;
using CareLink.Application.DTOs.TrendReport;

namespace CareLink.Application.Interfaces
{
    public interface ITrendReportService
    {
        Task<Result<TrendReportDto>> GenerateAsync(GenerateTrendReportDto request);
        Task<Result<IReadOnlyList<TrendReportDto>>> GetHistoryAsync(Guid patientProfileId);
        Task<Result<(byte[] Bytes, string FileName)>> DownloadPdfAsync(Guid reportId);
    }
}