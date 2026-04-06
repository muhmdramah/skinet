using System;
using Core.Entities;

namespace Core.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{

    Task<IReadOnlyCollection<string>> GetBrandsAsync();
    Task<IReadOnlyCollection<string>> GetTypesAsync();
    Task<IReadOnlyCollection<Product>> GetProductsByBrandAsync(string brandName);
    Task<IReadOnlyCollection<Product>> GetProductsByTypeAsync(string typeName);
}
