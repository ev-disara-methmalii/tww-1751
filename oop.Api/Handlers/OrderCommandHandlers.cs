using oop.Api.Commands;
using oop.Api.Models;
using oop.Api.Repositories;
using MediatR;

namespace OOP.Api.Handlers;
//OPEN/CLOSED PRINCIPLE(OCP)
// MEDIATOR PATTERN: Each handler has ONE job (SRP).


public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IOrderRepository _repository;

    public CreateOrderCommandHandler(IOrderRepository repository)
        => _repository = repository;

    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order { CustomerName = request.CustomerName };

        foreach (var item in request.Items)
            order.AddItem(item.ProductName, item.Quantity, item.UnitPrice);

        await _repository.AddAsync(order, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}

public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, bool>
{
    private readonly IOrderRepository _repository;

    public ConfirmOrderCommandHandler(IOrderRepository repository)
        => _repository = repository;

    public async Task<bool> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null) return false;

        order.Confirm();
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IOrderRepository _repository;

    public CancelOrderCommandHandler(IOrderRepository repository)
        => _repository = repository;

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null) return false;

        order.Cancel();
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}