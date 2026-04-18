using API.Exceptions;
using API.Filters;
using API.Helpers.LinkGeneratorHelper;
using API.Services_Registrations;
using Core.Entities.SeedData;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    // This ensures that if the client requests a media type that the API doesn't support, it will return a 406 Not Acceptable response.
    // This is important for APIs that want to strictly enforce content negotiation and ensure clients are aware of unsupported media types.
    // only we support application/json, application/xml
    options.ReturnHttpNotAcceptable = true;
    options.Filters.Add(typeof(MainResponseResultFilter));
    options.Filters.Add(typeof(GlobalExceptionFilter));
})
//.AddXmlSerializerFormatters() // adds support for XML input and output
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

#region API_Versioning_Configuration
// 1. Add Versioned Api Explorer
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // formats as v1, v1.1, etc.
    options.SubstituteApiVersionInUrl = true; // THIS FIXES THE {version} IN SWAGGER
});

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

// 2. Adjust SwaggerGen
builder.Services.AddSwaggerGen(c =>
{
    // this is for the XML comments to show up in Swagger UI 
    c.EnableAnnotations();
});

builder.Services.AddApiVersioning(options =>
{
    // Specify the default API version (usually 1.0)
    options.DefaultApiVersion = new ApiVersion(1, 0);

    // If the client doesn't provide a version, use the default
    options.AssumeDefaultVersionWhenUnspecified = true;

    // Advertise the supported versions in the response headers
    options.ReportApiVersions = true;

    // Configure how the version is read (UrlSegment is required for {version} in route)
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});
#endregion

#region Connection_String_Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
#endregion

#region Services_Configuration
// all manages a database context
builder.Services.AddApplicationServices();
#endregion

#region Mapping_Profiles_Configuration
builder.Services.AddAutoMapper(typeof(ProductMappingProfile).Assembly);
#endregion

#region Lowercase_URLs_Configuration
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(
        new PluralizeParameterTransformer()));
});

// Most professional APIs prefer lowercase URLs. While [controller] uses the class name (e.g., Products),
// you should ensure your routing configuration in Program.cs is set to lowercase:
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
#endregion

#region Link_Generator_Configuration
// 1. Allow access to the HTTP Context
builder.Services.AddHttpContextAccessor();

// 2. Bind the Core interface to the API implementation
builder.Services.AddTransient<ILinkGeneratorHelper, LinkGeneratorHelper>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/error-development");
}

// if production, we don't want to show the detailed error page,
// instead we want to show a custom error page that we will create in the future
if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/error");
}

app.UseAuthorization();

app.MapControllers();

#region Database_Migration_and_Seeding_At_Application_Startup
try
{
    using var scope = app.Services.CreateScope();
    var service = scope.ServiceProvider;

    var context = service.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
    await ApplicationDbContextSeed.SeedAsync(context);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    throw;
}
#endregion

app.Run();
