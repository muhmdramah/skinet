
namespace Core.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IReadOnlyCollection<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task AddAsync(T entity);
    T Update(T entity);
    T Delete(T entity);

    Task<int> SaveAsync();
    bool IsExists(int entityId);
}
