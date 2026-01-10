using Experts.OrderExpert.PlaceOrder.Shared.Business.Domain;
using Experts.Shared.Business.Domain;
using Experts.Shared.Infrastructure;
using FluentValidation;

namespace Experts.OrderExpert.PlaceOrder.Validate;

public class Business(IValidator<CreateOrderRequest> validator) {
    public async Task<IEnumerable<Error>> Run(CreateOrderRequest request, CancellationToken token) {
        var infraResult = await validator.ValidateAsync(request, token);
        var domainErrors = infraResult.Errors.Select(error => error.ToDomain());
        return domainErrors;
    }
}
