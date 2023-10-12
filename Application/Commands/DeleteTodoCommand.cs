using Application.Interfaces;
using FluentResults;
using MediatR;

namespace Application.Commands;

public static class DeleteTodoCommand
{
    public sealed record Request(Guid TodoId) : IRequest<Result<bool>>;

    public sealed class Handler : IRequestHandler<Request, Result<bool>>
    {
        private readonly IAuthorRepository _authorRepository;

        public Handler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<Result<bool>> Handle(Request request, CancellationToken cancellationToken)
        {
            await _authorRepository.DeleteTodoAsync(request.TodoId, cancellationToken);
            return true;
        }
    }
}