namespace Detailly.Application.Modules.Sales.Orders.Commands.Submit;

public sealed class SubmitOrderCommand : IRequest
{
    public int Id { get; set; }
}