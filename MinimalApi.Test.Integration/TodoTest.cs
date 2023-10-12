using Application.Request;
using Bogus;
using FluentAssertions;
using Xunit;

namespace MinimalApi.Test.Integration;

[Collection("ApiTest")]
public class TodoTest : AAppTest
{
    public TodoTest(AppTestFactory app) : base(app)
    { }

    [Fact]
    public async Task TodoTest_Add_New_Todo_Success()
    {
        var author = AppTestMock.CreateAuthorReguestMock();
        var authorId = await _httpClient.CreateAuthor(author);
        var todo = AppTestMock.CreateTodoRequestMock(authorId);
        
        // Act
        var todoIds = new List<Guid>
        {
            await _httpClient.CreateTodo(todo),
            await _httpClient.CreateTodo(todo)
        };
        
        // Assert
        var allTodos = await _httpClient.GetAllTodos(authorId);
        allTodos.Should()
            .NotBeEmpty()
            .And.HaveCount(todoIds.Count)
            .And.AllSatisfy(n => todoIds.Any(m => m.Equals(n.Id)).Should().BeTrue());
    }
    
    [Fact]
    public async Task TodoTest_Add_New_Todo_From_2_Users_Success()
    {
        var author = AppTestMock.CreateAuthorReguestMock();
        var authorIds = new List<Guid>
        {
            await _httpClient.CreateAuthor(author),
            await _httpClient.CreateAuthor(author)
        };
        var todos = new List<Faker<CreateTodoRequest>>
        {
            AppTestMock.CreateTodoRequestMock(authorIds[0]),
            AppTestMock.CreateTodoRequestMock(authorIds[1])
        };
        
        // Act
        var todosId = new List<Guid>
        {
            await _httpClient.CreateTodo(todos[0]),
            await _httpClient.CreateTodo(todos[1]),
            await _httpClient.CreateTodo(todos[1]),
            await _httpClient.CreateTodo(todos[1])
        };

        // Assert
        var allTodosFromFirst = await _httpClient.GetAllTodos(authorIds[0]);
        var allTodosFromSecond = await _httpClient.GetAllTodos(authorIds[1]);

        allTodosFromFirst.Should()
            .NotBeEmpty()
            .And.HaveCount(1);
        allTodosFromSecond.Should()
            .NotBeEmpty()
            .And.HaveCount(todosId.Count - 1);
    }

    [Fact]
    public async Task TodoTest_Add_New_Todo_And_Remove_Success()
    {
        var author = AppTestMock.CreateAuthorReguestMock();
        var authorId = await _httpClient.CreateAuthor(author);
        var todo = AppTestMock.CreateTodoRequestMock(authorId);
        
        // Act
        var todoIds = new List<Guid>
        {
            await _httpClient.CreateTodo(todo),
            await _httpClient.CreateTodo(todo)
        };
        var allTodosBeforeDeletion = await _httpClient.GetAllTodos(authorId);
        await _httpClient.DeleteTodo(todoIds[0]);

        // Assert
        var allTodos = await _httpClient.GetAllTodos(authorId);
        
        allTodos.Should()
            .NotBeEmpty()
            .And.HaveCount(todoIds.Count - 1);

        allTodosBeforeDeletion.Should()
            .NotBeEmpty()
            .And.HaveCount(todoIds.Count);
    }
}