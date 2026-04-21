using System.Text.Json;
using app.Application.Errors;
using app.Application.DTOs.Responses;

namespace app.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error");
            await WriteErrorResponse(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule conflict");
            await WriteErrorResponse(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "External service error");
            await WriteErrorResponse(context, StatusCodes.Status502BadGateway, AppErrors.External.AuthServiceCallFailed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorResponse(context, StatusCodes.Status500InternalServerError, AppErrors.System.InternalServerError);
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = ApiResponse<object>.Fail(message);
        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
