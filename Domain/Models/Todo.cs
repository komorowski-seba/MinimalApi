using Domain.Common;
using Domain.Events;
using Domain.Extensions;
using FluentResults;

namespace Domain.Models;

public class Todo : Entitie
{
    private readonly List<Comment> _commets = new();

    public Guid AuthorId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public virtual IReadOnlyCollection<Comment> Comments => _commets; 
    
    public virtual Author? Author { get; private set; }
    
    private Todo() // For EF
    { }

    private Todo(Guid id) => // For delete EF
        Id = id;
    
    private Todo(Guid id, string title, string description, Author author)
    {
        Title = title;
        Description = description;
        AuthorId = author.Id;
        Author = author;
        Id = id;
        
        AddEvent(new TodoCreateEvent { Id = Id });
    }

    public static Result<Todo> Create(Guid id, string title, string description, Author author)
    {
        var erros = new List<Error>();
        if (string.IsNullOrWhiteSpace(title)) 
            erros.SetError<ArgumentNullException>(nameof(title));
        if (string.IsNullOrWhiteSpace(description)) 
            erros.SetError<ArgumentNullException>(nameof(description));

        if (erros.Count > 0)
            return erros;
        
        return new Todo(id, title, description, author);
    }

    public static Todo CreateEmptyForRemove(Guid id) => new Todo(id);

    public Result<Comment> AddComment(Guid id, string nickname, string text)
    {
        var result = Comment.Create(id, nickname, text, this);
        if (result.IsFailed)
            return Result.Fail<Comment>(result.Errors);
        
        _commets.Add(result.Value);
        return result.Value;
    }
}