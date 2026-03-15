namespace Detailly.Application.Modules.Shared.Address.Queries.ListMine;

public sealed class ListMyAddressesQuery : BasePagedQuery<ListMyAddressesQueryDto>
{
    public string? Search { get; init; }
}