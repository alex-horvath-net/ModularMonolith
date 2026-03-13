using Orders.Core.Infrastructure.Data.Models;

namespace Orders.Core.Infrastructure.Data.Seed;

public sealed class DataProvider() {
    public IReadOnlyList<Order> GetSeedOrders() => [
        new() {
            Id = Id(1),
            CustomerId = Id(1),
            Lines = [
                new() { Id = Id(1), ProductId = Id(1), Quantity = 1, UnitPrice = 10.50m },
                new() { Id = Id(2), ProductId = Id(2), Quantity = 2, UnitPrice = 15.75m }
            ]
        },
        new() {
            Id = Id(2),
            CustomerId = Id(1),
            Lines = [
                new() { Id = Id(3), ProductId = Id(1), Quantity = 1, UnitPrice = 25.00m },
                new() { Id = Id(4), ProductId = Id(2), Quantity = 3, UnitPrice = 5.25m }
            ]
        },
        new() {
            Id = Id(3),
            CustomerId = Id(2),
            Lines = [
                new() { Id = Id(5), ProductId = Id(1), Quantity = 2, UnitPrice = 12.99m },
                new() { Id = Id(6), ProductId = Id(2), Quantity = 4, UnitPrice = 3.50m }
            ]
        }
    ];

    private Guid Id(int id) => Guid.Parse($"10000000-0000-0000-0000-{id:D12}");
}