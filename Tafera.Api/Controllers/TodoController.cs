using Microsoft.AspNetCore.Mvc;
using Tafera.Application.Interfaces;
using Tafera.Application.Todos;
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

    public TodoItemsMockList TodoItemsMock = new TodoItemsMockList();

    [HttpGet("hello-world")]
    public string GetHelloWorld()
    {
        return "Hello World";
    }

    [HttpGet()]
    public List<TodoItem> GetTodoItems() 
    {
        return TodoItemsMock.GetTodoItemsMockList();
    }

    [HttpGet("{id:guid}")]
    public TodoItem GetTodoItem(Guid id)
    {
        return TodoItemsMock.GetTodoItemsMockList().First(t => t.Id == id);
    }

    [HttpPost("create")]
    public async Task<Guid> CreateTodoItem(string title, string description, Priority priority, CancellationToken cancellationToken)
    {
        var result = await _todoItemService.CreateTodoItemAsync(title, description, priority, cancellationToken);
        
        return result;
    }
}
