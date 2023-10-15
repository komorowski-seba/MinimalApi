> ### FluentResults

It seems that lot of error throwing exceptions in services
is a bit overused, and it is costly.

I think if possible, you can terminate a method with an error.
Exceptions are expensive, so sometimes whe can limit them.
There have been some lot of cool packages [OneOf](https://github.com/mcintyre321/OneOf) or my favorite [FluentResults](https://github.com/altmann/FluentResults)
which nicely wrap up our error return

```c#
public async Task<Result<TodoResponse>> Handle(Request request, CancellationToken cancellationToken)
{
    var result = await _authorRepository.GetTodoResponseAsync(request.Id, cancellationToken);
    return result ?? 
            Result.Fail<TodoResponse>($"I can`t find Todo with Id: {request.Id}");
}
```

```c#
private static async Task<IResult> GetTodo(Guid id, IMediator mediator, CancellationToken cancellationToken)
{
    var result = await mediator.Send(new GetTodoQuery.Request(id), cancellationToken);
    return result switch
    {
        {IsSuccess: true} => Results.Ok(result.Value),
        {IsFailed: true} => Results.Json(result.ToJsonErrors(), statusCode: StatusCodes.Status500InternalServerError),
        _ => Results.NoContent()
    };
}
```

It can be done in the Domain in a pretty good way too

```c#
public static Result<Author> Create(Guid authorId, Guid addressId, string firstName, string lastName, string? street, string? city, string? country, string? zipCode, string email)
{
    var erros = new List<Error>();
    if (string.IsNullOrWhiteSpace(firstName))
        erros.SetError<ArgumentNullException>(nameof(firstName));
    if (string.IsNullOrWhiteSpace(lastName))
        erros.SetError<ArgumentNullException>(nameof(lastName));

    if (erros.Count > 0)
        return erros;
    
    var result = new Author(authorId, firstName, lastName);
    var address = Address.Create(addressId, street ?? "", city ?? "", country ?? "", zipCode ?? "", email, result);
    return address.IsFailed ? 
        Result.Fail<Author>(address.Errors) : 
        result.AddAddres(address.Value);
}
```

```c#
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
```

Of course, in middleware we continue to handle all reported exceptions
But validation exceptions do not have to be handled in such an expensive way

<br />

> ### Domain Events at Bg Service

Domain events, propagated in ***BackgroundService*** allow their asynchronous execution of time-consuming events 
and they do not block the process of writing changes to the database as the "classic" way.
When we writing to the database changes in domain allows we retrieve events from the domain and send 
them to the ***Redis queue***

```c#
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    var events = ChangeTracker.Entries()
        .Select(n => n.Entity as Entitie)
        .SelectMany(n => n?.UncommittedEvents ?? Array.Empty<IEvent>())
        .ToList();

    foreach (var @event in events)
    {
        var message = new BgEventMessage(@event.GetType().Name, JsonConvert.SerializeObject(@event));
        await _subscriber.PublishAsync(_redisChannel, new RedisValue(message.ToString()));
    }
    
    var result = await base.SaveChangesAsync(cancellationToken);
    return result;
}
```

Bg Service then takes events off the **redis queue** and, use the mediator to propagates the event
to interested recipients

```c#
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
```
