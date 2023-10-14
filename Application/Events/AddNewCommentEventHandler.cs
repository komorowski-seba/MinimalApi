using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Events;

public sealed class AddNewCommentEventHandler : INotificationHandler<CommentCreateEvent>
{
    private readonly ILogger<AddNewCommentEventHandler> _logger;

    public AddNewCommentEventHandler(ILogger<AddNewCommentEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(CommentCreateEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(nameof(CommentCreateEvent));
    }
}