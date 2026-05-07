using System.Security.Cryptography;
using System.Text;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Detailly.Application.Abstractions;
using Detailly.Shared.Options;
using Microsoft.Extensions.Options;

namespace Detailly.Infrastructure.Cloudinary;

public sealed class CloudinaryService : ICloudinaryService
{
    private readonly CloudinaryDotNet.Cloudinary _cloudinary;
    private readonly CloudinaryOptions _options;

    public CloudinaryService(IOptions<CloudinaryOptions> options)
    {
        _options = options.Value;
        _cloudinary = new CloudinaryDotNet.Cloudinary(new Account(_options.CloudName, _options.ApiKey, _options.ApiSecret));
        _cloudinary.Api.Secure = true;
    }

    public async Task<CloudinaryUploadResult> UploadAsync(Stream imageStream, string fileName, CancellationToken cancellationToken = default)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, imageStream),
            UseFilename = true,
            UniqueFilename = true,
            Overwrite = false
        };

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (result.Error != null)
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");

        return new CloudinaryUploadResult(result.SecureUrl.ToString(), result.PublicId);
    }

    public async Task DeleteAsync(string publicId, CancellationToken cancellationToken = default)
    {
        var deletionParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deletionParams);

        if (result.Error != null)
            throw new InvalidOperationException($"Cloudinary deletion failed: {result.Error.Message}");
    }

    public CloudinaryDirectUploadParams GenerateDirectUploadParams(string folder)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        // Cloudinary signature: alphabetically sorted params string + API secret, SHA-1
        var toSign = $"folder={folder}&timestamp={timestamp}{_options.ApiSecret}";
        var hashBytes = SHA1.HashData(Encoding.UTF8.GetBytes(toSign));
        var signature = Convert.ToHexString(hashBytes).ToLowerInvariant();
        return new CloudinaryDirectUploadParams(_options.CloudName, _options.ApiKey, timestamp, signature, folder);
    }
}
