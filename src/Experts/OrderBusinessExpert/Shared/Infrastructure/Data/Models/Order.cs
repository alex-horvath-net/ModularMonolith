namespace Experts.OrderBusinessExpert.Shared.Infrastructure.Data.Models;

public sealed class Order {
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public List<OrderLine> Lines { get; set; } = new();

    public Business.Domain.Order ToDomain() { 
        var orderDomain= new Business.Domain.Order(Id, CustomerId);
     
        foreach (var line in Lines)
            orderDomain.AddLine(line.ProductId, line.Quantity, line.UnitPrice);
        return orderDomain;
    }


    private Business.Domain.OrderLine ToInfraOrderLine(OrderLine domainOrderLine) => new(
        productId: domainOrderLine.ProductId,
        unitPrice: domainOrderLine.UnitPrice,
        quantity: domainOrderLine.Quantity
    );
}