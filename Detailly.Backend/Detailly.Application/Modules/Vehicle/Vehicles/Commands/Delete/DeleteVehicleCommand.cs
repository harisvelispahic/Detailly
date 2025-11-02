using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Delete
{
    public class DeleteVehicleCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
    }
}
