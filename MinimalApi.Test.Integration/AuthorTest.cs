using Application.Interfaces;
using Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MinimalApi.Test.Integration;

[Collection("ApiTest")]
public sealed class AuthorTest : AAppTest
{
    public AuthorTest(AppTestFactory app) : base(app)
    { }
    
    [Fact]
    public async Task AuthorTest_AddAndGet_Success()
    {
        var author = AppTestMock.CreateAuthorReguestMock();
        var authorRepository = Scope.ServiceProvider.GetRequiredService<IAuthorRepository>();
        
        // Act
        var authorId = await _httpClient.CreateAuthor(author);

        // Assert
        var findAuthor = await authorRepository!.FindAuthorAsync(authorId);
        findAuthor.Should()
            .NotBeNull()
            .And.Match<Author>(n => n.Id.Equals(authorId));
    }
}