namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Update
{
    public class UpdateVehicleCommandHandler(IAppDbContext ctx)
        : IRequestHandler<UpdateVehicleCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateVehicleCommand request, CancellationToken ct)
        {
            // Find the vehicle
            var entity = await ctx.Vehicles
                .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

            if (entity is null)
                throw new MarketNotFoundException($"Vehicle (ID={request.Id}) was not found.");

            // Optional: Check for duplicate Brand+Model+Year if all three are provided
            if (request.Brand is not null && request.Model is not null && request.YearOfManufacture.HasValue)
            {
                var exists = await ctx.Vehicles.AnyAsync(x =>
                    x.Id != request.Id &&
                    x.Brand.ToLower() == request.Brand.ToLower() &&
                    x.Model.ToLower() == request.Model.ToLower() &&
                    x.YearOfManufacture == request.YearOfManufacture.Value,
                    ct);

                if (exists)
                    throw new MarketConflictException("A vehicle with the same Brand, Model, and Year already exists.");
            }

            // Update only the properties that were sent
            if (!string.IsNullOrWhiteSpace(request.Brand))
                entity.Brand = request.Brand.Trim();

            if (!string.IsNullOrWhiteSpace(request.Model))
                entity.Model = request.Model.Trim();

            if (request.YearOfManufacture.HasValue)
            {
                if (request.YearOfManufacture.Value < 1886 || request.YearOfManufacture.Value > 2100)
                    throw new ArgumentException("Invalid Year of Manufacture.");

                entity.YearOfManufacture = request.YearOfManufacture.Value;
            }

            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
