using Application;
using Application.Options;
using Infrastructure;
using MinimalApi.Api;
using MinimalApi.Health;
using MinimalApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// options
builder.Services
    .AddOptions<ConnectionStringsOptions>()
    .BindConfiguration(ConnectionStringsOptions.Name);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHealthCheckServices(builder.Configuration);
builder.Services.AddTransient<ExceptionHandleMiddleware>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseInfrastructureConfiguration();
app.UseHttpsRedirection();
app.UseHealthCheckConfiguration();
app.UseMiddleware<ExceptionHandleMiddleware>();

app.MapApi();
app.Run();

public partial class Program { }