
// SOLID 4: INTERFACE SEGREGATION PRINCIPLE (ISP)

// SOLID 5: DEPENDENCY INVERSION PRINCIPLE (DIP)

using oop.Api.Models;

namespace oop.Api.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<List<Order>> GetAllAsync(CancellationToken ct = default);

    Task AddAsync(Order order, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}