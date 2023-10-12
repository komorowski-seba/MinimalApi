using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Events;
using Domain.Extensions;
using FluentResults;

namespace Domain.Models;

public class Author : Entitie, IAggregate
{
    private readonly List<Todo> _todos = new();

    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    
    public virtual Address? Address { get; private set; }
    public virtual IReadOnlyCollection<Todo> Todos => _todos;

    private Author() // For EF Core
    { }

    private Author(Guid id, string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        AddEvent(new AuthorCreateEvent { Id = Id });
    }

    public static Result<Author> Create(Guid authorId, Guid addressId, string firstName, string lastName, string? street, string? city, string? country, string? zipCode, string email)
    {
        var erros = new List<Error>();
        if (string.IsNullOrWhiteSpace(firstName))
            erros.SetError<ArgumentNullException>(nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            erros.SetError<ArgumentNullException>(nameof(lastName));

        if (erros.Count > 0)
            return erros;
        
        var result = new Author(authorId, firstName, lastName);
        var address = Address.Create(addressId, street ?? "", city ?? "", country ?? "", zipCode ?? "", email, result);
        return address.IsFailed ? 
            Result.Fail<Author>(address.Errors) : 
            result.AddAddres(address.Value);
    }

    public Result<Todo> AddTodo(Guid id, string title, string description)
    {
        var result = Todo.Create(id, title, description, this);
        if (result.IsFailed)
            return result;
        
        _todos.Add(result.Value);
        return result;
    }

    public Author AddAddres(Address address)
    {
        Address = address;
        return this;
    }
}