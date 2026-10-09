using BankApi.Exceptions;

namespace BankApi.Middleware;

public class BusinessExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public BusinessExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessRuleException ex)
            when (!context.Response.HasStarted)
        {
            await Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,    
                title: "Business rule violation",
                detail: ex.Message
            ).ExecuteAsync(context);
        }
    }
}