using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tafera.Api;
using Tafera.Api.Contracts.Todos;
using Tafera.Domain.Models.Todos;
using Tafera.Infraestructure.Data;

namespace Tafera.IntegrationTests;

public class TodoControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public TodoControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTodoItems_ShouldReturnOkWithEmptyList_WhenNoItemsExist()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaferaDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        // Act
        var response = await _client.GetAsync("/api/todos");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var todoItems = JsonSerializer.Deserialize<List<TodoItem>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.NotNull(todoItems);
        Assert.Empty(todoItems);
    }

    [Fact]
    public async Task GetTodoItems_ShouldReturnOkWithTodoItems_WhenItemsExist()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaferaDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var todo1 = new TodoItem("Test Title 1", "Test Description 1", Priority.High);
        var todo2 = new TodoItem("Test Title 2", "Test Description 2", Priority.Medium);
        dbContext.TodoItem.AddRange(todo1, todo2);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/todos");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var todoItems = JsonSerializer.Deserialize<List<TodoItem>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.NotNull(todoItems);
        Assert.Equal(2, todoItems.Count);
    }

    [Fact]
    public async Task GetTodoItem_ShouldReturnOk_WhenItemExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaferaDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var todo = new TodoItem("Test Title", "Test Description", Priority.High);
        dbContext.TodoItem.Add(todo);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/todos/{todo.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var todoItem = JsonSerializer.Deserialize<GetTodoItemResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        });
        Assert.NotNull(todoItem);
        Assert.Equal(todo.Id, todoItem.Id);
        Assert.Equal("Test Title", todoItem.Title);
        Assert.Equal("Test Description", todoItem.Description);
        Assert.Equal(Priority.High, todoItem.Priority);
    }

    [Fact]
    public async Task GetTodoItem_ShouldReturnNotFound_WhenItemDoesNotExist()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaferaDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/todos/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateTodoItem_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaferaDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var request = new CreateTodoItemRequest("New Todo", "New Description", Priority.Medium);

        // Act
        var response = await _client.PostAsJsonAsync("/api/todos/create", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var location = response.Headers.Location?.ToString();
        Assert.NotNull(location);
        Assert.Contains("/api/todos/", location);
    }

    [Fact]
    public async Task CreateTodoItem_ShouldCreateItemInDatabase_WhenRequestIsValid()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaferaDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var request = new CreateTodoItemRequest("New Todo", "New Description", Priority.High);

        // Act
        var response = await _client.PostAsJsonAsync("/api/todos/create", request);
        var location = response.Headers.Location?.ToString();
        var createdId = ExtractIdFromLocation(location);

        // Assert
        Assert.NotNull(createdId);
        var createdTodo = await dbContext.TodoItem.FindAsync(createdId);
        Assert.NotNull(createdTodo);
        Assert.Equal("New Todo", createdTodo.Title);
        Assert.Equal("New Description", createdTodo.Description);
        Assert.Equal(Priority.High, createdTodo.Priority);
        Assert.False(createdTodo.IsCompleted);
    }

    [Theory]
    [InlineData(Priority.Normal)]
    [InlineData(Priority.Low)]
    [InlineData(Priority.Medium)]
    [InlineData(Priority.High)]
    [InlineData(Priority.Top)]
    public async Task CreateTodoItem_ShouldAcceptAllPriorityValues(Priority priority)
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaferaDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var request = new CreateTodoItemRequest("Test Title", "Test Description", priority);

        // Act
        var response = await _client.PostAsJsonAsync("/api/todos/create", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodoItem_ShouldReturnNoContent_WhenItemExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaferaDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var todo = new TodoItem("Original Title", "Original Description", Priority.Normal);
        dbContext.TodoItem.Add(todo);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await _client.PutAsync(
            $"/api/todos/update/{todo.Id}?title=Updated%20Title&description=Updated%20Description&priority=High",
            null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    //[Fact]
    //public async Task UpdateTodoItem_ShouldUpdateItemInDatabase_WhenItemExists()
    //{
    //    // Arrange
    //    using var scope = _factory.Services.CreateScope();
    //    var dbContext = scope.ServiceProvider.GetRequiredService<TaferaDbContext>();
    //    dbContext.Database.EnsureDeleted();
    //    dbContext.Database.EnsureCreated();

    //    var todo = new TodoItem("Original Title", "Original Description", Priority.Normal);
    //    dbContext.TodoItem.Add(todo);
    //    await dbContext.SaveChangesAsync();

    //    // Act
    //    var response = await _client.PutAsync(
    //        $"/api/todos/update/{todo.Id}?title=Updated%20Title&description=Updated%20Description&priority=High",
    //        null);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
    //    // Verify the update in database - need to get fresh instance
    //    var updatedTodo = await dbContext.TodoItem.FindAsync(todo.Id);
    //    Assert.NotNull(updatedTodo);
    //    Assert.Equal("Updated Title", updatedTodo.Title);
    //    Assert.Equal("Updated Description", updatedTodo.Description);
    //    Assert.Equal(Priority.High, updatedTodo.Priority);
    //}

    [Fact]
    public async Task UpdateTodoItem_ShouldReturnNotFound_WhenItemDoesNotExist()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaferaDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.PutAsync(
            $"/api/todos/update/{nonExistentId}?title=Updated%20Title&description=Updated%20Description&priority=High",
            null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(Priority.Normal)]
    [InlineData(Priority.Low)]
    [InlineData(Priority.Medium)]
    [InlineData(Priority.High)]
    [InlineData(Priority.Top)]
    public async Task UpdateTodoItem_ShouldAcceptAllPriorityValues(Priority priority)
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaferaDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var todo = new TodoItem("Original Title", "Original Description", Priority.Normal);
        dbContext.TodoItem.Add(todo);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await _client.PutAsync(
            $"/api/todos/update/{todo.Id}?title=Updated%20Title&description=Updated%20Description&priority={priority}",
            null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodoItem_ShouldNotChangeIsCompleted_WhenUpdating()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaferaDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var todo = new TodoItem("Original Title", "Original Description", Priority.Normal);
        todo.MarkAsCompleted();
        dbContext.TodoItem.Add(todo);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await _client.PutAsync(
            $"/api/todos/update/{todo.Id}?title=Updated%20Title&description=Updated%20Description&priority=High",
            null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify IsCompleted is still true - need to get fresh instance
        var updatedTodo = await dbContext.TodoItem.FindAsync(todo.Id);
        Assert.NotNull(updatedTodo);
        Assert.True(updatedTodo.IsCompleted);
    }

    [Fact]
    public async Task CreateAndGet_ShouldWorkTogether()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaferaDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var request = new CreateTodoItemRequest("Integration Test", "Integration Description", Priority.High);

        // Act - Create
        var createResponse = await _client.PostAsJsonAsync("/api/todos/create", request);
        var location = createResponse.Headers.Location?.ToString();
        var createdId = ExtractIdFromLocation(location);

        // Act - Get
        var getResponse = await _client.GetAsync($"/api/todos/{createdId}");

        // Assert
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        
        var content = await getResponse.Content.ReadAsStringAsync();
        var todoItem = JsonSerializer.Deserialize<TodoItem>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.NotNull(todoItem);
        Assert.Equal("Integration Test", todoItem.Title);
        Assert.Equal("Integration Description", todoItem.Description);
        Assert.Equal(Priority.High, todoItem.Priority);
    }

    private static Guid? ExtractIdFromLocation(string? location)
    {
        if (string.IsNullOrEmpty(location))
            return null;

        var parts = location.Split('/');
        var idString = parts.LastOrDefault();
        if (Guid.TryParse(idString, out var id))
            return id;

        return null;
    }
}
