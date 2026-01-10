using Experts.OrderBusinessExpert.BusinessWorkFlows.Shared.Business.Domain;
using Experts.OrderBusinessExpert.BusinessWorkFlows.Shared.Infrastructure.Data.Models;
using Data = BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.Shared.Infrastructure.Data;

namespace Experts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.StoreBusinessWorkSteps;

public class Business(Business.IInfrastructure infrastructure) {
    public async Task Save(Order domainOrder, CancellationToken token) {
        var infraOrder = domainOrder.ToInfra();
        await infrastructure.Save(infraOrder, token);
    }

    public interface IInfrastructure {
        Task Save(BusinessWorkFlows.Shared.Infrastructure.Data.Models.Order order, CancellationToken token);
    }
}

 