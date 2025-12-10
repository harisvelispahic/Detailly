namespace Detailly.Application.Modules.Identity.User.Queries.GetById;
public class GetUserByIdQuery : IRequest<GetUserByIdQueryDto>
{
    public int Id { get; set; }
}
