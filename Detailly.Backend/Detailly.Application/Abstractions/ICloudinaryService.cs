namespace Detailly.Application.Abstractions;

public interface ICloudinaryService
{
    Task<CloudinaryUploadResult> UploadAsync(Stream imageStream, string fileName, CancellationToken cancellationToken = default);
    Task DeleteAsync(string publicId, CancellationToken cancellationToken = default);
    CloudinaryDirectUploadParams GenerateDirectUploadParams(string folder);
    Task<CloudinaryDownloadResult> DownloadAsync(string imageUrl, CancellationToken cancellationToken = default);
}

public record CloudinaryDownloadResult(byte[] Bytes, string ContentType);

public record CloudinaryUploadResult(string SecureUrl, string PublicId);

public record CloudinaryDirectUploadParams(
    string CloudName,
    string ApiKey,
    long Timestamp,
    string Signature,
    string Folder);
