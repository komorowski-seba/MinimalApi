using Application.Interfaces;
using Domain.Common.Interfaces;
using Domain.Models;
using FluentResults;
using MediatR;

namespace Application.Commands;

public static class CreateAuthorCommand
{
    public sealed record Request(
        string FirstName,
        string LastName,
        string Street,
        string City,
        string Country,
        string ZipCode,
        string Email) : IRequest<Result<Guid>>;
    
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
            var newAuthor = Author.Create(
                _guidAndTimeProvider.NewGuid, 
                _guidAndTimeProvider.NewGuid, 
                request.FirstName, 
                request.LastName, 
                request.Street, 
                request.City, 
                request.Country, 
                request.ZipCode, 
                request.Email);
            if (newAuthor.IsFailed)
                return Result.Fail<Guid>(newAuthor.Errors);
            
            await _authorRepository.AddAsync(newAuthor.Value, cancellationToken);
            await _authorRepository.SaveChangesAsync(cancellationToken);
            return newAuthor.Value.Id;
        }
    }
}