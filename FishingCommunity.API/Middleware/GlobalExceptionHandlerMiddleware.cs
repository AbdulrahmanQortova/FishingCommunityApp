using System.Net;
using System.Text.Json;
using FishingCommunity.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path
        };

        switch (exception)
        {
            case ValidationException validationException:
                problemDetails.Title = "Validation error occurred.";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Extensions["errors"] = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                _logger.LogWarning(exception, "Validation error on {Path}", context.Request.Path);
                break;

            case NotFoundException notFoundException:
                problemDetails.Title = "Resource not found.";
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Detail = notFoundException.Message;
                _logger.LogWarning(exception, "Not found on {Path}", context.Request.Path);
                break;

            case BusinessRuleValidationException businessRuleException:
                problemDetails.Title = "Business rule violation.";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Detail = businessRuleException.Message;
                _logger.LogWarning(exception, "Business rule violation on {Path}", context.Request.Path);
                break;

            case UnauthorizedAccessException:
                problemDetails.Title = "Unauthorized.";
                problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                _logger.LogWarning(exception, "Unauthorized access on {Path}", context.Request.Path);
                break;

            default:
                problemDetails.Title = "An unexpected error occurred.";
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                // Never leak internal exception details/stack traces in production responses.
                problemDetails.Detail = _environment.IsDevelopment()
                    ? exception.ToString()
                    : "An internal server error occurred. Please try again later.";
                _logger.LogError(exception, "Unhandled exception on {Path}", context.Request.Path);
                break;
        }

        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}