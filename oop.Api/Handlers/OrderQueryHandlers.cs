using oop.Api.Queries;
using oop.Api.Repositories;
using MediatR;
using oop.Api.Models;

//5: DEPENDENCY INVERSION PRINCIPLE (DIP)

namespace OOP.Api.Handlers;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderResponse?>   //LSP
{
    private readonly IOrderRepository _repository;

    public GetOrderByIdQueryHandler(IOrderRepository repository)
        => _repository = repository;

    public async Task<OrderResponse?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);
        return order is null ? null : MapToResponse(order);
    }

    private static OrderResponse MapToResponse(Order order) => new(
        order.Id,
        order.CustomerName,
        order.Status.ToString(),
        order.TotalAmount,
        order.Items.Select(i => new OrderItemResponse(
            i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice)).ToList(),
        order.CreatedAt
    );
}

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<OrderResponse>>
{
    private readonly IOrderRepository _repository;

    public GetAllOrdersQueryHandler(IOrderRepository repository)
        => _repository = repository;

    public async Task<List<OrderResponse>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _repository.GetAllAsync(cancellationToken);

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