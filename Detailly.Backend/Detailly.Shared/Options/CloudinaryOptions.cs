using System.ComponentModel.DataAnnotations;

namespace Detailly.Shared.Options;

public sealed class CloudinaryOptions
{
    public const string SectionName = "Cloudinary";

    [Required]
    public string CloudName { get; init; } = string.Empty;

    [Required]
    public string ApiKey { get; init; } = string.Empty;

    [Required]
    public string ApiSecret { get; init; } = string.Empty;
}
