using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers.LinkGeneratorHelper
{
    public class LinkGeneratorHelper : ILinkGeneratorHelper
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LinkGeneratorHelper(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public object GenerateConfirmationUrl(int entityId)
        {
            var request = _httpContextAccessor.HttpContext?.Request;

            if (request == null)
                return string.Empty; // Handle background task scenarios where HTTP context is null

            // Generates an absolute URL (e.g., https://skinet.com/api/orders/5)
            return new
            {
                self = _linkGenerator.GetUriByName(_httpContextAccessor.HttpContext,
                    endpointName: "GetById", // This must match the Name="" on your Controller endpoint
                    values: new { id = entityId }),

                update = _linkGenerator.GetUriByName(_httpContextAccessor.HttpContext,
                    endpointName: "Update",
                    values: new { id = entityId }),

                delete = _linkGenerator.GetUriByName(_httpContextAccessor.HttpContext,
                    endpointName: "Delete",
                    values: new { id = entityId }),
            };
        }
    }
}
