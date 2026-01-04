using Business.MemberApplicationUser.OrderBusinessExpert.CreateOrderWorkFlow.Infrastructure.Data.Models;

namespace Business.MemberApplicationUser.OrderBusinessExpert.CreateOrderWorkFlow;

public class CreateOrderWorkStep {
    public Order Create(CreateOrderRequest request) { 
       return Order.Create(request.CustomerId, request.Lines.Select(l => (l.ProductId, l.Quantity, l.UnitPrice)));
    }
}
