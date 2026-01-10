using Data = BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.Shared.Infrastructure.Data;

namespace BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.StoreBusinessWorkSteps;

public class Business(Business.IInfrastructure infrastructure) {
    public async Task Save(BusinessWorkFlows.Shared.Business.Domain.Order domainOrder, CancellationToken token) {
        var infraOrder = domainOrder.ToInfra();
        await infrastructure.Save(infraOrder, token);
    }

    public interface IInfrastructure {
        Task Save(Data.Models.Order order, CancellationToken token);
    }
}

 