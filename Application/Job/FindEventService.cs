using System.Text.Json;
using Application.Interfaces;
using Domain.Common.Interfaces;

namespace Application.Job;

public sealed class FindEventService : IFindEventService
{
    private readonly List<Type> _eventTypes = AppDomain
        .CurrentDomain
        .GetAssemblies()
        .First(n => n.Modules.Any(m => m.Name.Contains(nameof(Domain))))
        .GetTypes()
        .Where(n => 
            n.GetInterfaces().Any(m => m == typeof(IEvent)) &&
            n.Namespace!.Equals($"{nameof(Domain)}.{nameof(Domain.Events)}"))
        .ToList();
    
    public TEvent? FindEvent<TEvent>(string nameType, string serializeData) where TEvent : class, IEvent
    {
        var findType = _eventTypes.FirstOrDefault(n => n.Name.Equals(nameType));
        if (findType == null)
            return null;
        var result = JsonSerializer.Deserialize(serializeData, findType) as TEvent;
        return result;
    }
}