using FluentResults;

namespace Domain.Extensions;

public static class ResultErrorsExtension
{
    private const string NameParameter = "name";
    private const string MessageParameter = "message";

    public static IEnumerable<object> ToJsonErrors<T>(this Result<T> resultError)
    {
        var result = resultError
            .Errors
            .Select(n =>
            {
                var r = new Dictionary<string, object>();
                if (n.Metadata.TryGetValue(NameParameter, out var p))
                {
                    r[NameParameter] = p;
                    r[MessageParameter] = n.Message;
                    return r;
                }

                r[MessageParameter] = n.Message;
                return r;
            });
        return result;
    }

    public static void SetError<T>(this List<Error> errors, string paramName) where T: class, new()
    {
        var creator = typeof(T).GetConstructor(new[] { typeof(string) });
        var exc = creator?.Invoke(new object?[] { paramName }) as Exception;
        errors.Add(new ExceptionalError(exc ?? new Exception(paramName)).WithMetadata(NameParameter, paramName));
    }
}