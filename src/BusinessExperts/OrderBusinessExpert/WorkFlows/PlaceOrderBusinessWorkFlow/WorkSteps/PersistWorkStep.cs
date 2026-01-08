using Domain = Experts.OrderBusinessExpert.Shared.Business.Domain;
using Infra = Experts.OrderBusinessExpert.Shared.Infrastructure.Data;

namespace Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.WorkSteps;

public class PersistWorkStep(Infra.OrdersDbContext db) {
    public async Task Save(Domain.Order order, CancellationToken token) {
        var data = MapOrderDomainToInfra(order);
        db.Add(data);
        await db.SaveChangesAsync(token);
    }

    private Infra.Models.Order MapOrderDomainToInfra(Domain.Order order) => new() {
        Id = order.Id,
        CustomerId = order.CustomerId,
        Lines = order.Lines.Select(MapOrderLineDomainToInfra).ToList()
    };

    private Infra.Models.OrderLine MapOrderLineDomainToInfra(Domain.OrderLine line) => new() {
        ProductId = line.ProductId,
        UnitPrice = line.UnitPrice,
        Quantity = line.Quantity
    };
}
    