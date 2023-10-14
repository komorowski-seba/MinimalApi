using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Events;

public sealed class AddNewAuthorEventHandler : INotificationHandler<AuthorCreateEvent>
{
    private readonly ILogger<AddNewAuthorEventHandler> _logger;

    public AddNewAuthorEventHandler(ILogger<AddNewAuthorEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(AuthorCreateEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(nameof(AuthorCreateEvent));
    }
}