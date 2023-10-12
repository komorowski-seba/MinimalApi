namespace Application.Options;

public sealed class ConnectionStringsOptions
{
    public const string Name = "ConnectionStrings";
    public required string DBConnection { get; set; } = string.Empty;
    public required string Redis { get; set; } = string.Empty;
}