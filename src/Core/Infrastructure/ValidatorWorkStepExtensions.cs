using Core.Domain;
using FluentValidation.Results;

namespace Core.Infrastructure;

public static class ValidatorWorkStepExtensions {
    public static Issue ToDomain(this ValidationFailure infra) => new(
        infra.PropertyName,
        infra.ErrorMessage);
}