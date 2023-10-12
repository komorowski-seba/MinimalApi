using Domain.Events;
using MediatR;

namespace Application.Events;

public class AddNewAuthorEventHandler : INotificationHandler<AuthorCreateEvent>
{
    public async Task Handle(AuthorCreateEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"AddNewAuthorEvent ---- @@@@@ >>> hop: {notification.Id}");
    }
}