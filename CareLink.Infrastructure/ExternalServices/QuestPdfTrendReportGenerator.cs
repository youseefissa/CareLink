using CareLink.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CareLink.Infrastructure.ExternalServices
{
    public class QuestPdfTrendReportGenerator : IPdfReportGenerator
    {
        public QuestPdfTrendReportGenerator()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateTrendReportPdf(TrendReportPdfData data)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("CareLink AI").FontSize(22).Bold();
                        column.Item().Text("Patient Trend Report").FontSize(14).FontColor(Colors.Grey.Darken2);
                        column.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        column.Spacing(12);

                        column.Item().Text($"Patient: {data.PatientFullName}").Bold();
                        column.Item().Text($"Period: {data.PeriodStart:dd MMM yyyy} - {data.PeriodEnd:dd MMM yyyy}");
                        column.Item().Text($"Generated at: {data.GeneratedAt:dd MMM yyyy, HH:mm} UTC")
                            .FontSize(9).FontColor(Colors.Grey.Darken1);

                        column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        column.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            void AddRow(string label, string value)
                            {
                                table.Cell().Padding(6).Text(label);
                                table.Cell().Padding(6).AlignRight().Text(value).Bold();
                            }

                            AddRow("Total Falls", data.TotalFalls.ToString());
                            AddRow("Average Daily Activity", data.AverageDailyActivity.ToString("0.0"));
                            AddRow("Medication Confirmations", data.MedicationConfirmationsCount.ToString());
                            AddRow("Medication Missed", data.MedicationMissedCount.ToString());
                            AddRow("Inactivity Alerts", data.InactivityEventsCount.ToString());
                        });

                        column.Item().PaddingTop(20).Text(
                            "This report is intended for caregivers and medical professionals. " +
                            "It does not constitute a medical diagnosis.")
                            .FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}