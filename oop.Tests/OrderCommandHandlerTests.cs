// =======================================================
// FILE: OrderCommandHandlerTests.cs
// =======================================================
// PURPOSE:
// These are UNIT TESTS for the command handlers.
// They test the handler logic in complete isolation.
//
// WHAT IS MOCKING?
//    Handlers depend on IOrderRepository.
//    In unit tests we do NOT want to use a real database.
//    So we create a FAKE (mock) repository using Moq.
//
//    A mock lets us:
//    - Control what the fake repository returns
//    - Verify which methods were called
//    - Test handler logic without any database
//
// HOW MOQ WORKS:
//    var mockRepo = new Mock<IOrderRepository>();
//    This creates a fake IOrderRepository.
//
//    mockRepo.Setup(r => r.GetByIdAsync(1, ...)).ReturnsAsync(order);
//    This says: "when GetByIdAsync(1) is called, return this order"
//
//    mockRepo.Verify(r => r.AddAsync(...), Times.Once);
//    This checks: "was AddAsync called exactly once?"
// =======================================================

using FluentAssertions;
using Moq;
using OOP.Api.Commands;
using OOP.Api.Handlers;
using oop.Api.Models;
using oop.Api.Repositories;
using OOP.Api.Commands;
using OOP.Api.Handlers;

namespace oop.Tests.Unit;

public class OrderCommandHandlerTests
{
    // =======================================================
    // TESTS FOR: CreateOrderCommandHandler
    // =======================================================

    [Fact]
    public async Task CreateOrder_ValidCommand_CallsAddAndSave()
    {
        // ARRANGE: create a fake repository
        var mockRepo = new Mock<IOrderRepository>();
        var handler = new CreateOrderCommandHandler(mockRepo.Object);

        var command = new CreateOrderCommand(
            "Dissara",
            new List<OrderItemDto> { new("Laptop", 1, 150000m) }
        );

        // ACT: run the handler
        await handler.Handle(command, CancellationToken.None);

        // ASSERT: verify AddAsync was called once
        mockRepo.Verify(r => r.AddAsync(
            It.IsAny<Order>(),
            It.IsAny<CancellationToken>()), Times.Once);

        // ASSERT: verify SaveChangesAsync was called once
        mockRepo.Verify(r => r.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // =======================================================
    // TESTS FOR: ConfirmOrderCommandHandler
    // =======================================================

    [Fact]
    public async Task ConfirmOrder_ExistingOrder_ReturnsTrue()
    {
        // ARRANGE: set up fake repository to return an order
        var order = new Order { CustomerName = "Dissara" };
        var mockRepo = new Mock<IOrderRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

        var handler = new ConfirmOrderCommandHandler(mockRepo.Object);

        // ACT: run the handler
        var result = await handler.Handle(
            new ConfirmOrderCommand(1), CancellationToken.None);

        // ASSERT: should return true and order should be Confirmed
        result.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task ConfirmOrder_NonExistingOrder_ReturnsFalse()
    {
        // ARRANGE: set up fake repository to return null (order not found)
        var mockRepo = new Mock<IOrderRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

        var handler = new ConfirmOrderCommandHandler(mockRepo.Object);

        // ACT: run the handler with an order Id that does not exist
        var result = await handler.Handle(
            new ConfirmOrderCommand(99), CancellationToken.None);

        // ASSERT: should return false because order was not found
        result.Should().BeFalse();
    }

    // =======================================================
    // TESTS FOR: CancelOrderCommandHandler
    // =======================================================

    [Fact]
    public async Task CancelOrder_ExistingPendingOrder_ReturnsTrue()
    {
        // ARRANGE: set up fake repository to return an order
        var order = new Order { CustomerName = "Dissara" };
        var mockRepo = new Mock<IOrderRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

        var handler = new CancelOrderCommandHandler(mockRepo.Object);

        // ACT: run the handler
        var result = await handler.Handle(
            new CancelOrderCommand(1), CancellationToken.None);

        // ASSERT: should return true and order should be Cancelled
        result.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }
}