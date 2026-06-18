using MediatR;
using Microsoft.AspNetCore.Mvc;
using oop.Api.Commands;
using oop.Api.Queries;
using oop.Api.Models;

namespace oop.Api.Controllers;
//Mediator Pattern
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator) => _mediator = mediator;

    // GET api/orders
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllOrdersQuery());
        return Ok(result);
    }

    // GET api/orders/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    // POST api/orders
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var command = new CreateOrderCommand(
            request.CustomerName,
            request.Items.Select(i => new OrderItemDto(
                i.ProductName, i.Quantity, i.UnitPrice)).ToList()
        );

        var newId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId });
    }

    // PUT api/orders/5/confirm
    [HttpPut("{id:int}/confirm")]
    public async Task<IActionResult> Confirm(int id)
    {
        var success = await _mediator.Send(new ConfirmOrderCommand(id));

        if (!success) return NotFound();
        return NoContent();
    }

    // PUT api/orders/5/cancel
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var success = await _mediator.Send(new CancelOrderCommand(id));

        if (!success) return NotFound();
        return NoContent();
    }
}