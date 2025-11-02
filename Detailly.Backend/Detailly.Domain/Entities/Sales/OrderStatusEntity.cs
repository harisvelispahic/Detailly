using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Sales
{
    public class OrderStatusEntity : BaseEntity
    {
        public string? Name { get; set; } // Pending, Completed, Failed -> prebaciti u enum

        // Foreign keys
        public IReadOnlyCollection<OrderEntity> Orders { get; private set; } = new List<OrderEntity>();
    }
}
