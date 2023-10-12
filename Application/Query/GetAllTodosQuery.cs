using Application.Interfaces;
using Application.Response;
using MediatR;

namespace Application.Query;

public static class GetAllTodosQuery
{
    public sealed record Request(Guid AuthorId) : IRequest<IEnumerable<TodoResponse>>;
    
    public sealed class Handler : IRequestHandler<Request, IEnumerable<TodoResponse>>
    {
        private readonly IAuthorRepository _authorRepository;
    
        public Handler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }
    
        public async Task<IEnumerable<TodoResponse>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await _authorRepository.GetAllTodosResponseAsync(request.AuthorId, cancellationToken);
            return result.ToList();
        }
    }
}