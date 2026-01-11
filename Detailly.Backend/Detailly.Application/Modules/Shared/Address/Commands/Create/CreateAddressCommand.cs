using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detailly.Application.Modules.Shared.Address.Commands.Create
{
    public class CreateAddressCommand : IRequest<int>
    {
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public string? Region { get; set; }
        public required string Country { get; set; }

        public decimal? Latitude { get; set; } = 0;
        public decimal? Longitude { get; set; } = 0;
    }

}
