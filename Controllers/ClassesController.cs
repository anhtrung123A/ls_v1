using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.DTOs.Classes;
using app.Repositories.Classes;

namespace app.Controllers;

[ApiController]
[Route("api/classes")]
[Authorize]
public class ClassesController : ControllerBase
{
    private readonly IClassRepository _repo;
    private readonly AppDbContext _db;

    public ClassesController(IClassRepository repo, AppDbContext db) { _repo = repo; _db = db; }

    [HttpGet]
    public async Task<IResult> List([FromQuery] ClassListQuery query, CancellationToken cancellationToken)
    {
        await EnsureClassPermissionAsync(cancellationToken);
        var result = await _repo.GetAllAsync(query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Get classes successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IResult> Detail(long id, CancellationToken cancellationToken)
    {
        await EnsureClassDetailPermissionAsync(id, cancellationToken);
        var result = await _repo.GetByIdAsync(id, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Class not found.");
        return Results.Ok(ApiResponse.Ok(result, "Get class successfully."));
    }

    [HttpPost]
    public async Task<IResult> Create([FromBody] CreateClassRequest request, CancellationToken cancellationToken)
    {
        await EnsureClassPermissionAsync(cancellationToken);
        var result = await _repo.CreateAsync(new ClassRequest
        {
            CourseId = request.CourseId,
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            MaxStudents = request.MaxStudents,
            CurrentCount = request.CurrentCount,
            Type = request.Type,
            TeacherId = request.TeacherId,
            CreatedBy = GetCurrentUserId()
        }, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Create class successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IResult> Update(long id, [FromBody] ClassRequest request, CancellationToken cancellationToken)
    {
        await EnsureClassPermissionAsync(cancellationToken);
        var result = await _repo.UpdateAsync(id, request, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Class not found.");
        return Results.Ok(ApiResponse.Ok(result, "Update class successfully."));
    }

    [HttpPost("{id:long}/set_schedule_created")]
    public async Task<IResult> SetScheduleCreated(long id, CancellationToken cancellationToken)
    {
        await EnsureClassPermissionAsync(cancellationToken);
        var result = await _repo.SetScheduleCreatedAsync(id, GetCurrentUserId(), cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Set class status schedule_created and generate class sessions successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IResult> Delete(long id, CancellationToken cancellationToken)
    {
        await EnsureClassPermissionAsync(cancellationToken);
        await _repo.DeleteAsync(id, cancellationToken);
        return Results.Ok(ApiResponse.Ok(null, "Delete class successfully."));
    }

    private async Task EnsureClassPermissionAsync(CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value;
        if (role == "1") return;
        if (role != "2") throw new UnauthorizedAccessException("Only admin, academic or operation can manage classes.");

        var dep = await GetDepartmentAsync(cancellationToken);
        if (dep != 2 && dep != 3) throw new UnauthorizedAccessException("Only admin, academic or operation can manage classes.");
    }

    private async Task EnsureClassDetailPermissionAsync(long classId, CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value;
        if (role == "1")
        {
            return;
        }

        var userId = GetCurrentUserId();

        if (role == "2")
        {
            var dep = await _db.Staff
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => x.Department)
                .FirstOrDefaultAsync(cancellationToken);

            if (dep == 2 || dep == 3)
            {
                return;
            }

            throw new UnauthorizedAccessException("You do not have permission to view this class.");
        }

        if (role == "3")
        {
            var canView = await _db.Classes
                .AsNoTracking()
                .AnyAsync(x => x.Id == classId && x.TeacherId == userId, cancellationToken);

            if (!canView) throw new UnauthorizedAccessException("Teacher can only view their own classes.");
            return;
        }

        if (role == "4")
        {
            var studentId = await _db.Students
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => (long?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (!studentId.HasValue)
            {
                throw new UnauthorizedAccessException("Student profile not found.");
            }

            var canView = await _db.ClassStudents
                .AsNoTracking()
                .AnyAsync(x => x.ClassId == classId && x.StudentId == studentId.Value, cancellationToken);

            if (!canView) throw new UnauthorizedAccessException("Student can only view classes they joined.");
            return;
        }

        throw new UnauthorizedAccessException("You do not have permission to view this class.");
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
