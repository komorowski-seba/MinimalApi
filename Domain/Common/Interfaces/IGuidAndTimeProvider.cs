namespace Domain.Common.Interfaces;

public interface IGuidAndTimeProvider
{
    public Guid NewGuid { get; }
    public DateTimeOffset NewTimeOffset { get; }
}