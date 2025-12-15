using Microsoft.AspNetCore.Diagnostics;

namespace Detailly.API.Middleware;

public sealed class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IExceptionHandler _handler;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, IExceptionHandler handler)
    {
        _next = next;
        _handler = handler;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var handled = await _handler.TryHandleAsync(context, ex, CancellationToken.None);
            if (!handled)
            {
                throw; // fallback to default pipeline
            }
        }
    }
}