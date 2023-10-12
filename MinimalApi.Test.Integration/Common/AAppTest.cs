using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MinimalApi.Test.Integration;

public abstract class AAppTest : IAsyncLifetime
{
    protected readonly AppTestFactory _app;
    protected readonly HttpClient _httpClient;
    private readonly Func<Task> _resetDb;

    protected IServiceScope Scope =>
        _app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    
    protected AAppTest(AppTestFactory app)
    {
        _app = app;
        _httpClient = app.HttpClient;
        _resetDb = app.ResetDbAsync;
    }

    public Task InitializeAsync() => 
        Task.CompletedTask;

    public async Task DisposeAsync() =>
        await _resetDb();
}