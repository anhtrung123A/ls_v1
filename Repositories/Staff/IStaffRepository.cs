using app.DTOs.Staff;

namespace app.Repositories.Staff;

public interface IStaffRepository
{
    Task<StaffResponse> CreateAsync(CreateStaffRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(long staffId, CancellationToken cancellationToken = default);
}
