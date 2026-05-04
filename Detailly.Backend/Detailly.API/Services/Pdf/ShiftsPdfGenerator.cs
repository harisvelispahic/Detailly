using Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ExportShifts;
using Detailly.Domain.Common.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Detailly.API.Services.Pdf;

public static class ShiftsPdfGenerator
{
    private const string PrimaryColor = "#9952E0";
    private const string PrimaryLight = "#B87EEA";
    private const string SurfaceColor = "#1C1A2E";
    private const string TextColor = "#F8F8F8";
    private const string MutedColor = "#9994AF";
    private const string ShopColor = "#0EA5E9";
    private const string MobileColor = "#F59E0B";
    private const string RowAlt = "#16141F";
    private const string RowBase = "#1C1A2E";
    private const string BorderColor = "#2D2A3E";

    public static byte[] Generate(
        List<ExportShiftsQueryDto> shifts,
        DateTime startDate,
        DateTime endDate,
        string locationName)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, QuestPDF.Infrastructure.Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(9).FontColor(TextColor));
                page.Background().Background(SurfaceColor);

                page.Content().Column(col =>
                {
                    // Header
                    col.Item().Background(PrimaryColor).Padding(20).Row(row =>
                    {
                        row.RelativeItem().Column(inner =>
                        {
                            inner.Item().Text("DETAILLY")
                                .FontSize(22).Bold().FontColor("#FFFFFF");
                            inner.Item().Text("Employee Shifts Report")
                                .FontSize(13).FontColor("#E8D5FF");
                            if (!string.IsNullOrWhiteSpace(locationName))
                            {
                                inner.Item().PaddingTop(4).Text(t =>
                                {
                                    t.Span("Location: ").FontColor("#E8D5FF").FontSize(8);
                                    t.Span(locationName).Bold().FontColor("#FFFFFF").FontSize(8);
                                });
                            }
                        });
                        row.ConstantItem(180).Column(inner =>
                        {
                            inner.Item().AlignRight().Text(t =>
                            {
                                t.Span("Period: ").FontColor("#E8D5FF").FontSize(8);
                                t.Span($"{startDate:dd/MM/yyyy} – {endDate:dd/MM/yyyy}").Bold().FontColor("#FFFFFF").FontSize(8);
                            });
                            inner.Item().AlignRight().PaddingTop(4).Text(t =>
                            {
                                t.Span("Generated: ").FontColor("#E8D5FF").FontSize(8);
                                t.Span($"{DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC").Bold().FontColor("#FFFFFF").FontSize(8);
                            });
                        });
                    });

                    // Stats bar
                    col.Item().PaddingTop(12).Row(row =>
                    {
                        var shopCount = shifts.Count(s => s.EmployeeWorkMode == EmployeeWorkMode.InShop);
                        var mobileCount = shifts.Count(s => s.EmployeeWorkMode == EmployeeWorkMode.Mobile);
                        var totalHours = shifts.Sum(s => (s.EndUtc - s.StartUtc).TotalHours);
                        var uniqueEmployees = shifts.Select(s => s.EmployeeName).Distinct().Count();

                        AddStatCard(row, "Total Shifts", shifts.Count.ToString(), PrimaryLight);
                        row.ConstantItem(8);
                        AddStatCard(row, "In Shop", shopCount.ToString(), ShopColor);
                        row.ConstantItem(8);
                        AddStatCard(row, "Mobile", mobileCount.ToString(), MobileColor);
                        row.ConstantItem(8);
                        AddStatCard(row, "Employees", uniqueEmployees.ToString(), "#22C55E");
                        row.ConstantItem(8);
                        AddStatCard(row, "Total Hours", $"{totalHours:N1}h", "#A855F7");
                    });

                    col.Item().PaddingTop(16);

                    if (shifts.Count == 0)
                    {
                        col.Item().Background(RowBase).Border(1).BorderColor(BorderColor).Padding(24)
                            .AlignCenter().Text("No shifts found for this period.").FontColor(MutedColor);
                        return;
                    }

                    // Table
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(40);   // ID
                            cols.RelativeColumn(3);    // Employee
                            cols.RelativeColumn(2);    // Mode
                            cols.RelativeColumn(2);    // Start
                            cols.RelativeColumn(2);    // End
                            cols.ConstantColumn(55);   // Duration
                        });

                        // Header row
                        table.Header(header =>
                        {
                            foreach (var label in new[] { "ID", "Employee", "Work Mode", "Start", "End", "Duration" })
                            {
                                header.Cell().Background(PrimaryColor).Padding(8).Text(label)
                                    .Bold().FontSize(8).FontColor("#FFFFFF");
                            }
                        });

                        // Data rows
                        for (int i = 0; i < shifts.Count; i++)
                        {
                            var s = shifts[i];
                            var bg = i % 2 == 0 ? RowBase : RowAlt;
                            var startLocal = s.StartUtc.ToLocalTime();
                            var endLocal = s.EndUtc.ToLocalTime();
                            var duration = s.EndUtc - s.StartUtc;
                            var durationStr = duration.Minutes > 0
                                ? $"{(int)duration.TotalHours}h {duration.Minutes}m"
                                : $"{(int)duration.TotalHours}h";

                            table.Cell().Background(bg).BorderBottom(1).BorderColor(BorderColor)
                                .Padding(7).Text($"#{s.Id}").FontSize(8).FontColor(MutedColor);

                            table.Cell().Background(bg).BorderBottom(1).BorderColor(BorderColor)
                                .Padding(7).Text(s.EmployeeName).FontSize(8).Bold();

                            table.Cell().Background(bg).BorderBottom(1).BorderColor(BorderColor)
                                .Padding(7).Text(t =>
                                {
                                    var (color, label) = s.EmployeeWorkMode == EmployeeWorkMode.InShop
                                        ? (ShopColor, "In Shop")
                                        : (MobileColor, "Mobile");
                                    t.Span(label).FontColor(color).Bold().FontSize(8);
                                });

                            table.Cell().Background(bg).BorderBottom(1).BorderColor(BorderColor)
                                .Padding(7).Text(startLocal.ToString("dd MMM  HH:mm")).FontSize(8);

                            table.Cell().Background(bg).BorderBottom(1).BorderColor(BorderColor)
                                .Padding(7).Text(endLocal.ToString("dd MMM  HH:mm")).FontSize(8);

                            table.Cell().Background(bg).BorderBottom(1).BorderColor(BorderColor)
                                .Padding(7).AlignCenter().Text(durationStr).FontSize(8);
                        }
                    });

                    // Footer note
                    col.Item().PaddingTop(12).AlignCenter().Text(t =>
                    {
                        t.Span("Generated by ").FontColor(MutedColor).FontSize(7);
                        t.Span("Detailly").Bold().FontColor(PrimaryLight).FontSize(7);
                        t.Span(" — Car Detailing Management System").FontColor(MutedColor).FontSize(7);
                    });
                });
            });
        }).GeneratePdf();
    }

    private static void AddStatCard(RowDescriptor row, string label, string value, string valueColor)
    {
        row.RelativeItem().Background(RowBase).Border(1).BorderColor(BorderColor).Padding(10).Column(col =>
        {
            col.Item().Text(label).FontSize(7).FontColor(MutedColor);
            col.Item().PaddingTop(4).Text(value).FontSize(14).Bold().FontColor(valueColor);
        });
    }
}
