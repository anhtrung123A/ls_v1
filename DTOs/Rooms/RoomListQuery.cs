using app.Common.Pagination;

namespace app.DTOs.Rooms;

public class RoomListQuery : PaginationQuery
{
    public string? Keyword { get; set; }
    public bool? IsActive { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
