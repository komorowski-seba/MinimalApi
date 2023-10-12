using Application.Job;
using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Models;
using Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ISubscriber _subscriber;
    private readonly RedisChannel _redisChannel = new(BgDomainEventsService.DomainEventChannelName, RedisChannel.PatternMode.Literal);
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConnectionMultiplexer connectionMultiplexer) : base(options)
    {
        _subscriber = connectionMultiplexer.GetSubscriber();
    }

    public DbSet<Todo> Todos => Set<Todo>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Address> Addresses => Set<Address>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Author>().Configure();
        builder.Entity<Address>().Configure();
        builder.Entity<Comment>().Configure();
        builder.Entity<Todo>().Configure();
    }
    
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
}