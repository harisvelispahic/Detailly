namespace Detailly.Application.Modules.Shared.Address.Queries.GetById;

public sealed class GetAddressByIdQuery : IRequest<GetAddressByIdQueryDto>
{
    [JsonIgnore]
    public int Id { get; set; }
}