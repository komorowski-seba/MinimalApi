using Domain.Common.Interfaces;

namespace Domain.Events;

public record TodoCreateEvent : IEvent
{
    public Guid Id { get; set; }
}