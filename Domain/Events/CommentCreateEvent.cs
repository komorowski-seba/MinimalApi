using Domain.Common.Interfaces;

namespace Domain.Events;

public record CommentCreateEvent: IEvent
{
    public Guid Id { get; set; } 
}