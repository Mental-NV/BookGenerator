﻿using BookGenerator.Domain.Primitives.Result;

namespace BookGenerator.Domain.Primitives;

public sealed class ValidationResult<TValue> : Result<TValue>, IValidationResult
{
    public ValidationResult(Error[] errors)
        : base(default, false, IValidationResult.ValidationError) =>
        Errors = errors;

    public Error[] Errors { get; }

    public static ValidationResult<TValue> WithErrors(Error[] errors) => new(errors);
}
