using Domain.Common.Interfaces;

namespace Domain.Common;

public abstract class Entitie : IEntitie
{
    [NonSerialized] private readonly List<IEvent> _events = new();

    public long SId { get; init; }
    public Guid Id { get; init; }

    public IEnumerable<IEvent> UncommittedEvents
    {
        get
        {
            var result = _events.ToArray();
            _events.Clear();
            return result;
        }
    }

    public void AddEvent(IEvent evt)
        => _events.Add(evt);
}