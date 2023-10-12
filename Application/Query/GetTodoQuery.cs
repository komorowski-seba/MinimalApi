using Application.Interfaces;
using Application.Response;
using FluentResults;
using MediatR;

namespace Application.Query;

public static class GetTodoQuery
{
    public sealed record Request(Guid Id) : IRequest<Result<TodoResponse>>;
    
    public sealed class Handler : IRequestHandler<Request, Result<TodoResponse>>
    {
        private readonly IAuthorRepository _authorRepository;
    
        public Handler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }
    
        public async Task<Result<TodoResponse>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await _authorRepository.GetTodoResponseAsync(request.Id, cancellationToken);
            return result ?? 
                   Result.Fail<TodoResponse>($"I can`t find Todo with Id: {request.Id}");
        }
    }
}