namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetUploadParams;

public record GetServicePackageUploadParamsQueryDto(
    string CloudName,
    string ApiKey,
    long Timestamp,
    string Signature,
    string Folder);
