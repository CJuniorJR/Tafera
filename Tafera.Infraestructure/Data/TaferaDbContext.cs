using Microsoft.EntityFrameworkCore;
using Tafera.Domain.Models.Todos;

namespace Tafera.Infraestructure.Data;

public class TaferaDbContext : DbContext
{
    public TaferaDbContext(DbContextOptions<TaferaDbContext> options)
        : base(options) { }

    public DbSet<TodoItem> TodoItem => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).ValueGeneratedNever();

            entity.Property(t => t.Title)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(t => t.Description)
                  .HasMaxLength(1000);

            entity.Property(t => t.Priority)
                  .HasConversion<string>();
        });
    }
}
