
using Detailly.Domain.Entities.Shared;

namespace Detailly.Application.Modules.Identity.User.Commands.Update;

public sealed class UpdateUserCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateUserCommand, Unit>
{
    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        var user = await context.ApplicationUsers
            .Include(x => x.Address)
            .Include(x => x.Image)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (user == null)
            throw new DetaillyNotFoundException("User not found.");

        // PERSONAL INFO
        if (request.FirstName != null)
            user.FirstName = request.FirstName.Trim();

        if (request.LastName != null)
            user.LastName = request.LastName.Trim();


        if (request.Email != null)
        {
            var exists = await context.ApplicationUsers
                .AnyAsync(x => x.Email == request.Email && x.Id != request.Id, ct);

            if (exists)
                throw new DetaillyConflictException("Email already exists.");

            user.Email = request.Email.Trim().ToLower();
        }

        if (request.Username != null)
        {
            var exists = await context.ApplicationUsers
                .AnyAsync(x => x.Username == request.Username && x.Id != request.Id, ct);

            if (exists)
                throw new DetaillyConflictException("Username already exists.");

            user.Username = request.Username.Trim();
        }

        if (request.Phone != null)
            user.Phone = request.Phone.Trim();

        if (request.CompanyName != null)
            user.CompanyName = request.CompanyName.Trim();

        // ADDRESS (create if missing)
        if (request.Address != null)
        {
            user.Address ??= new AddressEntity();

            if (request.Address.Street != null)
                user.Address.Street = request.Address.Street.Trim();

            if (request.Address.City != null)
                user.Address.City = request.Address.City.Trim();

            if (request.Address.Region != null)
                user.Address.Region = request.Address.Region.Trim();

            if (request.Address.PostalCode != null)
                user.Address.PostalCode = request.Address.PostalCode.Trim();

            if (request.Address.Country != null)
                user.Address.Country = request.Address.Country.Trim();

            if (request.Address.Latitude.HasValue)
                user.Address.Latitude = request.Address.Latitude.Value;

            if (request.Address.Longitude.HasValue)
                user.Address.Longitude = request.Address.Longitude.Value;
        }

        // IMAGE
        if (request.Image?.ImageUrl != null)
        {
            if (user.Image == null)
            {
                user.Image = new ImageEntity
                {
                    ImageUrl = request.Image.ImageUrl.Trim()
                };
            }
            else
            {
                user.Image.ImageUrl = request.Image.ImageUrl.Trim();
            }
        }

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}