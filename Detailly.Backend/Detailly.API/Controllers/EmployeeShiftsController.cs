
using Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Create;
using Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Delete;
using Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Update;
using Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ListForDate;

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
    [Authorize]
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
    [Authorize]
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
    [Authorize]
    public async Task<List<ListEmployeeShiftsForDateQueryDto>> ListForDate([FromQuery] ListEmployeeShiftsForDateQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }
}