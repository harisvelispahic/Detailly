using Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Create;
using Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Delete;
using Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Update;
using Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ExportShifts;
using Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ListForDate;
using Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ListMine;
using Detailly.API.Services.Pdf;
using Detailly.Domain.Common.Enums;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class EmployeeShiftsController(ISender sender) : ControllerBase
{
    // ---------------------------------------
    // CREATE SHIFT (Manager/Admin)
    // POST /EmployeeShifts
    // ---------------------------------------
    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<ActionResult<int>> Create(CreateEmployeeShiftCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(ListForDate), new { dateUtc = command.StartUtc.Date, shopLocationId = command.ShopLocationId }, new { id });
    }

    // ---------------------------------------
    // UPDATE SHIFT (Manager/Admin)
    // PUT /EmployeeShifts/{id}
    // ---------------------------------------
    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task Update(int id, UpdateEmployeeShiftCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
        // 204
    }

    // ---------------------------------------
    // DELETE SHIFT (soft delete) (Manager/Admin)
    // DELETE /EmployeeShifts/{id}
    // ---------------------------------------
    [HttpDelete("{id:int}")]
    [Authorize]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteEmployeeShiftCommand { Id = id }, ct);
        // 204
    }

    // ---------------------------------------
    // LIST SHIFTS FOR DATE (Manager/Admin)
    // GET /EmployeeShifts?dateUtc=YYYY-MM-DD&shopLocationId=1&employeeWorkMode=InShop
    // ---------------------------------------
    [HttpGet]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<PageResult<ListEmployeeShiftsForDateQueryDto>> ListForDate([FromQuery] ListEmployeeShiftsForDateQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }

    // ---------------------------------------
    // LIST MY SHIFTS (Employee)
    // GET /EmployeeShifts/mine?days=7
    // ---------------------------------------
    [HttpGet("mine")]
    [Authorize(Policy = AuthPolicies.EmployeeOnly)]
    public async Task<List<ListMyShiftsQueryDto>> ListMine([FromQuery] ListMyShiftsQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }

    // ---------------------------------------
    // EXPORT SHIFTS AS PDF (Manager/Admin)
    // GET /EmployeeShifts/export-pdf?startDate=2026-01-01&endDate=2026-01-31&shopLocationId=1
    // ---------------------------------------
    [HttpGet("export-pdf")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<IActionResult> ExportShiftsPdf(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int shopLocationId,
        [FromQuery] EmployeeWorkMode? employeeWorkMode,
        CancellationToken ct)
    {
        var query = new ExportShiftsQuery
        {
            StartDateUtc = startDate,
            EndDateUtc = endDate,
            ShopLocationId = shopLocationId,
            EmployeeWorkMode = employeeWorkMode,
        };
        var shifts = await sender.Send(query, ct);
        var locationName = shifts.FirstOrDefault()?.LocationName ?? string.Empty;

        var pdfBytes = ShiftsPdfGenerator.Generate(shifts, startDate, endDate, locationName);
        var fileName = $"shifts-{startDate:yyyy-MM-dd}-to-{endDate:yyyy-MM-dd}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }
}