using System;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        IQueryable<T> query = _context.Set<T>();

        return await query.ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await SaveAsync();
    }

    public T Update(T entity)
    {
        _context.Set<T>().Update(entity);

        return entity;
    }
    public T Delete(T entity)
    {
        _context.Set<T>().Remove(entity);

        return entity;
    }

    public bool IsExists(int entityId)
    {
        var exists = _context.Set<T>().Find(entityId);

        if (exists is not null)
            return true;

        return false;
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
