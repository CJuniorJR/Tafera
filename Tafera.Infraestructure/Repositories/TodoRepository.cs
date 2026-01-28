using Microsoft.EntityFrameworkCore;
using Tafera.Application.Interfaces;
using Tafera.Domain.Models.Todos;
using Tafera.Infraestructure.Data;

namespace Tafera.Infraestructure.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly TaferaDbContext _context;

    public TodoRepository(TaferaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TodoItem item, CancellationToken cancellationToken)
    {
        await _context.TodoItem.AddAsync(item, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await _context.TodoItem.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product is null) return;

        _context.TodoItem.Remove(product);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.TodoItem
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(TodoItem item, CancellationToken cancellationToken)
    {
        _context.TodoItem.Update(item);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
