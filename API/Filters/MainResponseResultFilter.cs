using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters
{
    public class MainResponseResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value is not null)
            {
                var response = new
                {
                    Success = true,
                    Data = objectResult.Value
                };

                context.Result = new JsonResult(response)
                {
                    StatusCode = objectResult.StatusCode
                };
            }

            await next();
        }
    }
}