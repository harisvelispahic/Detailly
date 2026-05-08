namespace Detailly.Application.Modules.Sales.Orders.Queries.GetOrderDetails;

public sealed class GetOrderDetailsQueryValidator : AbstractValidator<GetOrderDetailsQuery>
{
    public GetOrderDetailsQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Order ID must be greater than zero.");
    }
}
