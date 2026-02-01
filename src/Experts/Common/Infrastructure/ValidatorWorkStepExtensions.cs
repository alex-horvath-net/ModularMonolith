using Experts.Common.Business.Domain;
using FluentValidation.Results;

namespace Experts.Common.Infrastructure;

public static class ValidatorWorkStepExtensions {
    public static Issue ToDomain(this ValidationFailure infra) => new(
        infra.PropertyName,
        infra.ErrorMessage);
}