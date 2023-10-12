using Domain.Common.Interfaces;

namespace Application.Interfaces;

public interface IFindEventService
{
    TEvent? FindEvent<TEvent>(string nameType, string serializeData) where TEvent : class, IEvent;
}