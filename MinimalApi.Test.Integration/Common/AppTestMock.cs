using Application.Request;
using Bogus;

namespace MinimalApi.Test.Integration;

public static class AppTestMock
{
    public static Faker<CreateAutorRequest> CreateAuthorReguestMock()
    {
        var result = new Faker<CreateAutorRequest>()
            .RuleFor(n => n.City, s => s.Address.City())
            .RuleFor(n => n.Street, s => s.Address.StreetName())
            .RuleFor(n => n.Email, s => s.Person.Email)
            .RuleFor(n => n.FirstName, s => s.Person.FirstName)
            .RuleFor(n => n.LastName, s => s.Person.LastName);
        return result;
    }

    public static Faker<CreateTodoRequest> CreateTodoRequestMock(Guid authorId)
    {
        var result = new Faker<CreateTodoRequest>()
            .RuleFor(n => n.AuthorId, _ => authorId)
            .RuleFor(n => n.Title, s => s.Lorem.Lines(1))
            .RuleFor(n => n.Description, s => s.Lorem.Lines(2));
        return result;
    }

    public static Faker<CreateCommentRequest> CreateCommentRequestMock(Guid todoId)
    {
        var result = new Faker<CreateCommentRequest>()
            .RuleFor(n => n.TodoId, _ => todoId)
            .RuleFor(n => n.Nickname, s => s.Person.Avatar)
            .RuleFor(n => n.Text, s => s.Lorem.Text());
        return result;
    }
}