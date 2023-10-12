using Domain.Events;
using MediatR;

namespace Application.Events;

public class AddNewCommentEventHandler : INotificationHandler<CommentCreateEvent>
{
    public async Task Handle(CommentCreateEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"AddNewCommentEvent ---- @@@@@ >>> hop: {notification.Id}");
    }
}