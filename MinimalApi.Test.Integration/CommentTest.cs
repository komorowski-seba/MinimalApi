using FluentAssertions;
using Xunit;

namespace MinimalApi.Test.Integration;

[Collection("ApiTest")]
public class CommentTest : AAppTest
{
    public CommentTest(AppTestFactory app) : base(app)
    { }

    [Fact]
    public async Task CommentTest_Add_Comment_Success()
    {
        var author = AppTestMock.CreateAuthorReguestMock();
        var authorId = await _httpClient.CreateAuthor(author);
        var todo = AppTestMock.CreateTodoRequestMock(authorId);
        var todoId = await _httpClient.CreateTodo(todo);
        var comment = AppTestMock.CreateCommentRequestMock(todoId);
        
        // Act
        var commentId = await _httpClient.CreateComment(comment);
        
        // Assert
        var allTodos = await _httpClient.GetAllTodos(authorId);

        allTodos.Should()
            .NotBeEmpty()
            .And.HaveCount(1)
            .And.Satisfy(n => n.AuthorId.Equals(authorId))
            .And.Satisfy(n => n.Id.Equals(todoId))
            .And.Satisfy(n => n.Comments.Count == 1)
            .And.Satisfy(n => n.Comments.Any(m => m.Id.Equals(commentId)));
    }
}