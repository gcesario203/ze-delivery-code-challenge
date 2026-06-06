

using PartnerService.Api.Shared.DataObjects;
using PartnerService.Application.Shared.Exceptions;
using PartnerService.Application.Shared.ValueObjects;

namespace PartnerService.Api.Shared.Middlewares;

public class GlobalExceptionsMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<GlobalExceptionsMiddleware> _logger;

    public GlobalExceptionsMiddleware(RequestDelegate next, ILogger<GlobalExceptionsMiddleware> logger)
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
        catch (CommandValidationException ex)
        {
            _logger.LogError(ex, "An error occurred while processing the command.");

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<IEnumerable<ValidationFailure>>(ex.Errors, ex.Message, false);
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<string>(null, "An unexpected error occurred. Please try again later.", false);
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}