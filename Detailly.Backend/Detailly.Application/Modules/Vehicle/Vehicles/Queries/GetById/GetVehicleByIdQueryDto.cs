using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.GetById
{
    public class GetVehicleByIdQueryDto
    {
        public required int Id { get; init; }
        public required string Model { get; init; }
        public required string Brand { get; init; }
        public required int YearOfManufacture { get; init; }
    }
}
