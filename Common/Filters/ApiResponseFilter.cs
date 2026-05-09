using app.Common.Responses;

namespace app.Common.Filters;

public class ApiResponseFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var result = await next(context);

        if (result is null)
        {
            return ApiResponse.Ok(null);
        }

        if (result is ApiResponse)
        {
            return result;
        }

        if (result is IResult)
        {
            return result;
        }

        return ApiResponse.Ok(result);
    }
}
