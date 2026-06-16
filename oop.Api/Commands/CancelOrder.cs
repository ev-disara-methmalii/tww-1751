using MediatR;
using oop.Api.Repositories;

namespace oop.Api.Commands;
// MEDIATOR PATTERN
// SOLID SRP: this file only handles CANCEL logic


// Command 
public record CancelOrderCommand(int OrderId) : IRequest<bool>;

// Handler — cancels an order
public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, bool>
{
    // SOLID DIP: depend on interface, not concrete class
    private readonly IOrderRepository _repository;

    public CancelOrderHandler(IOrderRepository repository)
        => _repository = repository;

    public async Task<bool> Handle(
        CancelOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);

        // order not found — controller will return 404
        if (order is null) return false;

        // OOP ENCAPSULATION
        order.Cancel();

        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}