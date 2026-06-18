using MediatR;
using oop.Api.Models;
using oop.Api.Repositories;

namespace oop.Api.Queries;

// =====================================================
// MEDIATOR PATTERN: Query (read operation)
// SOLID SRP: this file only handles GET ALL logic
// Queries NEVER change data — only read
// =====================================================

// Query — no input needed, just get all orders
public record GetAllOrdersQuery : IRequest<List<OrderResponse>>;

// Handler — fetches all orders and maps to response
public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, List<OrderResponse>>
{
    // SOLID DIP: depend on interface, not concrete class
    private readonly IOrderRepository _repository;

    public GetAllOrdersHandler(IOrderRepository repository)
        => _repository = repository;

    public async Task<List<OrderResponse>> Handle(
        GetAllOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await _repository.GetAllAsync(cancellationToken);

        // map each Order to OrderResponse
        return orders.Select(order => new OrderResponse(
            order.Id,
            order.CustomerName,
            order.Status.ToString(),
            order.TotalAmount,
            order.Items.Select(i => new OrderItemResponse(
                i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice)).ToList(),
            order.CreatedAt
        )).ToList();
    }
}