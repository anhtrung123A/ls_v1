namespace app.Application.DTOs.Responses;

public class PaginationQueryDto
{
    private const int DefaultPage = 1;
    private const int DefaultLimit = 20;
    private const int MaxLimit = 100;

    public int Page { get; set; } = DefaultPage;
    public int Limit { get; set; } = DefaultLimit;

    public int NormalizedPage => Page < 1 ? DefaultPage : Page;
    public int NormalizedLimit => Limit < 1 ? DefaultLimit : (Limit > MaxLimit ? MaxLimit : Limit);
    public int Offset => (NormalizedPage - 1) * NormalizedLimit;
}
