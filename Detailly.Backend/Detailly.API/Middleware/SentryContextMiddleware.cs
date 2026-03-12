using System.Security.Claims;

namespace Detailly.API.Middleware;

public sealed class SentryContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        SentrySdk.ConfigureScope(scope =>
        {
            var user = context.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var userId =
                    user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? user.FindFirst("sub")?.Value;

                var email = user.FindFirst(ClaimTypes.Email)?.Value;

                scope.User = new SentryUser
                {
                    Id = userId,
                    Email = email
                };

                scope.SetTag("is_admin", user.FindFirst("is_admin")?.Value ?? "false");
                scope.SetTag("is_manager", user.FindFirst("is_manager")?.Value ?? "false");
                scope.SetTag("is_employee", user.FindFirst("is_employee")?.Value ?? "false");
                scope.SetTag("is_fleet", user.FindFirst("is_fleet")?.Value ?? "false");
            }

            scope.SetTag("area", "api");
            scope.SetTag("request_path", context.Request.Path);
            scope.SetTag("http_method", context.Request.Method);
        });

        await next(context);
    }
}