using Microsoft.AspNetCore.Mvc;
using Tafera.Api.Contracts.Todos;
using Tafera.Application.Interfaces;
using Tafera.Domain.Models.Todos;

namespace Tafera.Api.Controllers;

[ApiController]
[Route("api/todos")]
public class TodosController : Controller
{
    private readonly ITodoItemService _todoItemService;

    public TodosController(ITodoItemService todoItemService)
    {
        _todoItemService = todoItemService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetTodoItems(CancellationToken cancellationToken)
    {
        var todoItems = await _todoItemService.GetAllTodoItemsAsync(cancellationToken);

        var todoItemResponse = new List<GetTodoItemResponse>();

        if (todoItems != null)
        {
            foreach (var item in todoItems)
            {
                todoItemResponse.Add(new GetTodoItemResponse(
                    item.Id,
                    item.Title,
                    item.Description,
                    item.IsCompleted,
                    item.Priority,
                    item.CreatedAt,
                    item.UpdatedAt));
            }
        }

        return Ok(todoItemResponse);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTodoItem(Guid id, CancellationToken cancellationToken)
    {
        var todoItem = await _todoItemService.GetTodoItemByIdAsync(id, cancellationToken);

        if (todoItem is null)
            return NotFound();

        var todoItemResponse = new GetTodoItemResponse(
            todoItem!.Id,
            todoItem.Title,
            todoItem.Description,
            todoItem.IsCompleted,
            todoItem.Priority,
            todoItem.CreatedAt,
            todoItem.UpdatedAt);

        return Ok(todoItemResponse);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateTodoItem([FromBody] CreateTodoItemRequest request, CancellationToken cancellationToken)
    {
        var id = await _todoItemService.CreateTodoItemAsync(request.Title, request.Description, request.Priority, cancellationToken);

        return CreatedAtAction(nameof(GetTodoItem),
            new { id },
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
