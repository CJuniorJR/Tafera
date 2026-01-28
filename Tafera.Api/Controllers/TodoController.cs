using Microsoft.AspNetCore.Mvc;
using Tafera.Application.Todos;
using Tafera.Domain.Models.Todos;

namespace Tafera.Api.Controllers;

[ApiController]
[Route("api/todo")]
public class TodoController : Controller
{
    public TodoItemsMockList TodoItemsMock = new TodoItemsMockList();

    public List<TodoItem> TodoItems;

    public TodoController()
    {
        TodoItems = TodoItemsMock.GetTodoItemsMockList();
    }

    [HttpGet("hello-world")]
    public string GetHelloWorld()
    {
        return "Hello World";
    }

    [HttpGet()]
    public List<TodoItem> GetTodoItems() 
    {
        return TodoItems;
    }

    [HttpGet("{id:guid}")]
    public TodoItem GetTodoItem(Guid id)
    {
        return TodoItems.First(t => t.Id == id);
    }

    [HttpPost("create")]
    public TodoItem CreateTodoItem(string title, string description, Priority priority)
    {
        var todoItem = new TodoItem(title, description, priority);
        TodoItems.Add(todoItem);
        return todoItem;
    }
}
