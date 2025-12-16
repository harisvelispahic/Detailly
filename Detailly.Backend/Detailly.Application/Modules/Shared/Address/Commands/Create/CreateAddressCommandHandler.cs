using Detailly.Application.Modules.Identity.User.Commands.Create;
using Detailly.Domain.Entities.Payment;
using Detailly.Domain.Entities.Sales;
using Detailly.Domain.Entities.Shared;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detailly.Application.Modules.Shared.Address.Commands.Create
{
    public class CreateAddressCommandHandler(IAppDbContext context) : IRequestHandler<CreateAddressCommand, int>
    {

        public async Task<int> Handle(CreateAddressCommand request, CancellationToken ct)
        {
            // Normalize values
            var street = request.Street.Trim();
            var city = request.City.Trim();
            var postalCode = request.PostalCode.Trim();
            var country = request.Country.Trim();
            var region = request.Region?.Trim();

            // Create entity
            var address = new AddressEntity
            {
                Street = street,
                City = city,
                PostalCode = postalCode,
                Region = region,
                Country = country,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            context.Addresses.Add(address);
            await context.SaveChangesAsync(ct);

            return address.Id;
        }
    }
}
