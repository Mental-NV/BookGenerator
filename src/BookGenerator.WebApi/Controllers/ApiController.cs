using BookGenerator.Domain.Primitives;
using BookGenerator.Domain.Primitives.Result;
using FluentValidation.Internal;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookGenerator.WebApi.Controllers
{
    public class ApiController : ControllerBase
    {
        protected readonly ISender sender;

        public ApiController(ISender sender)
        {
            this.sender = sender;
        }

        protected IActionResult HandleFailure(Result result) =>
            result switch
            {
                { IsSuccess: true } => throw new InvalidOperationException(),
                IValidationResult validationResult => 
                    BadRequest(
                        CreateProblemDetails(
                            "Validation Error",
                            StatusCodes.Status400BadRequest,
                            result.Error,
                            validationResult.Errors)),
                { Error.Code: string code } when code.EndsWith(".NotFound", StringComparison.OrdinalIgnoreCase) =>
                    NotFound(
                        CreateProblemDetails(
                            "Not found",
                            StatusCodes.Status404NotFound,
                            result.Error)),
                _ =>
                    BadRequest(
                        CreateProblemDetails(
                            "Bad Request",
                            StatusCodes.Status400BadRequest,
                            result.Error))
            };

        private static ProblemDetails CreateProblemDetails(
            string title, 
            int status,
            Error error,
            Error[] errors = null)
        {
            return new()
            {
                Title = title,
                Type = error.Code,
                Detail = error.Message,
                Status = status,
                Extensions = { { nameof(errors), errors } }
            };
        }
    }
}
