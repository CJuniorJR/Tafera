using Tafera.Domain.Models.Todos;

namespace Tafera.Application.Interfaces;

public interface ITodoItemService
{
    Task<Guid> CreateTodoItemAsync(string title, string description, Priority priority, CancellationToken cancellationToken);
}
