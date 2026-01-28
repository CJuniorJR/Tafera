using Microsoft.AspNetCore.Mvc;
using Tafera.Application.Interfaces;
using Tafera.Domain.Models.Todos;

namespace Tafera.Api.Controllers;

[ApiController]
[Route("api/todo")]
public class TodoController : Controller
{
    private readonly ITodoItemService _todoItemService;

    public TodoController(ITodoItemService todoItemService)
    {
        _todoItemService = todoItemService;
    }

    [HttpGet("hello-world")]
    public string GetHelloWorld()
    {
        return "Hello World";
    }

    [HttpGet()]
    public async Task<IEnumerable<TodoItem?>> GetTodoItems(CancellationToken cancellationToken)
    {
        return await _todoItemService.GetAllTodoItemsAsync(cancellationToken);
    }

    [HttpGet("{id:guid}")]
    public async Task<TodoItem?> GetTodoItem(Guid id, CancellationToken cancellationToken)
    {
        return await _todoItemService.GetTodoItemByIdAsync(id, cancellationToken);
    }

    [HttpPost("create")]
    public async Task<Guid> CreateTodoItem(string title, string description, Priority priority, CancellationToken cancellationToken)
    {
        var result = await _todoItemService.CreateTodoItemAsync(title, description, priority, cancellationToken);

        return result;
    }

    [HttpPut("update/{id:guid}")]
    public async Task<Guid> UpdateTodoItem(Guid id, string title, string description, Priority priority, CancellationToken cancellationToken)
    {
        var result = await _todoItemService.UpdateTodoItemAsync(id, title, description, priority, cancellationToken);

        return result;
    }
}
