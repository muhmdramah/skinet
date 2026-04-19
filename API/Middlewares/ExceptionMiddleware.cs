using API.Errors;
using System.Text.Json;

namespace API.Middlewares
{
    public class ExceptionMiddleware(IHostEnvironment environment, RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, environment);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex, IHostEnvironment environment)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = environment.IsDevelopment()
                ? new ApiErrorResponse(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString() ?? "")
                : new ApiErrorResponse(context.Response.StatusCode, "Internal Server Error", "");

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }
}
