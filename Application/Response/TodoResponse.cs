namespace Application.Response;

public sealed class TodoResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public List<CommentResponse> Comments { get; set; } = new();
}