using FluentValidation;
using Detailly.Application.Common.Exceptions;
using Detailly.Shared.Dtos;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Detailly.Infrastructure.Common;

/// <summary>
/// Global exception handler for unhandled exceptions.
/// Keeps the same ErrorDto format as the previous middleware.
/// </summary>
public sealed class DetaillyExceptionHandler(
    ILogger<DetaillyExceptionHandler> logger,
    IHostEnvironment env
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
    {
        // If the response has already started, let it bubble up
        if (ctx.Response.HasStarted)
        {
            logger.LogWarning(ex, "Response already started, letting the exception bubble.");
            return false;
        }

        var traceId = Activity.Current?.Id ?? ctx.TraceIdentifier;

        logger.LogError(ex,
            "Unhandled exception. Path: {Path}, Method: {Method}, TraceId: {TraceId}, User: {User}",
            ctx.Request.Path,
            ctx.Request.Method,
            traceId,
            ctx.User.Identity?.Name ?? "anonymous");


        ctx.Response.ContentType = "application/json";

        ctx.Response.StatusCode = ex switch
        {
            ValidationException => StatusCodes.Status400BadRequest,

            DetaillyUnauthorizedException => StatusCodes.Status401Unauthorized,
            
            DetaillyForbiddenException => StatusCodes.Status403Forbidden,

            DetaillyNotFoundException => StatusCodes.Status404NotFound,

            DetaillyGoneException => StatusCodes.Status410Gone,

            DetaillyConflictException
            or DetaillyBusinessRuleException => StatusCodes.Status409Conflict,

            DetaillyExternalServiceException => StatusCodes.Status502BadGateway,
            
            DetaillyTimeoutException => StatusCodes.Status504GatewayTimeout,

            _ => StatusCodes.Status500InternalServerError
        };

        var error = BuildErrorDto(ex, env.IsDevelopment(), traceId);

        await ctx.Response.WriteAsJsonAsync(error, cancellationToken: ct);
        return true; // prevents rethrowing the exception
    }

    private static ErrorDto BuildErrorDto(Exception ex, bool isDev, string traceId)
    {
        string code = "internal.error";
        string message = "An error occurred. Please try again.";

        switch (ex)
        {
            case DetaillyUnauthorizedException:
                code = "auth.unauthorized";
                message = ex.Message;
                break;

            case DetaillyForbiddenException:
                code = "auth.forbidden";
                message = ex.Message;
                break;

            case DetaillyNotFoundException:
                code = "entity.not_found";
                message = ex.Message;
                break;

            case DetaillyGoneException:
                code = "entity.gone";
                message = ex.Message;
                break;

            case DetaillyConflictException:
                code = "entity.conflict";
                message = ex.Message;
                break;

            case DetaillyBusinessRuleException:
                code = "business.rule";
                message = ex.Message;
                break;

            case ValidationException vex:
                code = "validation.error";
                message = string.Join("; ",
                    vex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                break;

            case DetaillyExternalServiceException:
                code = "external.service_error";
                message = ex.Message;
                break;

            case DetaillyTimeoutException:
                code = "external.timeout";
                message = ex.Message;
                break;

            default:
                code = "internal.error";
                message = "An unexpected error occurred. Please try again.";
                break;
        }

        return new ErrorDto
        {
            Code = code,
            Message = message,
            TraceId = traceId,
            Details = isDev ? ex.ToString() : null // stack trace only in Development environment
        };
    }
}
