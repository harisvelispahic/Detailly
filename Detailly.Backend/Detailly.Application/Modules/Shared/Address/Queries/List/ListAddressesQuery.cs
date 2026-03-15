namespace Detailly.Application.Modules.Shared.Address.Queries.List;

public sealed class ListAddressesQuery : BasePagedQuery<ListAddressesQueryDto>
{
    public string? Search { get; init; }
}