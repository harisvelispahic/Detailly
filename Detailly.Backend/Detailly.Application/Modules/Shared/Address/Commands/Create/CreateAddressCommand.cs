namespace Detailly.Application.Modules.Shared.Address.Commands.Create;

public sealed class CreateAddressCommand : IRequest<int>
{
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string PostalCode { get; set; }
    public string? Region { get; set; }
    public required string Country { get; set; }

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}