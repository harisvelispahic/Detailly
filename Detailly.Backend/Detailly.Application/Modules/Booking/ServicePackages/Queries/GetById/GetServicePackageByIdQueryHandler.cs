using Detailly.Application.Modules.Booking.ServicePackages.Shared;

namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetById;

public class GetServicePackageByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetServicePackageByIdQuery, GetServicePackageByIdQueryDto>
{
    public async Task<GetServicePackageByIdQueryDto> Handle(GetServicePackageByIdQuery request, CancellationToken ct)
    {
        var package = await context.ServicePackages
            .Where(sp => sp.Id == request.Id)
            .FirstOrDefaultAsync(ct);

        if (package == null)
            throw new DetaillyNotFoundException($"Service package with Id {request.Id} not found.");

        if (package.IsDeleted)
            throw new DetaillyBusinessRuleException("SERVICE_PACKAGE_DELETED", "Service package does not exist.");

        var result = await context.ServicePackages
            .Where(sp => sp.Id == request.Id)
            .Select(sp => new GetServicePackageByIdQueryDto
            {
                Id = sp.Id,
                Name = sp.Name,
                Description = sp.Description,
                Price = sp.Price,
                EstimatedDurationHours = 1,

                Items = context.ServicePackageItemAssignments
                    .Where(a => a.ServicePackageId == sp.Id && !a.IsDeleted)
                    .Select(a => new GetServicePackageByIdQueryDtoItem
                    {
                        Id = a.ServicePackageItem.Id,
                        Name = a.ServicePackageItem.Name,
                        Price = a.ServicePackageItem.Price,
                        Description = a.ServicePackageItem.Description
                    })
                    .ToList(),

                Images = context.Images
                    .Where(i => i.ServicePackageId == sp.Id)
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => new ServicePackageImageDto(i.Id, i.ImageUrl, i.PublicId, i.IsThumbnail, i.DisplayOrder))
                    .ToList()
            })
            .FirstAsync(ct);

        return result;
    }
}
