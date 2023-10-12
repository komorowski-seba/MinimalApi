using Application.Exceptions;
using Application.Interfaces;
using Domain.Common.Interfaces;
using FluentResults;
using MediatR;

namespace Application.Commands;

public static class CreateCommentCommand
{
    public sealed record Request(string Nickname, Guid TodoId, string Text) : IRequest<Result<Guid>>;
    
    public sealed class Handler : IRequestHandler<Request, Result<Guid>>
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IGuidAndTimeProvider _guidAndTimeProvider;
    
        public Handler(IAuthorRepository authorRepository, IGuidAndTimeProvider guidAndTimeProvider)
        {
            _authorRepository = authorRepository;
            _guidAndTimeProvider = guidAndTimeProvider;
        }
    
        public async Task<Result<Guid>> Handle(Request request, CancellationToken cancellationToken)
        {
            var todo = await _authorRepository.GetTodoAsync(request.TodoId, cancellationToken);
            if (todo is null)
                return Result.Fail<Guid>(new ExceptionalError(new NotFindException(request.TodoId.ToString())));
    
            var newComment = todo.AddComment(_guidAndTimeProvider.NewGuid, request.Nickname, request.Text);
            if (newComment.IsFailed)
                return Result.Fail<Guid>(newComment.Errors);
            
            await _authorRepository.SaveChangesAsync(cancellationToken);
            return newComment.Value.Id;
        }
    }
}