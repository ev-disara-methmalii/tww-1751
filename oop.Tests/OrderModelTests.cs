using oop.Api.Models;
using FluentAssertions;

namespace oop.Tests.Unit;

public class OrderModelTests
{
  
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