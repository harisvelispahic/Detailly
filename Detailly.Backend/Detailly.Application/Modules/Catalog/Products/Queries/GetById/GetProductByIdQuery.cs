namespace Detailly.Application.Modules.Catalog.Products.Queries.GetById;

public class GetProductByIdQuery : IRequest<GetProductByIdQueryDto>
{
    [JsonIgnore]
    public int Id { get; set; }
}