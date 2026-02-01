using Tafera.Domain.Models.Todos;

namespace Tafera.Api.Contracts.Todos;

public sealed record GetTodoItemResponse(Guid Id, string Title, string Description, bool IsCompleted, Priority Priority, DateTime CreatedAt, DateTime? UpdatedAt);
