namespace Detailly.Application.Modules.Shared.Address.Commands.Delete;

public sealed class DeleteAddressCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
}