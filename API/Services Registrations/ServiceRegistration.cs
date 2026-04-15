using Core.Interfaces;
using Infrastructure.Repositories;

namespace API.Services_Registrations
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices
        (this IServiceCollection builder)
        {
            builder.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.AddScoped<IProductRepository, ProductRepository>();
            builder.AddScoped<IUnitOfWork, UnitOfWork>();
            return builder;
        }
    }
}
