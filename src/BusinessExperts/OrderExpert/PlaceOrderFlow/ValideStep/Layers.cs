using Experts.OrderExpert.PlaceOrderFlow.Shared.Business.Domain;
using Experts.Shared.Business.Domain;
using Experts.Shared.Infrastructure;
using FluentValidation;

namespace Experts.OrderExpert.PlaceOrderFlow.ValideStep;

public class Business(Business.IAdapter adapter) {
    public Task<IEnumerable<Error>> Run(CreateOrderRequest request, CancellationToken token) =>
        adapter.Run(request, token);

    public interface IAdapter {
        Task<IEnumerable<Error>> Run(CreateOrderRequest request, CancellationToken token);
    }
}

public class Adapter(IValidator<CreateOrderRequest> validator) : Business.IAdapter {
    public async Task<IEnumerable<Error>> Run(CreateOrderRequest request, CancellationToken token) {
        var infraResult = await validator.ValidateAsync(request, token);
        var domainErrors = infraResult.Errors.Select(error => error.ToDomain());
        return domainErrors;
    }
}

public sealed class Infrastructure : AbstractValidator<CreateOrderRequest> {
    public Infrastructure() {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Lines).NotEmpty().WithMessage("Must provide at least one order line");
        RuleForEach(x => x.Lines).ChildRules(lines => {
            lines.RuleFor(l => l.ProductId).NotEmpty();
            lines.RuleFor(l => l.Quantity).GreaterThan(0);
            lines.RuleFor(l => l.UnitPrice).GreaterThanOrEqualTo(0m);
        });
    }
}