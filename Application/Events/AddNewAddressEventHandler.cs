using Domain.Events;
using MediatR;

namespace Application.Events;

public class AddNewAddressEventHandler : INotificationHandler<AddressCreatedEvent>
{
    public async Task Handle(AddressCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"AddNewAddressEvent >>>>> ##### address !!!");
    }
}