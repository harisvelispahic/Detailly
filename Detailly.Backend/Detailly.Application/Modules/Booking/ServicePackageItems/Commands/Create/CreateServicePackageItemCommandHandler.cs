using Detailly.Domain.Entities.Booking;

namespace Detailly.Application.Modules.Booking.ServicePackageItems.Commands.Create;

public class CreateServicePackageItemCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateServicePackageItemCommand, int>
{
    public async Task<int> Handle(CreateServicePackageItemCommand request, CancellationToken ct)
    {
        var item = new ServicePackageItemEntity
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            DurationMinutes = request.DurationMinutes,
            RequiredEmployees = request.RequiredEmployees,
            IsAddon = request.IsAddon,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        context.ServicePackageItems.Add(item);
        await context.SaveChangesAsync(ct);

        return item.Id;
    }
}
