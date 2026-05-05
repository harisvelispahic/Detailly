using Detailly.Application.Modules.Booking.Bookings.Commands.AssignEmployees;
using Detailly.Application.Modules.Booking.Bookings.Commands.Cancel;
using Detailly.Application.Modules.Booking.Bookings.Commands.Complete;
using Detailly.Application.Modules.Booking.Bookings.Commands.CreateHold;
using Detailly.Application.Modules.Booking.Bookings.Commands.ExportMyBookingsPdf;
using Detailly.Application.Modules.Booking.Bookings.Queries.GetAvailability;
using Detailly.Application.Modules.Booking.Bookings.Queries.GetById;
using Detailly.Application.Modules.Booking.Bookings.Queries.ListAssignableEmployees;
using Detailly.Application.Modules.Booking.Bookings.Queries.ListForDate;
using Detailly.Application.Modules.Booking.Bookings.Queries.ListMine;
using Detailly.Application.Modules.Booking.Bookings.Queries.ListMyAssigned;
using Detailly.Application.Modules.Booking.Bookings.Queries.ListUnassigned;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingsController(ISender sender) : ControllerBase
{
    // ---------------------------------------
    // CREATE BOOKING HOLD (PendingPayment)
    // ---------------------------------------
    [HttpPost]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<ActionResult<int>> Create(CreateBookingHoldCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    // ---------------------------------------
    // CANCEL BOOKING
    // ---------------------------------------
    [HttpPut("cancel/{id:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task Cancel(int id, CancelBookingCommand command, CancellationToken ct)
    {
        command.BookingId = id; // Route ID precedence
        await sender.Send(command, ct);
        // no return -> 204 No Content
    }

    // ---------------------------------------
    // GET BOOKING BY ID (ownership enforced)
    // ---------------------------------------
    [HttpGet("{id:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<GetBookingByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var booking = await sender.Send(new GetBookingByIdQuery { Id = id }, ct);
        return booking;
    }

    // ---------------------------------------
    // LIST MY BOOKINGS
    // ---------------------------------------
    [HttpGet("my")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<PageResult<ListMyBookingsQueryDto>> ListMine([FromQuery] ListMyBookingsQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }

    // ---------------------------------------
    // EXPORT MY BOOKINGS AS PDF
    // GET /Bookings/my/export-pdf?startDate=2026-01-01&endDate=2026-12-31
    // ---------------------------------------
    [HttpGet("my/export-pdf")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<IActionResult> ExportMyBookingsPdf(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken ct)
    {
        var customerName = User.FindFirst("name")?.Value
            ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
            ?? string.Empty;

        var pdfBytes = await sender.Send(new ExportMyBookingsPdfCommand
        {
            StartDateUtc = startDate,
            EndDateUtc = endDate,
            CustomerName = customerName,
        }, ct);

        return File(pdfBytes, "application/pdf", $"my-appointments-{startDate:yyyy-MM-dd}-to-{endDate:yyyy-MM-dd}.pdf");
    }

    // ---------------------------------------
    // GET AVAILABILITY
    // ---------------------------------------
    [HttpGet("availability")]
    [AllowAnonymous]
    public async Task<GetAvailabilityResult> GetAvailability(
        [FromQuery] GetAvailabilityQuery query,
        CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }

    // ---------------------------------------
    // COMPLETE BOOKING (Employee, Manager or Admin)
    // ---------------------------------------
    [HttpPut("complete/{id:int}")]
    [Authorize(Policy = AuthPolicies.Staff)]
    public async Task Complete(int id, CancellationToken ct)
    {
        await sender.Send(new CompleteBookingCommand { BookingId = id }, ct);
        // no return -> 204 No Content
    }

    // ==========================================================
    // STAFF SCHEDULING + ASSIGNMENT
    // ==========================================================

    // ---------------------------------------
    // STAFF: LIST BOOKINGS FOR DATE (dashboard)
    // ---------------------------------------
    // Example:
    // GET /Bookings/staff/schedule?dateUtc=2026-03-01&shopLocationId=1&serviceMode=InShop&includePendingPayment=true
    [HttpGet("staff/schedule")]
    [Authorize(Policy = AuthPolicies.Staff)]
    public async Task<PageResult<ListBookingsForDateQueryDto>> ListForDate([FromQuery] ListBookingsForDateQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }

    // ---------------------------------------
    // MANAGER: LIST ASSIGNABLE EMPLOYEES FOR BOOKING
    // ---------------------------------------
    [HttpGet("assignable-employees/{id:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<List<ListAssignableEmployeesForBookingQueryDto>> ListAssignableEmployees(int id, CancellationToken ct)
    {
        var result = await sender.Send(new ListAssignableEmployeesForBookingQuery(id), ct);
        return result;
    }

    // ---------------------------------------
    // MANAGER: ASSIGN EMPLOYEES TO BOOKING
    // ---------------------------------------
    [HttpPut("assign-employees/{id:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task AssignEmployees(int id, AssignEmployeesToBookingCommand command, CancellationToken ct)
    {
        command.BookingId = id; // Route ID precedence
        await sender.Send(command, ct);
        // no return -> 204 No Content
    }

    // ---------------------------------------
    // MANAGER: LIST UNASSIGNED CONFIRMED BOOKINGS
    // ---------------------------------------
    [HttpGet("staff/unassigned")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<PageResult<ListUnassignedBookingsQueryDto>> ListUnassigned(
        [FromQuery] ListUnassignedBookingsQuery query,
        CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }

    // ---------------------------------------
    // EMPLOYEE: LIST MY ASSIGNED BOOKINGS
    // ---------------------------------------
    [HttpGet("employee/my")]
    [Authorize(Policy = AuthPolicies.Staff)]
    public async Task<PageResult<ListMyAssignedBookingsQueryDto>> ListMyAssigned(
        [FromQuery] ListMyAssignedBookingsQuery query,
        CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }
}