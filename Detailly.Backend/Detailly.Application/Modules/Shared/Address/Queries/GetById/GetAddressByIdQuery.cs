namespace Detailly.Application.Modules.Shared.Address.Queries.GetById;

public class GetAddressByIdQuery : IRequest<GetAddressByIdQueryDto>
{
    public int Id { get; set; }
}
