using System.Data.Common;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Respawn;
using TestContainers.Container.Abstractions.Hosting;
using TestContainers.Container.Database.Hosting;
using TestContainers.Container.Database.MsSql;
using Xunit;

namespace MinimalApi.Test.Integration;

public class AppTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static string UserName => "sa";
    private static string Password => "Abcd1234!";

    public HttpClient HttpClient { get; private set; } = default!;
    private MsSqlContainer _container { get; }
    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;

    public AppTestFactory() : base()
    {
        _container = new ContainerBuilder<MsSqlContainer>()
            .ConfigureDockerImageName("mcr.microsoft.com/mssql/server:2017-latest-ubuntu")
            .ConfigureDatabaseConfiguration("not-used", Password, "not-used")
            .ConfigureLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            })
            .Build();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var port = _container.GetMappedPort(_container.ExposedPorts[0]);
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            
            services.Remove(descriptor!);
            services.AddDbContext<ApplicationDbContext>((_, context) =>
            {
                SqlServerDbContextOptionsExtensions.UseSqlServer(context, GenerateConnectionString(port, UserName, Password));
            });
        });
        builder.ConfigureAppConfiguration(c =>
        {
            var port = _container.GetMappedPort(_container.ExposedPorts[0]);
            c.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DBConnection"] = GenerateConnectionString(port, UserName, Password)
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        HttpClient = CreateClient();
        
        _dbConnection = new SqlConnection(GenerateConnectionString(_container.GetMappedPort(_container.ExposedPorts[0]), UserName, Password));
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                SchemasToInclude = new[] { "dbo" }
            });
    }

    public async Task ResetDbAsync() =>
        await _respawner.ResetAsync(_dbConnection);
    
    public new async Task DisposeAsync() => 
        await _container.StopAsync();

    private static string GenerateConnectionString(int port, string userName, string password) =>
        $"Server=127.0.0.1,{port}; Database=IntegrationTestDbDocker; User Id={userName}; Password={password}; TrustServerCertificate=True";
}