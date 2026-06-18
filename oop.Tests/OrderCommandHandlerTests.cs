

using FluentAssertions;
using Moq;
using oop.Api.Commands;
using OOP.Api.Handlers;
using oop.Api.Models;
using oop.Api.Repositories;

namespace oop.Tests.Unit;

public class OrderCommandHandlerTests
{

    [Fact]
    public async Task CreateOrder_ValidCommand_CallsAddAndSave()
    {
        var mockRepo = new Mock<IOrderRepository>();
        var handler = new CreateOrderCommandHandler(mockRepo.Object);

        var command = new CreateOrderCommand(
            "Disara",
            new List<OrderItemDto> { new("Laptop", 1, 150000m) }
        );

        // ACT
        await handler.Handle(command, CancellationToken.None);

        // ASSERT
        mockRepo.Verify(r => r.AddAsync(
            It.IsAny<Order>(),
            It.IsAny<CancellationToken>()), Times.Once);

        // ASSERT
        mockRepo.Verify(r => r.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task ConfirmOrder_ExistingOrder_ReturnsTrue()
    {
        // ARRANGE
        var order = new Order { CustomerName = "Disara" };
        var mockRepo = new Mock<IOrderRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

        var handler = new ConfirmOrderCommandHandler(mockRepo.Object);

        // ACT
        var result = await handler.Handle(
            new ConfirmOrderCommand(1), CancellationToken.None);

        // ASSERT
        result.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task ConfirmOrder_NonExistingOrder_ReturnsFalse()
    {
        // ARRANGE
        var mockRepo = new Mock<IOrderRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

        var handler = new ConfirmOrderCommandHandler(mockRepo.Object);

        // ACT
        var result = await handler.Handle(
            new ConfirmOrderCommand(99), CancellationToken.None);

        // ASSERT
        result.Should().BeFalse();
    }


    [Fact]
    public async Task CancelOrder_ExistingPendingOrder_ReturnsTrue()
    {
        // ARRANGE
        var order = new Order { CustomerName = "Disara" };
        var mockRepo = new Mock<IOrderRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

        var handler = new CancelOrderCommandHandler(mockRepo.Object);

        // ACT
        var result = await handler.Handle(
            new CancelOrderCommand(1), CancellationToken.None);

        // ASSERT
        result.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }
}