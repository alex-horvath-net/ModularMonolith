using Experts.OrderExpert.Shared.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Experts.OrderExpert.Shared.Infrastructure.Data.Seed;

public sealed class DataSeeder(OrdersDbContext db, ILogger<DataSeeder> logger) {
    public void Seed() {
        var hasOrders = GetSeedOrders();
        if (hasOrders.Count == 0)
            return;

        using var transaction = db.Database.BeginTransaction();

        var existingOrders = db.Orders.Include(o => o.Lines).ToList();

        foreach (var hasOrder in hasOrders) {
            var order = existingOrders.SingleOrDefault(o => o.Id == hasOrder.Id);
            if (order is null) {
                db.Orders.Add(hasOrder);
            } else {
                order.CustomerId = hasOrder.CustomerId;
                SyncLines(order, hasOrder);
            }
        }

        db.SaveChanges();
        transaction.Commit();

        var lineCount = hasOrders.Sum(o => o.Lines.Count);
        logger.LogInformation("Seeded OrdersDbContext: {OrderCount} orders, {LineCount} lines", hasOrders.Count, lineCount);
    }

    private void SyncLines(Models.Order order, Models.Order seed) {
        var existingLines = order.Lines.ToDictionary(l => l.Id, l => l);
        var desiredIds = new HashSet<Guid>(seed.Lines.Select(l => l.Id));

        // Remove lines not in seedOrder
        foreach (var kvp in existingLines) {
            if (!desiredIds.Contains(kvp.Key)) {
                db.Remove(kvp.Value);
            }
        }

        // Upsert seedOrder lines
        foreach (var seedLine in seed.Lines) {
            if (existingLines.TryGetValue(seedLine.Id, out var existing)) {
                existing.ProductId = seedLine.ProductId;
                existing.Quantity = seedLine.Quantity;
                existing.UnitPrice = seedLine.UnitPrice;
            } else {
                order.Lines.Add(new OrderLine {
                    Id = seedLine.Id,
                    ProductId = seedLine.ProductId,
                    Quantity = seedLine.Quantity,
                    UnitPrice = seedLine.UnitPrice
                });
            }
        }
    }

    private static IReadOnlyList<Order> GetSeedOrders() {

        return [
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
                Lines = new List<Models.OrderLine> {
                    new() { Id = Id(5), ProductId = Id(1), Quantity = 2, UnitPrice = 12.99m },
                    new() { Id = Id(6), ProductId = Id(2), Quantity = 4, UnitPrice = 3.50m }
                }
            }
        ];
    }

    private static Guid Id(int id) => Guid.Parse($"10000000-0000-0000-0000-{id:D12}");
}