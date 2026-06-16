using System.ComponentModel.DataAnnotations.Schema;

namespace oop.Api.Models;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}

public class Order : BaseEntity
{
    public string CustomerName { get; set; } = string.Empty;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    private readonly List<OrderItem> _items = new();
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    // NotMapped = EF Core ignores this, it is calculated in memory only
    [NotMapped]
    public decimal TotalAmount => _items.Sum(i => i.TotalPrice);

    public void AddItem(string productName, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        _items.Add(new OrderItem
        {
            ProductName = productName,
            Quantity = quantity,
            UnitPrice = unitPrice
        });
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed.");

        Status = OrderStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped)
            throw new InvalidOperationException("Shipped orders cannot be cancelled.");

        Status = OrderStatus.Cancelled;
    }
}

public class OrderItem
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    [NotMapped]
    public decimal TotalPrice => Quantity * UnitPrice;
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped,
    Cancelled
}