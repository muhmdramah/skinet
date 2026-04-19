namespace API.Filters
{
    using Humanizer; // Add the Humanizer NuGet package
    using Microsoft.AspNetCore.Routing;

    public class PluralizeParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            if (value == null)
                return null;

            // Pluralize the controller name (e.g., Product -> products)
            // categoty controller name is in singular form, so we need to pluralize it and make it lowercase
            // it becomes catrogies instead of categorys
            return value.ToString()?.Pluralize().ToLowerInvariant();
        }
    }

}
