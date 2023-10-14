using Domain.Common.Interfaces;

namespace Domain.Events;

public sealed class AddressCreatedEvent : IEvent
{
    public Guid Id { get; set; } 
}