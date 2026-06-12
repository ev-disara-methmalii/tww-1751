using OOP.Api.Models;
using MediatR;


//  SINGLE RESPONSIBILITY PRINCIPLE 

namespace OOP.Api.Queries;

public record GetOrderByIdQuery(int OrderId) : IRequest<OrderResponse?>;

public record GetAllOrdersQuery() : IRequest<List<OrderResponse>>;