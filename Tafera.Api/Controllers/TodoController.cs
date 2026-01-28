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

    [HttpGet()]
    public async Task<IActionResult> GetTodoItems(CancellationToken cancellationToken)
    {
        var todoItems = await _todoItemService.GetAllTodoItemsAsync(cancellationToken);

        return Ok(todoItems);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTodoItem(Guid id, CancellationToken cancellationToken)
    {
        var todoItem = await _todoItemService.GetTodoItemByIdAsync(id, cancellationToken);

        if (todoItem is null)
            return NotFound();

        return Ok(todoItem);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateTodoItem(string title, string description, Priority priority, CancellationToken cancellationToken)
    {
        var result = await _todoItemService.CreateTodoItemAsync(title, description, priority, cancellationToken);

        return CreatedAtAction(nameof(GetTodoItem),
            new { result },
            null);
    }

    [HttpPut("update/{id:guid}")]
    public async Task<IActionResult> UpdateTodoItem(Guid id, string title, string description, Priority priority, CancellationToken cancellationToken)
    {
        var result = await _todoItemService.UpdateTodoItemAsync(id, title, description, priority, cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }
}
