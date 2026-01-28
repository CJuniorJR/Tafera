using Tafera.Application.Interfaces;
using Tafera.Application.Todos;
using Microsoft.Extensions.DependencyInjection;

namespace Tafera.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<ITodoItemService, TodoItemService>();

        return services;
    }
}
