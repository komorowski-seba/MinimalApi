using Application.Exceptions;
using Application.Interfaces;
using Domain.Common.Interfaces;
using FluentResults;
using MediatR;

namespace Application.Commands;

public static class CreateTodoCommand
{
    public sealed record Request(Guid AuthorId, string Title, string Description) : IRequest<Result<Guid>>;
    
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
            var author = await _authorRepository.FindAuthorAsync(request.AuthorId, cancellationToken);
            if (author is null)
                return Result.Fail<Guid>(new ExceptionalError(new NotFindException(request.AuthorId.ToString())));
    
            var newTodo = author.AddTodo(_guidAndTimeProvider.NewGuid, request.Title, request.Description);
            if (newTodo.IsFailed)
                return Result.Fail<Guid>(newTodo.Errors);
                
            await _authorRepository.SaveChangesAsync(cancellationToken);
            return newTodo.Value.Id;
        }
    }
}