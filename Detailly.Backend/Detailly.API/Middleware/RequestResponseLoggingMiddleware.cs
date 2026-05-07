using System.Diagnostics;
using System.Text;

namespace Detailly.API.Middleware;

/// <summary>
/// Middleware that logs incoming HTTP requests and outgoing responses,
/// including duration, method, path, and status code.
/// </summary>
public sealed class RequestResponseLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestResponseLoggingMiddleware> logger)
{
    private const int SlowRequestThresholdMs = 400;
    private static readonly SemaphoreSlim _slowLogLock = new(1, 1);

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;

        string? requestBody = null;
        if (request.Method is "POST" or "PUT")
        {
            var isMultipart = (request.ContentType ?? string.Empty)
                .StartsWith("multipart/", StringComparison.OrdinalIgnoreCase);

            if (!isMultipart)
            {
                request.EnableBuffering();
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }
        }

        var originalBodyStream = context.Response.Body;
        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();

            var contentType = context.Response.ContentType ?? string.Empty;
            var isBinary = contentType.StartsWith("application/pdf", StringComparison.OrdinalIgnoreCase)
                        || contentType.StartsWith("application/octet-stream", StringComparison.OrdinalIgnoreCase)
                        || contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

            string responseText = string.Empty;
            if (!isBinary)
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);
            }

            var logMessage = new StringBuilder()
                .AppendLine("HTTP Request/Response Log:")
                .AppendLine($"  Path: {request.Path}")
                .AppendLine($"  Method: {request.Method}")
                .AppendLine($"  Status: {context.Response.StatusCode}")
                .AppendLine($"  Duration: {stopwatch.ElapsedMilliseconds} ms");

            if (!string.IsNullOrWhiteSpace(requestBody))
                logMessage.AppendLine($"  Request Body: {requestBody}");

            if (isBinary)
                logMessage.AppendLine($"  Response Body: [binary {contentType}, {responseBody.Length} bytes]");
            else if (!string.IsNullOrWhiteSpace(responseText))
                logMessage.AppendLine($"  Response Body: {responseText}");

            var elapsed = stopwatch.ElapsedMilliseconds;
            if (elapsed > SlowRequestThresholdMs)
            {
                logger.LogWarning("[SLOW REQUEST] {Path} took {Elapsed} ms", request.Path, elapsed);
                await _slowLogLock.WaitAsync();
                try
                {
                    await File.AppendAllTextAsync(
                        "Logs/slow-requests.log",
                        $"{DateTime.UtcNow:u} | {request.Path} | {elapsed} ms{Environment.NewLine}");
                }
                finally
                {
                    _slowLogLock.Release();
                }
            }

            logger.LogInformation("{Log}", logMessage.ToString());

            context.Response.Body = originalBodyStream;
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
