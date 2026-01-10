using BusinessExperts.Shared.Business.Domain;
using FluentValidation.Results;

namespace BusinessExperts.Shared.Infrastructure;

public static class ValidatorWorkStepExtensions {
    public static Error ToDomain(this ValidationFailure infra) => new(
        infra.PropertyName,
        infra.ErrorMessage);
}