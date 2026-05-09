namespace app.Common.Responses;

public class ApiResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = "OK";
    public object? Data { get; init; }
    public object? Errors { get; init; }

    public static ApiResponse Ok(object? data, string message = "OK") =>
        new()
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = null
        };

    public static ApiResponse Fail(string message, object? errors = null) =>
        new()
        {
            Success = false,
            Message = message,
            Data = null,
            Errors = errors
        };
}
