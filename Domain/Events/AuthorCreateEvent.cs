using Domain.Common.Interfaces;

namespace Domain.Events;

public sealed class AuthorCreateEvent : IEvent
{
    public Guid Id { get; set; } 
}