using Business.Domain;
using FluentValidation.Results;

namespace Business.Infrastructure;

public static class ValidatorWorkStepExtensions {
    public static Issue ToDomain(this ValidationFailure infra) => new(
        infra.PropertyName,
        infra.ErrorMessage);
}