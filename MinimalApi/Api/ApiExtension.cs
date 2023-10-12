using Application.Commands;
using Application.Query;
using Application.Request;
using Domain.Extensions;
using MediatR;

namespace MinimalApi.Api;

public static class ApiExtension
{
    public static IEndpointRouteBuilder MapApi(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/todos/all/{authorId}", GetAllTodos);
        routes.MapGet("/todos/{id}", GetTodo);
        routes.MapPost("/todos/createAutor", CreateAutor);
        routes.MapPost("/todos", CreateTodo);
        routes.MapPost("/todos/createComment", CreateComment);
        routes.MapDelete("/todos/{id}", DeleteTodo);
        return routes;
    }
    
    private static async Task<IResult> GetAllTodos(Guid authorId, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllTodosQuery.Request(authorId), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetTodo(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTodoQuery.Request(id), cancellationToken);
        return result switch
        {
            {IsSuccess: true} => Results.Ok(result.Value),
            {IsFailed: true} => Results.Json(result.ToJsonErrors(), statusCode: StatusCodes.Status500InternalServerError),
            _ => Results.NoContent()
        };
    }

    private static async Task<IResult> CreateTodo(
        CreateTodoRequest todoRequest, 
        IMediator mediator, 
        CancellationToken cancellationToken)
    {
        var result = await mediator
            .Send(
                new CreateTodoCommand.Request(todoRequest.AuthorId, todoRequest.Title, todoRequest.Description), 
                cancellationToken);
        
        return result switch
        {
            { IsSuccess: true } => Results.Ok(result.Value),
            { IsFailed: true } => Results.Json(result.ToJsonErrors(), statusCode: StatusCodes.Status500InternalServerError),
            _ => Results.NoContent()
        };
    }

    private static async Task<IResult> DeleteTodo(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteTodoCommand.Request(id), cancellationToken);
        return result switch
        {
            {IsSuccess: true} => Results.Ok(),
            {IsFailed: true} => Results.Json(result.ToJsonErrors(), statusCode: StatusCodes.Status500InternalServerError),
            _ => Results.NoContent()
        };
    }

    private static async Task<IResult> CreateAutor(
        CreateAutorRequest autorRequest, 
        IMediator mediator, 
        CancellationToken cancellationToken)
    {
        var result = await mediator
            .Send(
                new CreateAuthorCommand.Request(
                    autorRequest.FirstName,
                    autorRequest.LastName,
                    autorRequest.Street,
                    autorRequest.City, 
                    autorRequest.Country,
                    autorRequest.ZipCode,
                    autorRequest.Email), 
                cancellationToken);
        
        return result switch
        {
            { IsSuccess: true } => Results.Ok(result.Value),
            { IsFailed: true } => Results.Json(result.ToJsonErrors(), statusCode: StatusCodes.Status500InternalServerError),
            _ => Results.NoContent()
        };
    }
    

    private static async Task<IResult> CreateComment(
        CreateCommentRequest commentRequest, 
        IMediator mediator, 
        CancellationToken cancellationToken)
    {
        var result = await mediator
            .Send(
                new CreateCommentCommand.Request(commentRequest.Nickname, commentRequest.TodoId, commentRequest.Text), 
                cancellationToken);
        
        return result switch
        {
            { IsSuccess: true } => Results.Ok(result.Value),
            { IsFailed: true } => Results.Json(result.ToJsonErrors(), statusCode: StatusCodes.Status500InternalServerError),
            _ => Results.NoContent()
        };
    }
}