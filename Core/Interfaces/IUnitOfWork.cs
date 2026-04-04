using System;
using Core.Entities;

namespace Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    public IGenericRepository<Product> Products { get; }
    
    Task<int> CompleteAsync();
}
