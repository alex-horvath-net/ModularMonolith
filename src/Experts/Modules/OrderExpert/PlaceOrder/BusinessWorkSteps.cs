using Business.Domain;
using Business.Events;
using Business.Infrastructure;
using FluentValidation;
using OrderData = Business.Modules.OrderExpert.Common.Infrastructure.Data.Models.Order;
using OrderDomain = Business.Modules.OrderExpert.Common.Business.Domain.Order;

namespace Business.Modules.OrderExpert.PlaceOrder;

internal sealed class BusinessWorkSteps(
    IValidator<PlaceOrderRequest> validator,
    BusinessWorkSteps.IStoreInfrastructure store,
    IBusinessEventPublisher publisher) : BusinessWorkFlow.IBusinessWorkSteps {

    public async Task<IEnumerable<Issue>> Validate(PlaceOrderRequest request, CancellationToken token) {
        var infraModel = await validator.ValidateAsync(request, token);
        var domainModel = infraModel.Errors.Select(error => error.ToDomain());
        return domainModel;
    }

    public OrderDomain Create(PlaceOrderRequest request) {
        var order = new OrderDomain(request.CustomerId);

        foreach (var line in request.Lines)
            order.AddLine(line.ProductId, line.Quantity, line.UnitPrice);

        return order;
    }

    public Task Save(OrderDomain order, CancellationToken token) {
        var orderDataModel = order.ToDataModel();
        return store.Save(orderDataModel, token);
    }

    public Task Publish(OrderDomain order, CancellationToken token) {

        var orderPlacedBusinessEvent = new OrderPlaced(
            order.Id,
            order.CustomerId,
            order.Total);

        return publisher.Publish(orderPlacedBusinessEvent, token);
    }

    public interface IStoreInfrastructure {
        Task Save(OrderData order, CancellationToken token);
    }
}
