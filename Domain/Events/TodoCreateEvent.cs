using Domain.Common.Interfaces;

namespace Domain.Events;

public sealed record TodoCreateEvent : IEvent
{
    public Guid Id { get; set; }
}