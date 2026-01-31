using Tafera.Domain.Models.Todos;

namespace Tafera.UnitTests
{
    public class TodoItemTests
    {
        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var title = "Test Title";
            var description = "Test Description";
            var priority = Priority.High;

            // Act
            var todoItem = new TodoItem(title, description, priority);

            // Assert
            Assert.Equal(title, todoItem.Title);
            Assert.Equal(description, todoItem.Description);
            Assert.Equal(priority, todoItem.Priority);
            Assert.False(todoItem.IsCompleted);
            Assert.NotEqual(Guid.Empty, todoItem.Id);
            Assert.True(todoItem.CreatedAt <= DateTime.UtcNow);
            Assert.Null(todoItem.UpdatedAt);
        }

        [Fact]
        public void Constructor_ShouldSetIsCompletedToFalse()
        {
            // Arrange & Act
            var todoItem = new TodoItem("Title", "Description", Priority.Normal);

            // Assert
            Assert.False(todoItem.IsCompleted);
        }

        [Fact]
        public void Constructor_ShouldGenerateNewGuid()
        {
            // Arrange & Act
            var todoItem1 = new TodoItem("Title 1", "Description 1", Priority.Normal);
            var todoItem2 = new TodoItem("Title 2", "Description 2", Priority.Normal);

            // Assert
            Assert.NotEqual(todoItem1.Id, todoItem2.Id);
        }

        [Fact]
        public void Constructor_ShouldSetCreatedAtToCurrentTime()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow;

            // Act
            var todoItem = new TodoItem("Title", "Description", Priority.Normal);
            var afterCreation = DateTime.UtcNow;

            // Assert
            Assert.True(todoItem.CreatedAt >= beforeCreation);
            Assert.True(todoItem.CreatedAt <= afterCreation);
        }

        [Theory]
        [InlineData(Priority.Normal)]
        [InlineData(Priority.Low)]
        [InlineData(Priority.Medium)]
        [InlineData(Priority.High)]
        [InlineData(Priority.Top)]
        public void Constructor_ShouldAcceptAllPriorityValues(Priority priority)
        {
            // Arrange & Act
            var todoItem = new TodoItem("Title", "Description", priority);

            // Assert
            Assert.Equal(priority, todoItem.Priority);
        }

        [Fact]
        public void MarkAsCompleted_ShouldSetIsCompletedToTrue()
        {
            // Arrange
            var todoItem = new TodoItem("Title", "Description", Priority.Normal);
            Assert.False(todoItem.IsCompleted);

            // Act
            todoItem.MarkAsCompleted();

            // Assert
            Assert.True(todoItem.IsCompleted);
        }

        [Fact]
        public void MarkAsCompleted_ShouldSetUpdatedAt()
        {
            // Arrange
            var todoItem = new TodoItem("Title", "Description", Priority.Normal);
            Assert.Null(todoItem.UpdatedAt);
            var beforeUpdate = DateTime.UtcNow;

            // Act
            todoItem.MarkAsCompleted();
            var afterUpdate = DateTime.UtcNow;

            // Assert
            Assert.NotNull(todoItem.UpdatedAt);
            Assert.True(todoItem.UpdatedAt >= beforeUpdate);
            Assert.True(todoItem.UpdatedAt <= afterUpdate);
        }

        [Fact]
        public void MarkAsCompleted_ShouldNotChangeOtherProperties()
        {
            // Arrange
            var title = "Original Title";
            var description = "Original Description";
            var priority = Priority.Medium;
            var todoItem = new TodoItem(title, description, priority);
            var originalId = todoItem.Id;
            var originalCreatedAt = todoItem.CreatedAt;

            // Act
            todoItem.MarkAsCompleted();

            // Assert
            Assert.Equal(title, todoItem.Title);
            Assert.Equal(description, todoItem.Description);
            Assert.Equal(priority, todoItem.Priority);
            Assert.Equal(originalId, todoItem.Id);
            Assert.Equal(originalCreatedAt, todoItem.CreatedAt);
        }

        [Fact]
        public void UpdateDetails_ShouldUpdateTitleDescriptionAndPriority()
        {
            // Arrange
            var todoItem = new TodoItem("Original Title", "Original Description", Priority.Normal);
            var newTitle = "New Title";
            var newDescription = "New Description";
            var newPriority = Priority.High;

            // Act
            todoItem.UpdateDetails(newTitle, newDescription, newPriority);

            // Assert
            Assert.Equal(newTitle, todoItem.Title);
            Assert.Equal(newDescription, todoItem.Description);
            Assert.Equal(newPriority, todoItem.Priority);
        }

        [Fact]
        public void UpdateDetails_ShouldSetUpdatedAt()
        {
            // Arrange
            var todoItem = new TodoItem("Title", "Description", Priority.Normal);
            Assert.Null(todoItem.UpdatedAt);
            var beforeUpdate = DateTime.UtcNow;

            // Act
            todoItem.UpdateDetails("New Title", "New Description", Priority.High);
            var afterUpdate = DateTime.UtcNow;

            // Assert
            Assert.NotNull(todoItem.UpdatedAt);
            Assert.True(todoItem.UpdatedAt >= beforeUpdate);
            Assert.True(todoItem.UpdatedAt <= afterUpdate);
        }

        [Fact]
        public void UpdateDetails_ShouldNotChangeIdAndCreatedAt()
        {
            // Arrange
            var todoItem = new TodoItem("Title", "Description", Priority.Normal);
            var originalId = todoItem.Id;
            var originalCreatedAt = todoItem.CreatedAt;

            // Act
            todoItem.UpdateDetails("New Title", "New Description", Priority.High);

            // Assert
            Assert.Equal(originalId, todoItem.Id);
            Assert.Equal(originalCreatedAt, todoItem.CreatedAt);
        }

        [Fact]
        public void UpdateDetails_ShouldNotChangeIsCompleted()
        {
            // Arrange
            var todoItem = new TodoItem("Title", "Description", Priority.Normal);
            todoItem.MarkAsCompleted();
            Assert.True(todoItem.IsCompleted);

            // Act
            todoItem.UpdateDetails("New Title", "New Description", Priority.High);

            // Assert
            Assert.True(todoItem.IsCompleted);
        }

        [Theory]
        [InlineData("", "Description", Priority.Normal)]
        [InlineData("Title", "", Priority.Normal)]
        [InlineData("Very Long Title That Exceeds Normal Length", "Description", Priority.High)]
        public void UpdateDetails_ShouldAcceptVariousInputValues(string title, string description, Priority priority)
        {
            // Arrange
            var todoItem = new TodoItem("Original Title", "Original Description", Priority.Normal);

            // Act
            todoItem.UpdateDetails(title, description, priority);

            // Assert
            Assert.Equal(title, todoItem.Title);
            Assert.Equal(description, todoItem.Description);
            Assert.Equal(priority, todoItem.Priority);
        }

        [Fact]
        public void UpdateDetails_ShouldUpdateUpdatedAtMultipleTimes()
        {
            // Arrange
            var todoItem = new TodoItem("Title", "Description", Priority.Normal);
            
            // Act
            todoItem.UpdateDetails("Title 1", "Description 1", Priority.Low);
            var firstUpdate = todoItem.UpdatedAt;
            
            // Wait a small amount to ensure different timestamps
            Thread.Sleep(10);
            
            todoItem.UpdateDetails("Title 2", "Description 2", Priority.Medium);
            var secondUpdate = todoItem.UpdatedAt;

            // Assert
            Assert.NotNull(firstUpdate);
            Assert.NotNull(secondUpdate);
            Assert.True(secondUpdate > firstUpdate);
        }
    }
}
