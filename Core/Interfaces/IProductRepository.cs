using System;
using Core.Entities;

namespace Core.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{

    Task<IReadOnlyCollection<string>> GetBrandsAsync();
    Task<IReadOnlyCollection<string>> GetTypesAsync();

}
