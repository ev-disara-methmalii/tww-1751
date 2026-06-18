namespace oop.Api.Models;

// Request DTOs
public record CreateOrderRequest(
	string CustomerName,
	List<OrderItemRequest> Items
);

public record OrderItemRequest(
	string ProductName,
	int Quantity,
	decimal UnitPrice
);

// Response DTOs
public record OrderResponse(
	int Id,
	string CustomerName,
	string Status,
	decimal TotalAmount,
	List<OrderItemResponse> Items,
	DateTime CreatedAt
);

public record OrderItemResponse(
	string ProductName,
	int Quantity,
	decimal UnitPrice,
	decimal TotalPrice
);