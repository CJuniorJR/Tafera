using FluentValidation;
using Tafera.Api.Contracts.Todos;

namespace Tafera.Api.Validators.Todos;

public sealed class CreateTodoItemRequestValidator : AbstractValidator<CreateTodoItemRequest>
{
    public CreateTodoItemRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.Priority)
            .IsInEnum();
    }
}
