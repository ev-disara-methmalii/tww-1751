using MediatR;
using oop.Api.Models;
using oop.Api.Repositories;

namespace oop.Api.Commands;

// MEDIATOR PATTERN
// SOLID SRP
// SOLID OCP

// Command 
public record CreateOrderCommand(
    string CustomerName,
    List<OrderItemDto> Items
) : IRequest<int>;

// Shared DTO used inside the command
public record OrderItemDto(string ProductName, int Quantity, decimal UnitPrice);

// Handler — does the actual work for CreateOrderCommand
public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
{
    // SOLID DIP: depend on interface, not concrete class
    private readonly IOrderRepository _repository;

    public CreateOrderHandler(IOrderRepository repository)
        => _repository = repository;

    public async Task<int> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        // OOP CLASS AND OBJECT: creating a new Order object
        var order = new Order { CustomerName = request.CustomerName };

        // OOP ENCAPSULATION: AddItem() enforces the quantity rule
        foreach (var item in request.Items)
            order.AddItem(item.ProductName, item.Quantity, item.UnitPrice);

        await _repository.AddAsync(order, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}