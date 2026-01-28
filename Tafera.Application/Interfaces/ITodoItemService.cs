using Tafera.Domain.Models.Todos;

namespace Tafera.Application.Interfaces;

public interface ITodoItemService
{
    Task<Guid> CreateTodoItemAsync(string title, string description, Priority priority, CancellationToken cancellationToken);
    Task<TodoItem?> GetTodoItemByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<TodoItem?>> GetAllTodoItemsAsync(CancellationToken cancellationToken);
    Task<bool> UpdateTodoItemAsync(Guid id, string title, string description, Priority priority, CancellationToken cancellationToken);
}
