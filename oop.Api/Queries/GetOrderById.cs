using MediatR;
using oop.Api.Models;
using oop.Api.Repositories;


namespace oop.Api.Queries;

// =====================================================
// MEDIATOR PATTERN: Query (read operation)
// SOLID SRP: this file only handles GET BY ID logic
// Queries NEVER change data — only read
// =====================================================

// Query — takes an Id, returns one order or null
public record GetOrderByIdQuery(int OrderId) : IRequest<OrderResponse?>;

// Handler — fetches one order by Id and maps to response
public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderResponse?>
{
    // SOLID DIP: depend on interface, not concrete class
    private readonly IOrderRepository _repository;

    public GetOrderByIdHandler(IOrderRepository repository)
        => _repository = repository;

    public async Task<OrderResponse?> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);

        // return null if not found — controller will return 404
        if (order is null) return null;

        // map Order to OrderResponse
        return new OrderResponse(
            order.Id,
            order.CustomerName,
            order.Status.ToString(),
            order.TotalAmount,
            order.Items.Select(i => new OrderItemResponse(
                i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice)).ToList(),
            order.CreatedAt
        );
    }
}