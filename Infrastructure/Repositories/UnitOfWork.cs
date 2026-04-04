using System;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IGenericRepository<Product> Products { get; private set; }

    public UnitOfWork(ApplicationDbContext context, IGenericRepository<Product> products)
    {
        _context = context;
        Products = new GenericRepository<Product>(context);
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
