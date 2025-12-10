using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detailly.Application.Modules.Identity.User.Commands.Delete;
public class DeleteUserCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
