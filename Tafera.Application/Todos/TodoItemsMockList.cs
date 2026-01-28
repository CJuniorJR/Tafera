using Tafera.Domain.Models.Todos;

namespace Tafera.Application.Todos;

public class TodoItemsMockList
{
    public List<TodoItem> GetTodoItemsMockList()
    {
        return new List<TodoItem>
        {
            new TodoItem(
                "Estudar fundamentos de .NET",
                "Revisar Program.cs, DI básica e ciclo de vida",
                Priority.High
            ),

            new TodoItem(
                "Criar endpoint de criação de tarefas",
                "Implementar POST /todos no controller",
                Priority.High
            ),

            new TodoItem(
                "Criar endpoint de listagem",
                "Implementar GET /todos",
                Priority.Medium
            ),

            new TodoItem(
                "Refatorar nomes de classes",
                "Garantir que nomes reflitam bem o domínio",
                Priority.Low
            ),

            new TodoItem(
                "Adicionar endpoint de conclusão",
                "Implementar PUT /todos/{id}/complete",
                Priority.Medium
            ),

            new TodoItem(
                "Testar fluxo completo no Swagger",
                "Criar, listar e concluir tarefas",
                Priority.High
            ),

            new TodoItem(
                "Configurar SQLite",
                "Adicionar persistência com EF Core",
                Priority.Medium
            ),

            new TodoItem(
                "Criar README do projeto",
                "Explicar objetivo, stack e como rodar",
                Priority.Low
            ),

            new TodoItem(
                "Revisar domínio",
                "Validar se métodos do TodoItem fazem sentido",
                Priority.Medium
            ),

            new TodoItem(
                "Commitar versão estável",
                "Criar tag v0.1.0",
                Priority.High
            )
        };
    }
}
