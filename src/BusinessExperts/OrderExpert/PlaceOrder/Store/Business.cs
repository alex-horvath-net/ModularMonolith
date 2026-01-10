using Experts.OrderExpert.Shared.Business.Domain;

namespace Experts.OrderExpert.PlaceOrder.Store;

public class Business(Business.IInfrastructure infrastructure) {
    public async Task Save(Order domainOrder, CancellationToken token) {
        var infraOrder = domainOrder.ToInfra();
        await infrastructure.Save(infraOrder, token);
    }

    public interface IInfrastructure {
        Task Save(OrderExpert.Shared.Infrastructure.Data.Models.Order order, CancellationToken token);
    }
}