using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Events;

public sealed class AddNewAddressEventHandler : INotificationHandler<AddressCreatedEvent>
{
    private readonly ILogger<AddNewAddressEventHandler> _logger;

    public AddNewAddressEventHandler(ILogger<AddNewAddressEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(AddressCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(nameof(AddressCreatedEvent));
    }
}