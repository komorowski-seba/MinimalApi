namespace Application.Response;

public sealed class CommentResponse
{
    public Guid Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}