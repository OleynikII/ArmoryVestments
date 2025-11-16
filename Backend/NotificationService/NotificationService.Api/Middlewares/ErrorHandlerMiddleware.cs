namespace UserService.Api.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    } 
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "text/plain";
            var responseModel = "Произошло не обработанное исключение!";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;

            _logger.LogError(error.Message);
            
            var result = JsonSerializer.Serialize(responseModel);
            await response.WriteAsync(result);
        }
    }
}