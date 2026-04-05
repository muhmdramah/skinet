using API.Filters;
using API.Helpers.LinkGeneratorHelper;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 1. Add Versioned Api Explorer
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // formats as v1, v1.1, etc.
    options.SubstituteApiVersionInUrl = true; // THIS FIXES THE {version} IN SWAGGER
});

builder.Services.AddApiVersioning(options => {
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

// 2. Adjust SwaggerGen
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// all manages a database context
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAutoMapper(typeof(ProductMappingProfile).Assembly);

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(
        new PluralizeParameterTransformer()));
});

// Most professional APIs prefer lowercase URLs. While [controller] uses the class name (e.g., Products),
// you should ensure your routing configuration in Program.cs is set to lowercase:
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

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
}

app.UseAuthorization();

app.MapControllers();

app.Run();
