using Application.Interfaces;
using Application.Options;
using Application.Response;
using Dapper;
using Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly IApplicationDbContext _applicationDb;
    private readonly ConnectionStringsOptions _connectionStrings;

    public AuthorRepository(IApplicationDbContext applicationDb, IOptions<ConnectionStringsOptions> connectionStrings)
    {
        _applicationDb = applicationDb;
        _connectionStrings = connectionStrings.Value;
    }

    public async Task<Guid> AddAsync(Author author, CancellationToken cancellationToken)
    {
        await _applicationDb.Authors.AddAsync(author, cancellationToken);
        return author.Id;
    }

    public async Task<Author?> FindAuthorAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        var result = await _applicationDb.Authors.FirstOrDefaultAsync(n => n.Id.Equals(authorId), cancellationToken);
        return result;
    }

    public async Task<Todo?> GetTodoAsync(Guid todoId, CancellationToken cancellationToken)
    {
        var result = await _applicationDb.Todos.FirstOrDefaultAsync(n => n.Id.Equals(todoId), cancellationToken);
        return result;
    }

    public async Task DeleteTodoAsync(Guid todoId, CancellationToken cancellationToken = default)
    {
        const string querySql = @"delete from dbo.Todos where Id = @id";
        await using var connection = new SqlConnection(_connectionStrings.DBConnection);
        await connection.OpenAsync(cancellationToken);
        await connection.QueryAsync(querySql, new { id = todoId });
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _applicationDb.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<TodoResponse?> GetTodoResponseAsync(Guid todoId, CancellationToken cancellationToken)
    {
        const string querySql = @"select dbo.Todos.Title, dbo.Todos.Description, dbo.Todos.AuthorId, dbo.Todos.Id as [Id], c.Id, c.Nickname, c.Text from dbo.Todos
                left join dbo.Comments C on dbo.Todos.Id = C.TodoId 
                where dbo.Todos.Id = @id";
        
        await using var connection = new SqlConnection(_connectionStrings.DBConnection);
        await connection.OpenAsync(cancellationToken);

        var findTodo = (await connection
                .QueryAsync<TodoResponse, CommentResponse, TodoResponse>(
                    querySql, 
                    (todo, comment) =>
                    {
                        if (comment is not null) todo.Comments.Add(comment);
                        return todo;
                    },
                    param: new { id = todoId },
                    splitOn: "Id"))
                .ToList();

        if (!findTodo.Any())
            return null;
        
        var result = findTodo.Aggregate((f, n) =>
        {
            n.Comments.AddRange(f.Comments);
            return n;
        });
        return result;
    }

    public async Task<IEnumerable<TodoResponse>> GetAllTodosResponseAsync(Guid authorId, CancellationToken cancellationToken)
    {
        const string querySql = @"select dbo.Todos.Title, dbo.Todos.Description, dbo.Todos.AuthorId, dbo.Todos.Id as [Id], c.Id, c.Nickname, c.Text from dbo.Todos
                left join dbo.Comments C on dbo.Todos.Id = C.TodoId 
                where dbo.Todos.AuthorId = @id";
        
        await using var connection = new SqlConnection(_connectionStrings.DBConnection);
        await connection.OpenAsync(cancellationToken);
        
        var findAllTodo = (await connection
                .QueryAsync<TodoResponse, CommentResponse, TodoResponse>(
                    querySql,
                    (todo, comment) =>
                    {
                        if (comment is not null) todo.Comments.Add(comment);
                        return todo;
                    },
                    param: new { id = authorId },
                    splitOn: "Id"))
            .GroupBy(n => n.Id)
            .ToList();
        
        if (!findAllTodo.Any())
            return new List<TodoResponse>();
        
        var result = findAllTodo.Select(n =>
            n.Aggregate((f, m) =>
            {
                m.Comments.AddRange(f.Comments);
                return m;
            }));
        return result;
    }
}