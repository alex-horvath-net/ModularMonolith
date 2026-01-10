using Experts.Order.Shared.Infrastructure.Data;

namespace Experts.Order.PlaceOrder.StoreBusinessWorkSteps;

public class Infrastructure(OrdersDbContext db) : Business.IInfrastructure {
    public async Task Save(Order.Shared.Infrastructure.Data.Models.Order order, CancellationToken token) {
        db.Add(order);
        await db.SaveChangesAsync(token);
    }
}

