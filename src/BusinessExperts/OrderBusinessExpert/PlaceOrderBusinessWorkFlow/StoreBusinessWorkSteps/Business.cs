using Experts.OrderBusinessExpert.Shared.Business.Domain;

namespace Experts.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.StoreBusinessWorkSteps;

public class Business(Business.IInfrastructure infrastructure) {
    public async Task Save(Order domainOrder, CancellationToken token) {
        var infraOrder = domainOrder.ToInfra();
        await infrastructure.Save(infraOrder, token);
    }

    public interface IInfrastructure {
        Task Save(OrderBusinessExpert.Shared.Infrastructure.Data.Models.Order order, CancellationToken token);
    }
}

