namespace Detailly.Application.Modules.Identity.Staff.Queries.GetById;

public class GetStaffMemberByIdQuery : IRequest<GetStaffMemberByIdQueryDto>
{
    public int Id { get; set; }
}
