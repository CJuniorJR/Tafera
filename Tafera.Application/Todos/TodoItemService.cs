using Tafera.Application.Interfaces;
using Tafera.Domain.Models.Todos;

namespace Tafera.Application.Todos;

public class TodoItemService : ITodoItemService
{
    private readonly ITodoRepository _todoRepository;

    public TodoItemService(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Guid> CreateTodoItemAsync(string title, string description, Priority priority, CancellationToken cancellationToken)
    {
        var todoItem = new TodoItem(title, description, priority);
        await _todoRepository.AddAsync(todoItem, cancellationToken);

        return todoItem.Id;
    }

    public async Task<TodoItem?> GetTodoItemByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _todoRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<TodoItem?>> GetAllTodoItemsAsync(CancellationToken cancellationToken)
    {
        return await _todoRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Guid> UpdateTodoItemAsync(Guid id, string title, string description, Priority priority, CancellationToken cancellationToken)
    {
        var todoItem = await _todoRepository.GetByIdAsync(id, cancellationToken);

        if (todoItem == null)
        {
            throw new KeyNotFoundException($"Todo item with ID {id} not found.");
        }

        todoItem.UpdateDetails(title, description, priority);

        await _todoRepository.UpdateAsync(todoItem, cancellationToken);

        return todoItem.Id;
    }
}
