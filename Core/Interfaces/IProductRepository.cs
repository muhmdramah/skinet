using System;
using Core.Entities;

namespace Core.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IReadOnlyCollection<string>> GetBrandsAsync();
    Task<IReadOnlyCollection<string>> GetTypesAsync();
    Task<IReadOnlyCollection<Product>> GetProductsByBrandAsync(string brandName);
    Task<IReadOnlyCollection<Product>> GetProductsByTypeAsync(string typeName);
    Task<IReadOnlyCollection<Product>> GetProductsByASpecificBrandAndTypeAsync(string brand, string type);
    Task<IReadOnlyCollection<Product>> GetProductsSortedByPriceAsync(string sort);
    Task<bool> IsExistByName(string productName);
}
