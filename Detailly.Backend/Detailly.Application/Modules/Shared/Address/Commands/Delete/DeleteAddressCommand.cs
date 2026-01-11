namespace Detailly.Application.Modules.Shared.Address.Commands.Delete;

public class DeleteAddressCommand : IRequest<Unit>
{
    public int Id { get; set; }
}
