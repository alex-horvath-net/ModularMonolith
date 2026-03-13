using FluentValidation;
using Orders.Core.Infrastructure.Data;
using Orders.Core.Infrastructure.Data.Models;

namespace Orders.PlaceOrder;

internal sealed class Infrastructure {
    internal sealed class Store(OrdersDbContext db) : BusinessWorkSteps.IStoreInfrastructure {
        public async Task Save(Order order, CancellationToken token) {
            db.Add(order);
            await db.SaveChangesAsync(token);
        }
    }

    internal sealed class Validator : AbstractValidator<PlaceOrderRequest> {
        public Validator() {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.Lines).NotEmpty().WithMessage("Must provide at least one order line");
            RuleForEach(x => x.Lines).ChildRules(lines => {
                lines.RuleFor(l => l.ProductId).NotEmpty();
                lines.RuleFor(l => l.Quantity).GreaterThan(0);
                lines.RuleFor(l => l.UnitPrice).GreaterThanOrEqualTo(0m);
            });
        }
    }
}
