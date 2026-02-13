using Microsoft.Extensions.Logging;
using Tafera.Application.Interfaces;
using Tafera.Domain.Models.Todos;

namespace Tafera.Application.Todos;

public class TodoItemService : ITodoItemService
{
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<TodoItemService> _logger;

    public TodoItemService(ITodoRepository todoRepository, ILogger<TodoItemService> logger)
    {
        _todoRepository = todoRepository;
        _logger = logger;
    }

    public async Task<Guid> CreateTodoItemAsync(string title, string description, Priority priority, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{ClassName} | {Method} | Creating todo item: {title}", nameof(TodoItemService), nameof(CreateTodoItemAsync), title);

        var todoItem = new TodoItem(title, description, priority);

        await _todoRepository.AddAsync(todoItem, cancellationToken);

        _logger.LogInformation("{ClassName} | {Method} | Todo item: {title} Created", nameof(TodoItemService), nameof(CreateTodoItemAsync), title);

        return todoItem.Id;
    }

    public async Task<TodoItem?> GetTodoItemByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{ClassName} | {Method} | Getting todo item with Id: {id}", nameof(TodoItemService), nameof(GetTodoItemByIdAsync), id);

        return await _todoRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<TodoItem?>> GetAllTodoItemsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("{ClassName} | {Method} | Getting all todo items", nameof(TodoItemService), nameof(GetAllTodoItemsAsync));

        return await _todoRepository.GetAllAsync(cancellationToken);
    }

    public async Task<bool> UpdateTodoItemAsync(Guid id, string title, string description, Priority priority, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{ClassName} | {Method} | Updating todo item with Id: {id}", nameof(TodoItemService), nameof(GetTodoItemByIdAsync), id);

        var todoItem = await _todoRepository.GetByIdAsync(id, cancellationToken);

        if (todoItem == null)
            return false;

        todoItem.UpdateDetails(title, description, priority);

        await _todoRepository.UpdateAsync(todoItem, cancellationToken);

        _logger.LogInformation("{ClassName} | {Method} | Updated todo item with Id: {id}", nameof(TodoItemService), nameof(GetTodoItemByIdAsync), id);

        return true;
    }
}
