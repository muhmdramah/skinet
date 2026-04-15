using System;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
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
    }

    public T Update(T entity)
    {
        //_context.Set<T>().Update(entity);
        _context.Set<T>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;

        return entity;
    }
    public T Delete(T entity)
    {
        _context.Set<T>().Remove(entity);

        return entity;
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Set<T>().CountAsync();
    }

    public async Task<IReadOnlyCollection<T>> GetPagedAsync(int page = 1, int pageSize = 10)
    {
        var query = await _context.Set<T>()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return query;
    }
}
