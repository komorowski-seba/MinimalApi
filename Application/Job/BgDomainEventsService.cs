using System.Collections.Concurrent;
using Application.Interfaces;
using Domain.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Application.Job;

public sealed class BgDomainEventsService : BackgroundService
{
    public const string DomainEventChannelName = "domain-event-channel";
    
    private readonly ILogger<BgDomainEventsService> _logger;
    private readonly IFindEventService _findEventService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public BgDomainEventsService(
        ILogger<BgDomainEventsService> logger, 
        IFindEventService findEventService, 
        IServiceProvider serviceProvider, 
        IConnectionMultiplexer connectionMultiplexer)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _connectionMultiplexer = connectionMultiplexer;
        _findEventService = findEventService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain events Bg service started");
        
        var time = new PeriodicTimer(TimeSpan.FromMilliseconds(500));
        var stack = new ConcurrentStack<string>();
        var subscriber = _connectionMultiplexer.GetSubscriber();
        await subscriber.SubscribeAsync(
            new RedisChannel(DomainEventChannelName, RedisChannel.PatternMode.Literal), 
            (channel, value) =>
            {
                if (!channel.Equals(DomainEventChannelName) || value.IsNullOrEmpty)
                    return;
                    
                stack.Push(value.ToString());
            });
        
        while (await time.WaitForNextTickAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!stack.TryPop(out var value) || BgEventMessage.TryJobMessage(value) is not { } evtMessage)
                    continue;
                
                await using var scope = _serviceProvider.CreateAsyncScope();
                var mediator = scope.ServiceProvider.GetService<IMediator>();
                if (mediator is null)
                    continue;
                
                var createMessage = _findEventService.FindEvent<IEvent>(evtMessage.EventType, evtMessage.EventMessage);
                if (createMessage is not null)
                    await mediator.Publish(createMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to execute DomainEventsBgService '{ExMessage}'", ex.Message);
            }
        }
        
        _logger.LogInformation("Domain events Bg service stoped");
    }
}