using app.Common.Pagination;
using app.DTOs.Rooms;

namespace app.Repositories.Rooms;

public interface IRoomRepository
{
    Task<PagedResponse<RoomResponse>> GetAllAsync(RoomListQuery query, CancellationToken cancellationToken = default);
    Task<RoomResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<RoomResponse> CreateAsync(RoomRequest request, CancellationToken cancellationToken = default);
    Task<RoomResponse?> UpdateAsync(long id, RoomRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
