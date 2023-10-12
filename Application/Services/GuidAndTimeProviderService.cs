using Domain.Common.Interfaces;

namespace Application.Services;

public class GuidAndTimeProviderService : IGuidAndTimeProvider
{
    public Guid NewGuid => Guid.NewGuid();
    public DateTimeOffset NewTimeOffset => DateTimeOffset.Now;
}