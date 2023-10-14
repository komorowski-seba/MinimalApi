using Domain.Common.Interfaces;

namespace Domain.Events;

public sealed record CommentCreateEvent: IEvent
{
    public Guid Id { get; set; } 
}