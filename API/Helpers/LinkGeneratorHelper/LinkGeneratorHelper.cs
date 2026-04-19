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
            var context = _httpContextAccessor.HttpContext;
            return new
            {
                self = _linkGenerator.GetUriByName(context,
                    endpointName: "GetById", // This must match the Name="" on your Controller endpoint
                    values: new { id = entityId }),

                create = _linkGenerator.GetUriByName(context,
                    endpointName: "Create",
                    values: new { }),

                update = _linkGenerator.GetUriByName(context,
                    endpointName: "Update",
                    values: new { id = entityId }),

                delete = _linkGenerator.GetUriByName(context,
                    endpointName: "Delete",
                    values: new { id = entityId }),
            };
        }
    }
}
