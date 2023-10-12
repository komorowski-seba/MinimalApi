using System.Net.Http.Json;
using Application.Request;
using Application.Response;
using Newtonsoft.Json;

namespace MinimalApi.Test.Integration;

public static class AppTestExtension
{
    private const string CreateAuthorUrl = "/todos/createAutor";
    private const string TodoUrl = "/todos";
    private const string GetAllTodosUrl = "/todos/all/";
    private const string CreateCommentUrl = "/todos/createComment";
    
    public static async Task<Guid> CreateAuthor(this HttpClient client, CreateAutorRequest request)
    {
        var response = await client.PostAsJsonAsync(CreateAuthorUrl, request);
        var result = await response.Content.ReadAsStringAsync(CancellationToken.None);
        return Guid.Parse(result.Replace("\"", ""));
    }

    public static async Task<Guid> CreateTodo(this HttpClient client, CreateTodoRequest request)
    {
        var response = await client.PostAsJsonAsync(TodoUrl, request);
        var result = await response.Content.ReadAsStringAsync(CancellationToken.None);
        return Guid.Parse(result.Replace("\"", ""));
    }

    public static async Task<List<TodoResponse>> GetAllTodos(this HttpClient client, Guid authorId)
    {
        var response = await client.GetStringAsync($"{GetAllTodosUrl}{authorId}");
        var result = JsonConvert.DeserializeObject<List<TodoResponse>>(response);
        return result!;
    }

    public static async Task DeleteTodo(this HttpClient client, Guid todoId)
    {
        await client.DeleteAsync($"{TodoUrl}/{todoId}");
    }

    public static async Task<Guid> CreateComment(this HttpClient client, CreateCommentRequest request)
    {
        var response = await client.PostAsJsonAsync(CreateCommentUrl, request);
        var result = await response.Content.ReadAsStringAsync(CancellationToken.None);
        return Guid.Parse(result.Replace("\"", ""));
    }
}