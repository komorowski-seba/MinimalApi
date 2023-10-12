namespace Application.Request;

public sealed class CreateCommentRequest
{
    public string Nickname { get; set; } = string.Empty;
    public Guid TodoId { get; set; }
    public string Text { get; set; } = string.Empty;
}