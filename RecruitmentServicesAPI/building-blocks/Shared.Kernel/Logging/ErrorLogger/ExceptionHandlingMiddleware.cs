using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CrudOperations;
using HttpStatusCodes = Microsoft.AspNetCore.Http.StatusCodes;

namespace Shared.Logging.ErrorLogger;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    IHostEnvironment environment,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception).ConfigureAwait(false);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            logger.LogError(exception, "An unhandled exception occurred after the response started.");
            throw exception;
        }

        var statusCode = MapStatusCode(exception);
        var controllerName = Convert.ToString(context.Request.RouteValues["controller"], CultureInfo.InvariantCulture);
        var actionName = Convert.ToString(context.Request.RouteValues["action"], CultureInfo.InvariantCulture);

        long? createdBy = null;
        var userIdRaw =
            context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? context.User?.Identity?.Name;
        if (long.TryParse(userIdRaw, out var userId) && userId > 0)
        {
            createdBy = userId;
        }

        var errorLogService = context.RequestServices.GetRequiredService<IApplicationErrorLogService>();
        var logEntry = ApplicationErrorLogService.CreateFromException(
            exception,
            controllerName,
            actionName,
            context.Request.Path.Value,
            context.Request.Method,
            createdBy);

        await errorLogService.LogAsync(logEntry, context.RequestAborted).ConfigureAwait(false);

        var responseModel = new Response
        {
            Status = false,
            Message = BuildClientMessage(exception, statusCode)
        };

        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(responseModel, JsonOptions),
            context.RequestAborted).ConfigureAwait(false);
    }

    private string BuildClientMessage(Exception exception, int statusCode)
    {
        if (statusCode != HttpStatusCodes.Status500InternalServerError)
        {
            return exception.Message;
        }

        if (environment.IsDevelopment())
        {
            return exception.Message;
        }

        return "An unexpected error occurred. Please try again later.";
    }

    private static int MapStatusCode(Exception exception) =>
        exception switch
        {
            KeyNotFoundException => HttpStatusCodes.Status404NotFound,
            UnauthorizedAccessException => HttpStatusCodes.Status401Unauthorized,
            InvalidOperationException => HttpStatusCodes.Status400BadRequest,
            ArgumentException => HttpStatusCodes.Status400BadRequest,
            _ when exception.GetType().Name == "RecruitmentPortalException"
                => HttpStatusCodes.Status400BadRequest,
            _ => HttpStatusCodes.Status500InternalServerError
        };
}
