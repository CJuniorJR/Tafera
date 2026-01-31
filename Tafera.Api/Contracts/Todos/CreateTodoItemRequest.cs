using Tafera.Domain.Models.Todos;

namespace Tafera.Api.Contracts.Todos;

public sealed record CreateTodoItemRequest(string Title, string Description, Priority Priority);
