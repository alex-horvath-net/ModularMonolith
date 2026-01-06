using Business.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.Domain;
using FluentValidation;

namespace Business.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.WorkSteps;

public class ValidatorWorkStep(IValidator<CreateOrderRequest> validator) {
    public async Task<IEnumerable<Error>> Validate(CreateOrderRequest request, CancellationToken token) {
        var result = await validator.ValidateAsync(request, token);
        return result.Errors.Select(e => new Error(e.PropertyName, e.ErrorMessage));
    }

}

public sealed class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest> {
    public CreateOrderRequestValidator() {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Lines).NotEmpty().WithMessage("Must provide at least one order line");
        RuleForEach(x => x.Lines).ChildRules(lines => {
            lines.RuleFor(l => l.ProductId).NotEmpty();
            lines.RuleFor(l => l.Quantity).GreaterThan(0);
            lines.RuleFor(l => l.UnitPrice).GreaterThanOrEqualTo(0m);
        });
    }
}

