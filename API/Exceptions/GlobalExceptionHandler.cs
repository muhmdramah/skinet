using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly IHostEnvironment _hostEnvironment;

        public GlobalExceptionHandler(IProblemDetailsService problemDetailsService, IHostEnvironment hostEnvironment)
        {
            _problemDetailsService = problemDetailsService;
            _hostEnvironment = hostEnvironment;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            var problemDetailsContext = new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Type = exception.GetType().Name,
                    Status = httpContext.Response.StatusCode,
                    Title = "An error occurred!",
                    Detail = _hostEnvironment.IsDevelopment() ? exception.StackTrace : exception.Message.ToString()
                }
            };

            return await _problemDetailsService.TryWriteAsync(problemDetailsContext);
        }
    }
}
