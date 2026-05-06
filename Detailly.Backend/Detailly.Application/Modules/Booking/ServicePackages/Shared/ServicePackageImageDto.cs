namespace Detailly.Application.Modules.Booking.ServicePackages.Shared;

public record ServicePackageImageDto(
    int Id,
    string ImageUrl,
    string? PublicId,
    bool IsThumbnail,
    int DisplayOrder);
