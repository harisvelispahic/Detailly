using Detailly.Application.Modules.Identity.User.Commands.Create;
using Detailly.Application.Modules.Identity.User.Commands.Update;
using Detailly.Application.Modules.Identity.User.Commands.Delete;
using Detailly.Application.Modules.Identity.User.Queries.GetById;
using Detailly.Application.Modules.Identity.User.Queries.List;
using Detailly.Application.Modules.Identity.User.Commands.ChangePassword;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> CreateUser(CreateUserCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateUserCommand command, CancellationToken ct)
    {
        // ID from the route takes precedence
        command.Id = id;
        await sender.Send(command, ct);
        // no return -> 204 No Content
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteUserCommand { Id = id }, ct);
        // no return -> 204 No Content
    }

    [HttpGet("{id:int}")]
    public async Task<GetUserByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var category = await sender.Send(new GetUserByIdQuery { Id = id }, ct);
        return category; // if NotFoundException -> 404 via middleware
    }

    [HttpGet]
    public async Task<PageResult<ListUsersQueryDto>> List([FromQuery] ListUsersQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task ChangePassword(ChangePasswordCommand command, CancellationToken ct)
    {
        await sender.Send(command, ct);
    }
}