using MediatR;
using oop.Api.Repositories;

namespace oop.Api.Commands;

// MEDIATOR PATTERN
// SOLID SRP: this file only handles CONFIRM logic


// Command
public record ConfirmOrderCommand(int OrderId) : IRequest<bool>;

// Handler — confirms a pending order
public class ConfirmOrderHandler : IRequestHandler<ConfirmOrderCommand, bool>
{
    // SOLID DIP: depend on interface, not concrete class
    private readonly IOrderRepository _repository;

    public ConfirmOrderHandler(IOrderRepository repository)
        => _repository = repository;

    public async Task<bool> Handle(
        ConfirmOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);

        // order not found — controller will return 404
        if (order is null) return false;

        // OOP ENCAPSULATION + POLYMORPHISM
        order.Confirm();

        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}