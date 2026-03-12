namespace Detailly.Application.Modules.Catalog.ProductCategories.Queries.GetById;

public class GetProductCategoryByIdQuery : IRequest<GetProductCategoryByIdQueryDto>
{
    [JsonIgnore]
    public int Id { get; set; }
}