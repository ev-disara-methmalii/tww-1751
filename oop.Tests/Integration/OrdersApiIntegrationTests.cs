using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using oop.Api.Models;
using oop.Api.Repositories;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace oop.Tests.Integration;

public class OrdersApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OrdersApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");

                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));
                });
            })
            .CreateClient();
    }


    [Fact]
    public async Task GetAllOrders_EmptyDatabase_ReturnsEmptyList()
    {
        var response = await _client.GetAsync("/api/orders");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        var orders = JsonSerializer.Deserialize<List<OrderResponse>>(body, _jsonOptions);
        orders.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateOrder_ValidRequest_Returns201Created()
    {
        var request = new
        {
            customerName = "Dissara",
            items = new[]
            {
                new { productName = "Laptop", quantity = 1, unitPrice = 150000.00 }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }



    [Fact]
    public async Task GetOrderById_NotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/orders/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

}