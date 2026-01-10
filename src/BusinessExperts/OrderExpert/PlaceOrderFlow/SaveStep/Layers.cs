using Experts.OrderExpert.Shared.Business.Domain;
using Data = Experts.OrderExpert.Shared.Infrastructure.Data;

namespace Experts.OrderExpert.PlaceOrderFlow.SaveStep;

public class Business(Business.IAdapter adapter) {
    public async Task Run(Order order, CancellationToken token) => 
        await adapter.Save(order, token);

    public interface IAdapter {
        Task Save(Order order, CancellationToken token);
    }
}

public class Adapter(Adapter.IInfrastructure infrastructure) : Business.IAdapter {
    public Task Save(Order order, CancellationToken token) { 
        var dataModel = order.ToDataModel();
        return infrastructure.Save(dataModel, token);
    }

    public interface IInfrastructure {
        Task Save(Data.Models.Order order, CancellationToken token);
    }
}

public class Infrastructure(Data.OrdersDbContext db) : Adapter.IInfrastructure {
    public async Task Save(Data.Models.Order order, CancellationToken token) {
        db.Add(order);
        await db.SaveChangesAsync(token);
    }
}