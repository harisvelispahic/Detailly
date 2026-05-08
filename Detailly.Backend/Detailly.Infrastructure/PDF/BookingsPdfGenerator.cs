using Detailly.Application.Abstractions.PDF;
using Detailly.Application.Modules.Booking.Bookings.Queries.ExportMyBookings;
using Detailly.Domain.Common.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Detailly.Infrastructure.PDF;

public sealed class BookingsPdfGenerator : IBookingsPdfGenerator
{
    private const string PrimaryColor = "#9952E0";
    private const string PrimaryLight = "#B87EEA";
    private const string SurfaceColor = "#1C1A2E";
    private const string TextColor = "#F8F8F8";
    private const string MutedColor = "#9994AF";
    private const string SuccessColor = "#22C55E";
    private const string WarningColor = "#F59E0B";
    private const string InfoColor = "#0EA5E9";
    private const string DangerColor = "#EF4444";
    private const string NeutralColor = "#6B7280";
    private const string RowAlt = "#16141F";
    private const string RowBase = "#1C1A2E";
    private const string BorderColor = "#2D2A3E";

    public byte[] Generate(
        List<ExportMyBookingsQueryDto> bookings,
        DateTime startDate,
        DateTime endDate,
        string customerName)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(9).FontColor(TextColor));
                page.Background().Background(SurfaceColor);

                page.Content().Column(col =>
                {
                    col.Item().Background(PrimaryColor).Padding(20).Row(row =>
                    {
                        row.RelativeItem().Column(inner =>
                        {
                            inner.Item().Text("DETAILLY")
                                .FontSize(22).Bold().FontColor("#FFFFFF");
                            inner.Item().Text("My Appointments Report")
                                .FontSize(13).FontColor("#E8D5FF");
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
                            if (!string.IsNullOrWhiteSpace(customerName))
                            {
                                inner.Item().AlignRight().PaddingTop(4).Text(t =>
                                {
                                    t.Span("Client: ").FontColor("#E8D5FF").FontSize(8);
                                    t.Span(customerName).Bold().FontColor("#FFFFFF").FontSize(8);
                                });
                            }
                        });
                    });

                    col.Item().PaddingTop(12).Row(row =>
                    {
                        var completed = bookings.Count(b => b.Status == BookingStatus.Completed);
                        var confirmed = bookings.Count(b => b.Status == BookingStatus.Confirmed);
                        var totalSpent = bookings
                            .Where(b => b.Status is BookingStatus.Completed or BookingStatus.Confirmed)
                            .Sum(b => b.TotalPrice);

                        AddStatCard(row, "Total Bookings", bookings.Count.ToString(), PrimaryLight);
                        row.ConstantItem(8);
                        AddStatCard(row, "Completed", completed.ToString(), SuccessColor);
                        row.ConstantItem(8);
                        AddStatCard(row, "Upcoming", confirmed.ToString(), InfoColor);
                        row.ConstantItem(8);
                        AddStatCard(row, "Total Spent", $"BAM {totalSpent:N0}", WarningColor);
                    });

                    col.Item().PaddingTop(16);

                    if (bookings.Count == 0)
                    {
                        col.Item().Background(RowBase).Border(1).BorderColor(BorderColor).Padding(24)
                            .AlignCenter().Text("No appointments found for this period.").FontColor(MutedColor);
                        return;
                    }

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(40);
                            cols.RelativeColumn(3);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(2);
                            cols.ConstantColumn(45);
                            cols.ConstantColumn(70);
                        });

                        table.Header(header =>
                        {
                            foreach (var label in new[] { "ID", "Service Package", "Status", "Date", "Time", "Price (BAM)" })
                            {
                                header.Cell().Background(PrimaryColor).Padding(8).Text(label)
                                    .Bold().FontSize(8).FontColor("#FFFFFF");
                            }
                        });

                        for (int i = 0; i < bookings.Count; i++)
                        {
                            var b = bookings[i];
                            var bg = i % 2 == 0 ? RowBase : RowAlt;
                            var local = b.StartUtc.ToLocalTime();

                            table.Cell().Background(bg).BorderBottom(1).BorderColor(BorderColor)
                                .Padding(7).Text($"#{b.Id}").FontSize(8).FontColor(MutedColor);

                            table.Cell().Background(bg).BorderBottom(1).BorderColor(BorderColor)
                                .Padding(7).Text(b.ServicePackageName).FontSize(8).Bold();

                            table.Cell().Background(bg).BorderBottom(1).BorderColor(BorderColor)
                                .Padding(7).Text(t =>
                                {
                                    var (color, label) = GetStatusStyle(b.Status);
                                    t.Span(label).FontColor(color).Bold().FontSize(8);
                                });

                            table.Cell().Background(bg).BorderBottom(1).BorderColor(BorderColor)
                                .Padding(7).Text(local.ToString("dd MMM yyyy")).FontSize(8);

                            table.Cell().Background(bg).BorderBottom(1).BorderColor(BorderColor)
                                .Padding(7).Text(local.ToString("HH:mm")).FontSize(8);

                            table.Cell().Background(bg).BorderBottom(1).BorderColor(BorderColor)
                                .Padding(7).AlignRight().Text($"{b.TotalPrice:N2}").FontSize(8);
                        }
                    });

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
            col.Item().PaddingTop(4).Text(value).FontSize(16).Bold().FontColor(valueColor);
        });
    }

    private static (string color, string label) GetStatusStyle(BookingStatus status) => status switch
    {
        BookingStatus.Completed => (SuccessColor, "Completed"),
        BookingStatus.Confirmed => (InfoColor, "Confirmed"),
        BookingStatus.PendingPayment => (WarningColor, "Pending Payment"),
        BookingStatus.Cancelled => (DangerColor, "Cancelled"),
        BookingStatus.Expired => (NeutralColor, "Expired"),
        _ => (MutedColor, "Unknown"),
    };
}
