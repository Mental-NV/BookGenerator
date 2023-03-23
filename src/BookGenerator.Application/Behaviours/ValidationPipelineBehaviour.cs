using BookGenerator.Domain.Primitives;
using BookGenerator.Domain.Primitives.Result;
using FluentValidation;
using FluentValidation.TestHelper;
using MediatR;

namespace BookGenerator.Application.Behaviours;

public class ValidationPipelineBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> validators;

    public ValidationPipelineBehaviour(IEnumerable<IValidator<TRequest>> validators) => 
        this.validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            await next();
        }

        Error[] errors = validators
            .Select(validator => validator.Validate(request))
            .SelectMany(validationResult =>  validationResult.Errors)
            .Where(validationFailure => validationFailure is not null)
            .Select(failure => new Error(
                failure.PropertyName,
                failure.ErrorMessage))
            .Distinct()
            .ToArray();

        if (errors.Any())
        {
            return CreateValidationResult<TResponse>(errors);
        }

        return await next();
    }

    private static TResult CreateValidationResult<TResult>(Error[] errors)
        where TResult : Result
    {
        if (typeof(TResult) == typeof(Result))
        {
            return ValidationResult.WithErrors(errors) as TResult;
        }

        object validationResult = typeof(ValidationResult<>)
            .GetGenericTypeDefinition()
            .MakeGenericType(typeof(TResult).GenericTypeArguments[0])
            .GetMethod(nameof(ValidationResult.WithErrors))
            .Invoke(null, new object[] { errors });

        return (TResult)validationResult;
    }
}
