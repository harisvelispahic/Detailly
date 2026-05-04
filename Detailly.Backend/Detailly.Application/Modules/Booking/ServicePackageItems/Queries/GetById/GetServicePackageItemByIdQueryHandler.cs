namespace Detailly.Application.Modules.Booking.ServicePackageItems.Queries.GetById;

public sealed class GetServicePackageItemByIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetServicePackageItemByIdQuery, GetServicePackageItemByIdQueryDto>
{
    public async Task<GetServicePackageItemByIdQueryDto> Handle(
        GetServicePackageItemByIdQuery request, CancellationToken ct)
    {
        var item = await ctx.ServicePackageItems
            .AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x => new GetServicePackageItemByIdQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                DurationMinutes = x.DurationMinutes,
                RequiredEmployees = x.RequiredEmployees,
                IsAddon = x.IsAddon,
                IsActive = x.IsActive,
            })
            .FirstOrDefaultAsync(ct);

        if (item is null)
            throw new DetaillyNotFoundException("Service package item not found.");

        return item;
    }
}
