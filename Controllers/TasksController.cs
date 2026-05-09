using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.DTOs.Tasks;
using app.Repositories.Tasks;

namespace app.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskRepository _taskRepository;
    private readonly AppDbContext _dbContext;

    public TasksController(ITaskRepository taskRepository, AppDbContext dbContext)
    {
        _taskRepository = taskRepository;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IResult> List([FromQuery] TaskListQuery query, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);

        if (!context.CanManage)
        {
            query.CreatedBy = context.StaffId;
        }

        var items = await _taskRepository.GetAllAsync(query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(items, "Get tasks successfully."));
    }

    [HttpGet("lead/{lead_id:long}")]
    public async Task<IResult> ListByLeadId(long lead_id, [FromQuery] TaskListQuery query, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);

        if (!context.CanManage)
        {
            query.CreatedBy = context.StaffId;
        }

        var items = await _taskRepository.GetByLeadIdAsync(lead_id, query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(items, "Get tasks by lead successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IResult> Detail(long id, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);
        var result = await _taskRepository.GetByIdAsync(id, cancellationToken);
        if (result is null)
        {
            throw new KeyNotFoundException("Task not found.");
        }

        if (!context.CanManage && result.CreatedBy != context.StaffId && result.AssignedTo != context.StaffId)
        {
            throw new UnauthorizedAccessException("You can only view your own tasks.");
        }

        return Results.Ok(ApiResponse.Ok(result, "Get task successfully."));
    }

    [HttpPost]
    public async Task<IResult> Create([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);

        if (!context.CanManage)
        {
            if (!request.RelatedLeadId.HasValue)
            {
                throw new InvalidOperationException("related_lead_id is required for sales task creation.");
            }

            var isLeadOwner = await _dbContext.Leads
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.RelatedLeadId.Value && x.AssignedTo == context.StaffId, cancellationToken);

            if (!isLeadOwner)
            {
                throw new UnauthorizedAccessException("You can only create tasks for leads assigned to you.");
            }

            request.CreatedBy = context.StaffId;
        }

        var result = await _taskRepository.CreateAsync(request, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Create task successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IResult> Update(long id, [FromBody] TaskRequest request, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);

        if (!context.CanManage)
        {
            var existed = await _taskRepository.GetByIdAsync(id, cancellationToken);
            if (existed is null)
            {
                throw new KeyNotFoundException("Task not found.");
            }

            if (existed.CreatedBy != context.StaffId)
            {
                throw new UnauthorizedAccessException("You can only update tasks created by you.");
            }

            request.CreatedBy = context.StaffId;
        }

        var result = await _taskRepository.UpdateAsync(id, request, cancellationToken);
        if (result is null)
        {
            throw new KeyNotFoundException("Task not found.");
        }

        return Results.Ok(ApiResponse.Ok(result, "Update task successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IResult> Delete(long id, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);

        if (!context.CanManage)
        {
            var existed = await _taskRepository.GetByIdAsync(id, cancellationToken);
            if (existed is null)
            {
                throw new KeyNotFoundException("Task not found.");
            }

            if (existed.CreatedBy != context.StaffId)
            {
                throw new UnauthorizedAccessException("You can only delete tasks created by you.");
            }
        }

        await _taskRepository.DeleteAsync(id, cancellationToken);
        return Results.Ok(ApiResponse.Ok(null, "Delete task successfully."));
    }

    private async Task<(bool CanManage, long StaffId)> GetAccessContextAsync(CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value;
        if (role == "1")
        {
            return (true, 0);
        }

        if (role != "2")
        {
            throw new UnauthorizedAccessException("Only staff can manage tasks.");
        }

        var userId = GetCurrentUserId();
        var staff = await _dbContext.Staff
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new { x.Id, x.Department })
            .FirstOrDefaultAsync(cancellationToken);

        if (staff is null)
        {
            throw new UnauthorizedAccessException("Staff profile not found.");
        }

        if (staff.Department == 4)
        {
            return (true, staff.Id);
        }

        if (staff.Department == 1)
        {
            return (false, staff.Id);
        }

        throw new UnauthorizedAccessException("Only sales/management can manage tasks.");
    }

    private long GetCurrentUserId()
    {
        var subject = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!long.TryParse(subject, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid access token.");
        }

        return userId;
    }
}
