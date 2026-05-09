using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.DTOs.ClassSchedules;
using app.Repositories.ClassSchedules;

namespace app.Controllers;

[ApiController]
[Route("api/class_schedules")]
[Authorize]
public class ClassSchedulesController : ControllerBase
{
    private readonly IClassScheduleRepository _repo;
    private readonly AppDbContext _db;

    public ClassSchedulesController(IClassScheduleRepository repo, AppDbContext db) { _repo = repo; _db = db; }

    [HttpGet]
    public async Task<IResult> List([FromQuery] ClassScheduleListQuery query, CancellationToken cancellationToken)
    {
        await EnsureSchedulePermissionAsync(cancellationToken);
        var result = await _repo.GetAllAsync(query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Get class schedules successfully."));
    }

    [HttpGet("class/{class_id:long}")]
    public async Task<IResult> ListByClass(long class_id, [FromQuery] ClassScheduleListQuery query, CancellationToken cancellationToken)
    {
        await EnsureSchedulePermissionAsync(cancellationToken);
        var result = await _repo.GetByClassIdAsync(class_id, query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Get class schedules by class successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IResult> Detail(long id, CancellationToken cancellationToken)
    {
        await EnsureSchedulePermissionAsync(cancellationToken);
        var result = await _repo.GetByIdAsync(id, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Class schedule not found.");
        return Results.Ok(ApiResponse.Ok(result, "Get class schedule successfully."));
    }

    [HttpPost]
    public async Task<IResult> Create([FromBody] CreateClassScheduleRequest request, CancellationToken cancellationToken)
    {
        await EnsureSchedulePermissionAsync(cancellationToken);
        var result = await _repo.CreateAsync(new ClassScheduleRequest
        {
            ClassId = request.ClassId,
            RoomId = request.RoomId,
            Weekday = request.Weekday,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        }, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Create class schedule successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IResult> Update(long id, [FromBody] ClassScheduleRequest request, CancellationToken cancellationToken)
    {
        await EnsureSchedulePermissionAsync(cancellationToken);
        var result = await _repo.UpdateAsync(id, request, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Class schedule not found.");
        return Results.Ok(ApiResponse.Ok(result, "Update class schedule successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IResult> Delete(long id, CancellationToken cancellationToken)
    {
        await EnsureSchedulePermissionAsync(cancellationToken);
        await _repo.DeleteAsync(id, cancellationToken);
        return Results.Ok(ApiResponse.Ok(null, "Delete class schedule successfully."));
    }

    private async Task EnsureSchedulePermissionAsync(CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value;
        if (role == "1") return;
        if (role != "2") throw new UnauthorizedAccessException("Only admin, academic or operation can manage class schedules.");

        var dep = await GetDepartmentAsync(cancellationToken);
        if (dep != 2 && dep != 3) throw new UnauthorizedAccessException("Only admin, academic or operation can manage class schedules.");
    }

    private async Task<byte?> GetDepartmentAsync(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        return await _db.Staff.AsNoTracking().Where(x => x.UserId == userId).Select(x => x.Department).FirstOrDefaultAsync(cancellationToken);
    }

    private long GetCurrentUserId()
    {
        var subject = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(subject, out var userId)) throw new UnauthorizedAccessException("Invalid access token.");
        return userId;
    }
}
