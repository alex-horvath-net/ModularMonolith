using BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.Shared.Business.Domain;
using FluentValidation;

namespace BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.ValidatorBusinessWorkSteps;

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
