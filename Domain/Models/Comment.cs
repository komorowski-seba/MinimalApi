using Domain.Common;
using Domain.Events;
using Domain.Extensions;
using FluentResults;

namespace Domain.Models;

public class Comment : Entitie
{
    public Guid TodoId { get; init; }
    public string Nickname { get; init; }
    public string Text { get; init; }

    public virtual Todo? Todo { get; private set; }
    
    private Comment() // For EF
    { }

    private Comment(Guid id, string nickname, string text, Todo todo)
    {
        Nickname = nickname;
        Text = text;
        Todo = todo;
        TodoId = todo.Id;
        Id = id;
        
        AddEvent(new CommentCreateEvent { Id = Id });
    }

    public static Result<Comment> Create(Guid id, string nickname, string text, Todo todo)
    {
        var erros = new List<Error>();
        if (string.IsNullOrWhiteSpace(text))
            erros.SetError<ArgumentNullException>(nameof(text));
        if (string.IsNullOrWhiteSpace(nickname))
            erros.SetError<ArgumentNullException>(nameof(nickname));

        if (erros.Count > 0)
            return erros;

        var result = new Comment(id, nickname, text, todo);
        return result;
    }
}