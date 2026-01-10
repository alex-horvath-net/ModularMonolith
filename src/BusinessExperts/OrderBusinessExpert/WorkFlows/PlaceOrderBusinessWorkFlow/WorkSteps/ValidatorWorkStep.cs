using BusinessExperts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;
using BusinessExperts.Shared.Business.Domain;
using FluentValidation;
using FluentValidation.Results;

namespace BusinessExperts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.WorkSteps;

public class ValidatorWorkStep(IValidator<CreateOrderRequest> validator) {
    public async Task<IEnumerable<Error>> Validate(CreateOrderRequest request, CancellationToken token) {
        var infraResult = await validator.ValidateAsync(request, token);
        var domainErrors = infraResult.Errors.Select(MapInfraToDomain);
        return domainErrors;
    }

    private Error MapInfraToDomain(ValidationFailure infra) => new(
        infra.PropertyName,
        infra.ErrorMessage);
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

