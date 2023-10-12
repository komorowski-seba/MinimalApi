using Domain.Events;
using MediatR;

namespace Application.Events;

public class AddNewTodoEventHandler : INotificationHandler<TodoCreateEvent>
{
    public async Task Handle(TodoCreateEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"AddNewTodoEvent ---- @@@@@ >>> hop: {notification.Id}");
    }
}