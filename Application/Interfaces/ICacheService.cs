namespace Application.Interfaces;

public interface ICacheService
{
    Task AddAsync<T>(string recordId, T data, CancellationToken cancellationToken);
    Task<T?> GetAsync<T>(string recordId, CancellationToken cancellationToken);
    Task<T?> GetAndRemoveAsync<T>(string recordId, CancellationToken cancellationToken);
}