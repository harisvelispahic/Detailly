using Detailly.Domain.Common;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Identity;
using Detailly.Domain.Entities.Sales;
using System.ComponentModel.DataAnnotations;

namespace Detailly.Domain.Entities.Shared;

public class AddressEntity : BaseEntity
{
    [MaxLength(250)]
    public string? Street { get; set; }
    
    [MaxLength(100)]
    public string? City { get; set; }
    
    [MaxLength(20)]
    public string? PostalCode { get; set; }
    
    [MaxLength(100)]
    public string? Region { get; set; }
    
    [MaxLength(100)]
    public string? Country { get; set; }
    
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    // Foreign keys
    public IReadOnlyCollection<LocationEntity> Locations { get; private set; } = new List<LocationEntity>();
    public IReadOnlyCollection<ApplicationUserEntity> ApplicationUsers { get; private set; } = new List<ApplicationUserEntity>();
    public IReadOnlyCollection<OrderEntity> Orders { get; private set; } = new List<OrderEntity>();
}