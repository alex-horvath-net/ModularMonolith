namespace Experts.Order.PlaceOrder.StoreBusinessWorkSteps;

public class Business(Business.IInfrastructure infrastructure) {
    public async Task Save( Order.Shared.Business.Domain.Order domainOrder, CancellationToken token) {
        var infraOrder = domainOrder.ToInfra();
        await infrastructure.Save(infraOrder, token);
    }

    public interface IInfrastructure {
        Task Save(Order.Shared.Infrastructure.Data.Models.Order order, CancellationToken token);
    }
}

