namespace Detailly.Application.Modules.Shared.Address.Commands.Update;

public class UpdateAddressCommand : IRequest<Unit>
{
    public required int Id { get; set; }

    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Region { get; set; }
    public string? Country { get; set; }

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}
