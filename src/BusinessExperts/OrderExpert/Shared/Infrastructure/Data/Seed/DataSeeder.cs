using Experts.OrderExpert.Shared.Infrastructure.Data;
using Experts.OrderExpert.Shared.Infrastructure.Data.Models;

public sealed class DataSeeder(OrdersDbContext db) {
    public async void Seed() {
        var orders = GetSeedOrders().ToList();
        db.Orders.AddRange(orders);
        db.SaveChanges();
    }

    private static IEnumerable<Order> GetSeedOrders() {
        yield return new Order {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
            CustomerId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
            Lines = [
                new OrderLine {
                    ProductId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    Quantity = 2,
                    UnitPrice = 19.99m
                }
            ]
        };
    }
}