using System.Net;
using System.Net.Mime;

namespace MinimalApi.Middleware;

public class ExceptionHandleMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandleMiddleware> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    public ExceptionHandleMiddleware(ILogger<ExceptionHandleMiddleware> logger, IHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            var result = new Dictionary<string, object>();
            result["Message"] = ex.Message;
            if (_hostEnvironment.IsDevelopment())
                result["StackTrace"] = ex.StackTrace ?? "[empty]";
            
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(result);
        }
    }
}