using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Events;

public sealed class AddNewTodoEventHandler : INotificationHandler<TodoCreateEvent>
{
    private readonly ILogger<AddNewTodoEventHandler> _logger;

    public AddNewTodoEventHandler(ILogger<AddNewTodoEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TodoCreateEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(nameof(TodoCreateEvent));
    }
}