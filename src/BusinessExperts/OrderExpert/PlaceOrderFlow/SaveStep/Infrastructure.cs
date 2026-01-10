using Experts.OrderExpert.Shared.Infrastructure.Data;
using Experts.OrderExpert.Shared.Infrastructure.Data.Models;

namespace Experts.OrderExpert.PlaceOrderFlow.SaveStep;

public class Infrastructure(OrdersDbContext db) : Business.IInfrastructure {
    public async Task Save(Order order, CancellationToken token) {
        db.Add(order);
        await db.SaveChangesAsync(token);
    }
}

