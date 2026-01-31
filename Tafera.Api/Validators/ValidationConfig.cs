using FluentValidation;
using FluentValidation.AspNetCore;
using Tafera.Api.Validators.Todos;

namespace Tafera.Api.Validators;

public static class ValidationConfig
{
    public static IServiceCollection AddValidations(
        this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        services.AddValidatorsFromAssemblyContaining<CreateTodoItemRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateTodoItemRequestValidator>();

        return services;
    }
}
