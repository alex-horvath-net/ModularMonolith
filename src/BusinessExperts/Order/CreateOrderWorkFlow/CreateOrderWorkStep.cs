using ApplicationUsers.Member.BusinessExperts.OrderBusinessExpert.CreateOrderWorkFlow.Infrastructure.Data.Models;

namespace BusinessExperts.Order.CreateOrderWorkFlow;

public class CreateOrderWorkStep {
    public Order Create(CreateOrderRequest request) { 
       return Order.Create(request.CustomerId, request.Lines.Select(l => (l.ProductId, l.Quantity, l.UnitPrice)));
    }
}
