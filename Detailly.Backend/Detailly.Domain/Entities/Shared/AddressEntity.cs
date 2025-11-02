using Detailly.Domain.Common;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Identity;
using Detailly.Domain.Entities.Sales;

namespace Detailly.Domain.Entities.Shared
{
    public class AddressEntity : BaseEntity
    {
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        // Foreign keys
        public IReadOnlyCollection<LocationEntity> Locations { get; private set; } = new List<LocationEntity>();
        public IReadOnlyCollection<ApplicationUserEntity> ApplicationUsers { get; private set; } = new List<ApplicationUserEntity>();
        public IReadOnlyCollection<OrderEntity> Orders { get; private set; } = new List<OrderEntity>();
    }
}