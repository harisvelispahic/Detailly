using Microsoft.EntityFrameworkCore;
using Detailly.Domain.Entities.Shared;

namespace Detailly.Application.Modules.Shared.Address.Commands.Delete;

public class DeleteAddressCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteAddressCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAddressCommand request, CancellationToken ct)
    {
        var address = await context.Addresses
            .Include(a => a.ApplicationUsers) // Include povezanih usera
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (address is null)
            throw new DetaillyNotFoundException($"Address with Id {request.Id} was not found.");

        // Provjera constrainta
        if (address.ApplicationUsers.Any(u => !u.IsDeleted))
        {
            throw new ValidationException(
                "Cannot delete address because it is assigned to one or more active users.");
        }

        // Soft delete
        address.IsDeleted = true;

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
