using Domain.Common.Interfaces;

namespace Domain.Events;

[Serializable]
public class AddressCreatedEvent : IEvent
{
    public Guid Id { get; set; } 
}