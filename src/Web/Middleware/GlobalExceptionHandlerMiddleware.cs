using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Web.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An error occurred while processing your request.";

        switch (exception)
        {
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                break;

            case ValidationException validationEx:
                statusCode = HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                var validationResponse = new
                {
                    title = "Validation Error",
                    status = (int)statusCode,
                    errors = validationEx.Errors
                };

                return context.Response.WriteAsync(JsonSerializer.Serialize(validationResponse));

            case ArgumentNullException:
            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            title = exception.GetType().Name,
            status = (int)statusCode,
            detail = message,
            instance = context.Request.Path
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

