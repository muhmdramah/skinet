
using Core.Entities;

namespace Core.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<IReadOnlyCollection<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task AddAsync(T entity);
    T Update(T entity);
    T Delete(T entity);

    Task<IReadOnlyCollection<T>> GetPagedAsync(int page = 1, int pageSize = 10);
    Task<int> CountAsync();

    Task<int> SaveAsync();
}
