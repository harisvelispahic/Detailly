
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Sales;

namespace Detailly.Application.Modules.Sales.Orders.Commands.Create;

public class CreateOrderCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<CreateOrderCommand, int>
{
    public async Task<int> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        try
        {
            var order = new OrderEntity
            {
                OrderNumber = Guid.NewGuid().ToString(),
                OrderDate = DateTime.UtcNow,
                TotalAmount = 0.0m,     // update when order items are inserted
                Notes = request.Notes?.Trim(),
                ShipToAddressId = request.ShipToAddressId,
                ApplicationUserId = appCurrentUser.ApplicationUserId!.Value,
                Status = OrderStatus.Draft
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync(ct);

            decimal totalAmount = 0;

            foreach (var item in request.OrderItems)
            {
                var orderItem = new OrderItemEntity
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    UnitPrice = item.UnitPrice,
                    Currency=item.Currency,
                    Quantity = item.Quantity,
                    LineSubtotal = item.UnitPrice * item.Quantity,
                    DiscountPercentage = 0.05m,
                    LineTotal = (1 - 0.05m) * item.UnitPrice * item.Quantity
                };

                totalAmount += orderItem.LineTotal;

                await context.OrderItems.AddAsync(orderItem, ct);
            }
            
            order.TotalAmount = totalAmount;
            await context.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);

            return order.Id;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw; // rethrow the exception to be handled elsewhere
        }
    }

}