using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ErrorController : ControllerBase
    {
        public IActionResult Error(int code)
        {
            // RFC 7231, Section 6.6.1: https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1

            var response = new
            {
                type = "https://localhost:7221/error-development",
                title = "Internal Server Error",
                status = StatusCodes.Status500InternalServerError,
                detail = "An unexpected error occurred.",
                instance = HttpContext.Request.Path
            };

            return new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        [Route("error-development")]
        public IActionResult HandleErrorDevelopment([FromServices] IHostEnvironment environment)
        {
            if(environment.IsProduction())
            {
                return NotFound();
            }

            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()!.Error;

            var response = new
            {
                type = "https://localhost:7221/error-development",
                title = exception.Message ?? "Unhandled Exception",
                status = StatusCodes.Status500InternalServerError,
                detail = exception?.StackTrace,
                instance = HttpContext.Request.Path
            };

            return new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
