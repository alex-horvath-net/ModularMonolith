using Core.Domain;
using Core.Events;
using Core.Infrastructure;
using FluentValidation;
using Orders.Core.Domain;
using Orders.Core.Infrastructure.Data.Models;

namespace Orders.PlaceOrder;

internal sealed class BusinessWorkSteps(
    IValidator<PlaceOrderRequest> validator,
    BusinessWorkSteps.IStoreInfrastructure store,
    IBusinessEventPublisher publisher) : BusinessWorkFlow.IBusinessWorkSteps {

    public async Task<IEnumerable<Issue>> Validate(PlaceOrderRequest request, CancellationToken token) {
        var infraModel = await validator.ValidateAsync(request, token);
        var domainModel = infraModel.Errors.Select(error => error.ToDomain());
        return domainModel;
    }

    public Order Create(PlaceOrderRequest request) {
        var order = new Order(request.CustomerId);

        foreach (var line in request.Lines)
            order.AddLine(line.ProductId, line.Quantity, line.UnitPrice);

        return order;
    }

    public Task Save(Order order, CancellationToken token) {
        var orderDataModel = order.ToDataModel();
        return store.Save(orderDataModel, token);
    }

    public Task Publish(Order order, CancellationToken token) {

        var orderPlacedBusinessEvent = new OrderPlaced(
            order.Id,
            order.CustomerId,
            order.Total);

        return publisher.Publish(orderPlacedBusinessEvent, token);
    }

    public interface IStoreInfrastructure {
        Task Save(Infrastructure.Data.Models.Order order, CancellationToken token);
    }
}
