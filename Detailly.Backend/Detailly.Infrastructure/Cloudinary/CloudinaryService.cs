using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Detailly.Application.Abstractions;
using Detailly.Shared.Options;
using Microsoft.Extensions.Options;

namespace Detailly.Infrastructure.Cloudinary;

public sealed class CloudinaryService : ICloudinaryService
{
    private readonly CloudinaryDotNet.Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinaryOptions> options)
    {
        var o = options.Value;
        _cloudinary = new CloudinaryDotNet.Cloudinary(new Account(o.CloudName, o.ApiKey, o.ApiSecret));
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
}
