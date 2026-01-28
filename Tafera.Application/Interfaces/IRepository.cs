namespace Tafera.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task AddAsync(T type, CancellationToken cancellationToken);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(T type, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
