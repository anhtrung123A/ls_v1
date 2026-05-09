using app.Common.Pagination;
using app.DTOs.Tasks;

namespace app.Repositories.Tasks;

public interface ITaskRepository
{
    Task<PagedResponse<TaskResponse>> GetAllAsync(TaskListQuery query, CancellationToken cancellationToken = default);
    Task<PagedResponse<TaskResponse>> GetByLeadIdAsync(long leadId, TaskListQuery query, CancellationToken cancellationToken = default);
    Task<TaskResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskResponse?> UpdateAsync(long id, TaskRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
