using Experts.OrderExpert.PlaceOrderFlow.Shared.Business.Domain;
using Experts.OrderExpert.Shared.Business.Domain;
using Experts.Shared.Business.Domain;
using Experts.Shared.Business.Events;
using Experts.Shared.Infrastructure;
using FluentValidation;
using Data = Experts.OrderExpert.Shared.Infrastructure.Data;

namespace Experts.OrderExpert.PlaceOrderFlow;

public class BusinessWorkSteps(
    IValidator<CreateOrderRequest> validator,
    BusinessWorkSteps.IStoreInfrastructure store,
    IBusinessEventPublisher publisher) : BusinessWorkFlow.IBusinessWorkSteps {

    public async Task<IEnumerable<Error>> Validate(CreateOrderRequest request, CancellationToken token) {
        var infraModel = await validator.ValidateAsync(request, token);
        var domainModel = infraModel.Errors.Select(error => error.ToDomain());
        return domainModel;
    }

    public Order Create(CreateOrderRequest request) {
        var order = new Order(request.CustomerId);

        foreach (var line in request.Lines) {
            order.AddLine(line.ProductId, line.Quantity, line.UnitPrice);
        }

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
        Task Save(Data.Models.Order order, CancellationToken token);
    }
}
