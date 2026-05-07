using Detailly.Application.Modules.Booking.ServicePackages.Commands.Create;
using Detailly.Application.Modules.Booking.ServicePackages.Commands.Delete;
using Detailly.Application.Modules.Booking.ServicePackages.Commands.Image.Confirm;
using Detailly.Application.Modules.Booking.ServicePackages.Commands.Image.Delete;
using Detailly.Application.Modules.Booking.ServicePackages.Commands.Image.SetThumbnail;
using Detailly.Application.Modules.Booking.ServicePackages.Commands.Image.Upload;
using Detailly.Application.Modules.Booking.ServicePackages.Commands.Update;
using Detailly.Application.Modules.Booking.ServicePackages.Queries.GetAvailableAddons;
using Detailly.Application.Modules.Booking.ServicePackages.Queries.GetById;
using Detailly.Application.Modules.Booking.ServicePackages.Queries.GetImage;
using Detailly.Application.Modules.Booking.ServicePackages.Queries.GetUploadParams;
using Detailly.Application.Modules.Booking.ServicePackages.Queries.List;
using Detailly.Application.Modules.Booking.ServicePackages.Shared;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ServicePackagesController(ISender sender, IHttpClientFactory httpClientFactory) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<ActionResult<int>> Create(CreateServicePackageCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task Update(int id, UpdateServicePackageCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteServicePackageCommand { Id = id }, ct);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<GetServicePackageByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        return await sender.Send(new GetServicePackageByIdQuery { Id = id }, ct);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<PageResult<ListServicePackagesQueryDto>> List([FromQuery] ListServicePackagesQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }

    [HttpGet("available-addons")]
    [AllowAnonymous]
    public async Task<ActionResult<PageResult<GetAvailableAddonsQueryDto>>> GetAvailableAddons([FromQuery] GetAvailableAddonsQuery query, CancellationToken ct)
    {
        return Ok(await sender.Send(query, ct));
    }

    // ── Images ───────────────────────────────────────────────────────────────

    [HttpGet("{id:int}/images/upload-params")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<ActionResult<GetServicePackageUploadParamsQueryDto>> GetUploadParams(int id, CancellationToken ct)
    {
        return Ok(await sender.Send(new GetServicePackageUploadParamsQuery { ServicePackageId = id }, ct));
    }

    [HttpPost("{id:int}/images/confirm")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<ActionResult<ServicePackageImageDto>> ConfirmImage(
        int id, ConfirmServicePackageImageCommand command, CancellationToken ct)
    {
        command.ServicePackageId = id;
        return Ok(await sender.Send(command, ct));
    }

    [HttpPost("{id:int}/images")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<ActionResult<ServicePackageImageDto>> UploadImage(
        int id, IFormFile file, CancellationToken ct)
    {
        var result = await sender.Send(new UploadServicePackageImageCommand
        {
            ServicePackageId = id,
            FileStream = file.OpenReadStream(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
        }, ct);
        return Ok(result);
    }

    [HttpDelete("{id:int}/images/{imageId:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task DeleteImage(int id, int imageId, CancellationToken ct)
    {
        await sender.Send(
            new DeleteServicePackageImageCommand { ServicePackageId = id, ImageId = imageId }, ct);
    }

    [HttpPatch("{id:int}/images/{imageId:int}/thumbnail")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task SetThumbnail(int id, int imageId, CancellationToken ct)
    {
        await sender.Send(
            new SetServicePackageThumbnailCommand { ServicePackageId = id, ImageId = imageId }, ct);
    }

    [HttpGet("{id:int}/images/{imageId:int}/download")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadImage(int id, int imageId, CancellationToken ct)
    {
        var info = await sender.Send(
            new GetServicePackageImageQuery { ServicePackageId = id, ImageId = imageId }, ct);

        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync(info.ImageUrl, ct);
        response.EnsureSuccessStatusCode();

        var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
        var bytes = await response.Content.ReadAsByteArrayAsync(ct);

        return File(bytes, contentType, info.FileName);
    }
}
