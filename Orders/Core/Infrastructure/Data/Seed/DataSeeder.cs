using Orders.Core.Infrastructure.Data;

namespace Orders.Core.Infrastructure.Data.Seed;

public sealed class DataSeeder(OrdersDbContext db, DataProvider data) {
    public void Seed() => SeedOrders();

    private void SeedOrders() {
        db.Orders.RemoveRange(db.Orders.ToList());
        db.SaveChanges();

        db.Orders.AddRange(data.GetSeedOrders());
        db.SaveChanges();
    }
}

