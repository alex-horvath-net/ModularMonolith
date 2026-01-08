namespace Experts.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Domain;

public sealed class CreateOrderResponse {
    public Order Order { get; set; }
    public IEnumerable<Error> Errors { get; set; }
    public bool IsOrderPlaced { get; internal set; }
}
