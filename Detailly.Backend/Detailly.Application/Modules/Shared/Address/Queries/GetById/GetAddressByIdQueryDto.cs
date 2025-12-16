using Detailly.Domain.Entities.Shared;

namespace Detailly.Application.Modules.Shared.Address.Queries.GetById;

public class GetAddressByIdQueryDto
{
    public required int Id { get; init; }
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }
    public string? Region { get; init; }
    public required string Country { get; init; }

    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
}
