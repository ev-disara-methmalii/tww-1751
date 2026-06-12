// =======================================================
// FILE: OrderModelTests.cs
// =======================================================
// PURPOSE:
// These are UNIT TESTS for the Order model.
// They test the business rules inside Order.cs directly.
//
// WHAT IS A UNIT TEST?
//    A unit test checks ONE small piece of logic in isolation.
//    No database. No HTTP. No external services.
//    Just the class being tested.
//
// PATTERN USED: ARRANGE / ACT / ASSERT (AAA)
//    ARRANGE = set up the data needed for the test
//    ACT     = call the method being tested
//    ASSERT  = check the result is what we expected
//
// WHY TEST THE ORDER MODEL?
//    Order.cs has business rules:
//      - Quantity must be > 0
//      - Cannot confirm an already confirmed order
//      - Cannot cancel a shipped order
//    These rules must ALWAYS work correctly.
//    Tests prove they work and alert us if they break later.
// =======================================================

using oop.Api.Models;
using FluentAssertions;

namespace oop.Tests.Unit;

public class OrderModelTests
{
    // =======================================================
    // TESTS FOR: AddItem()
    // =======================================================

    [Fact]
    public void AddItem_ValidItem_AddsToItemsList()
    {
        // ARRANGE: create a new order
        var order = new Order { CustomerName = "Dissara" };

        // ACT: add one item
        order.AddItem("Laptop", 1, 150000m);

        // ASSERT: the item should be in the list
        order.Items.Should().HaveCount(1);
        order.Items[0].ProductName.Should().Be("Laptop");
        order.Items[0].TotalPrice.Should().Be(150000m);
    }

    [Fact]
    public void AddItem_ZeroQuantity_ThrowsArgumentException()
    {
        // ARRANGE: create a new order
        var order = new Order { CustomerName = "Dissara" };

        // ACT: try to add an item with quantity 0
        var act = () => order.AddItem("Laptop", 0, 150000m);

        // ASSERT: should throw an error because quantity is 0
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Quantity must be greater than zero*");
    }

    [Fact]
    public void TotalAmount_MultipleItems_ReturnsSumOfAllItemTotals()
    {
        // ARRANGE: create order with two items
        var order = new Order { CustomerName = "Dissara" };
        order.AddItem("Laptop", 1, 150000m);
        order.AddItem("Mouse", 2, 2500m);

        // ACT: get the total amount
        var total = order.TotalAmount;

        // ASSERT: total should be 150000 + (2 x 2500) = 155000
        total.Should().Be(155000m);
    }

    // =======================================================
    // TESTS FOR: Confirm()
    // =======================================================

    [Fact]
    public void Confirm_PendingOrder_ChangesStatusToConfirmed()
    {
        // ARRANGE: new order starts as Pending
        var order = new Order { CustomerName = "Dissara" };

        // ACT: confirm the order
        order.Confirm();

        // ASSERT: status should now be Confirmed
        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void Confirm_AlreadyConfirmedOrder_ThrowsInvalidOperationException()
    {
        // ARRANGE: confirm the order once first
        var order = new Order { CustomerName = "Dissara" };
        order.Confirm();

        // ACT: try to confirm again
        var act = () => order.Confirm();

        // ASSERT: should throw error — cannot confirm twice
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Only pending orders can be confirmed*");
    }

    // =======================================================
    // TESTS FOR: Cancel()
    // =======================================================

    [Fact]
    public void Cancel_PendingOrder_ChangesStatusToCancelled()
    {
        // ARRANGE: new order starts as Pending
        var order = new Order { CustomerName = "Dissara" };

        // ACT: cancel the order
        order.Cancel();

        // ASSERT: status should now be Cancelled
        order.Status.Should().Be(OrderStatus.Cancelled);
    }
}