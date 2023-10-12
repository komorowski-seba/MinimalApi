namespace Domain.Common.Interfaces;

public interface IEntitie
{
    long SId { get; }
    Guid Id { get; }
    
    IEnumerable<IEvent> UncommittedEvents { get; }
    void AddEvent(IEvent evt);
}