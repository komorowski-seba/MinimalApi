using Domain.Common.Interfaces;

namespace Domain.Events;

[Serializable]
public class AuthorCreateEvent : IEvent
{
    public Guid Id { get; set; } 
}