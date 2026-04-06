using System;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IGenericRepository<Product> ProductsGeneric { get; private set; }

    public IProductRepository Products { get; private set; }

    public UnitOfWork(ApplicationDbContext context, IGenericRepository<Product> products)
    {
        _context = context;
        ProductsGeneric = new GenericRepository<Product>(context);
        Products = new ProductRepository(context);
    }

    public Task<int> CompleteAsync()
    {
        return _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
