using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tafera.Application.Interfaces;
using Tafera.Infraestructure.Data;
using Tafera.Infraestructure.Repositories;

namespace Tafera.Infraestructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        services.AddDbContext<TaferaDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly(typeof(TaferaDbContext).Assembly.FullName));
        });

        services.AddScoped<ITodoRepository, TodoRepository>();

        return services;
    }
}
