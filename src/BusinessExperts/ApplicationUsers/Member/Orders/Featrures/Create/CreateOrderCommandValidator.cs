using FluentValidation;

namespace Business.ApplicationUsers.Member.Orders.Featrures.Create;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Lines).NotEmpty().WithMessage("Must provide at least one order line");
        RuleForEach(x => x.Lines).ChildRules(lines =>
        {
            lines.RuleFor(l => l.ProductId).NotEmpty();
            lines.RuleFor(l => l.Quantity).GreaterThan(0);
            lines.RuleFor(l => l.UnitPrice).GreaterThanOrEqualTo(0m);
        });
    }
}
