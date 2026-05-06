namespace Detailly.Application.Abstractions;

public interface ICloudinaryService
{
    Task<CloudinaryUploadResult> UploadAsync(Stream imageStream, string fileName, CancellationToken cancellationToken = default);
    Task DeleteAsync(string publicId, CancellationToken cancellationToken = default);
}

public record CloudinaryUploadResult(string SecureUrl, string PublicId);
