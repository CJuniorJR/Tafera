using Tafera.Domain.Common;

namespace Tafera.Domain.Models.Todos;

public class TodoItem : Entity<Guid>
{
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public bool IsCompleted { get; private set; }
    public Priority Priority { get; private set; }

    public TodoItem(string title, string description, Priority priority)
        : base(Guid.NewGuid())
    {
        Title = title;
        Description = description;
        Priority = priority;
        IsCompleted = false;
    }

    public void MarkAsCompleted()
    {
        IsCompleted = true;
        SetUpdated();
    }

    public void UpdateDetails(string title, string description, Priority priority)
    {
        Title = title;
        Description = description;
        Priority = priority;
        SetUpdated();
    }
}
