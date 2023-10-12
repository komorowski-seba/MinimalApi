using Application.Response;
using Domain.Models;

namespace Application.Interfaces;

public interface IAuthorRepository
{
    Task<Guid> AddAsync(Author author, CancellationToken cancellationToken = default);
    Task<Author?> FindAuthorAsync(Guid authorId, CancellationToken cancellationToken = default);
    Task<Todo?> GetTodoAsync(Guid todoId, CancellationToken cancellationToken = default);
    Task DeleteTodoAsync(Guid todoId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    
    Task<TodoResponse?> GetTodoResponseAsync(Guid todoId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TodoResponse>> GetAllTodosResponseAsync(Guid authorId, CancellationToken cancellationToken = default);
}