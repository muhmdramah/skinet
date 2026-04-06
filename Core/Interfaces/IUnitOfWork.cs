using System;
using Core.Entities;

namespace Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Product> ProductsGeneric { get; }
    IProductRepository Products { get; }

    Task<int> CompleteAsync();
}
