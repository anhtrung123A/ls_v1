using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.DTOs.Courses;
using app.Repositories.Courses;

namespace app.Controllers;

[ApiController]
[Route("api/courses")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseRepository _repo;
    private readonly AppDbContext _db;

    public CoursesController(ICourseRepository repo, AppDbContext db) { _repo = repo; _db = db; }

    [HttpGet]
    public async Task<IResult> List([FromQuery] CourseListQuery query, CancellationToken cancellationToken)
    {
        await EnsureCoursePermissionAsync(cancellationToken);
        var result = await _repo.GetAllAsync(query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Get courses successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IResult> Detail(long id, CancellationToken cancellationToken)
    {
        await EnsureCoursePermissionAsync(cancellationToken);
        var result = await _repo.GetByIdAsync(id, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Course not found.");
        return Results.Ok(ApiResponse.Ok(result, "Get course successfully."));
    }

    [HttpPost]
    public async Task<IResult> Create([FromBody] CreateCourseRequest request, CancellationToken cancellationToken)
    {
        await EnsureCoursePermissionAsync(cancellationToken);
        var result = await _repo.CreateAsync(new CourseRequest
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            Level = request.Level,
            TotalSessions = request.TotalSessions,
            DurationMinutes = request.DurationMinutes,
            Price = request.Price,
            Currency = request.Currency,
            ThumbnailUrl = request.ThumbnailUrl,
            CreatedBy = GetCurrentUserId()
        }, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Create course successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IResult> Update(long id, [FromBody] CourseRequest request, CancellationToken cancellationToken)
    {
        await EnsureCoursePermissionAsync(cancellationToken);
        var result = await _repo.UpdateAsync(id, request, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Course not found.");
        return Results.Ok(ApiResponse.Ok(result, "Update course successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IResult> Delete(long id, CancellationToken cancellationToken)
    {
        await EnsureCoursePermissionAsync(cancellationToken);
        await _repo.DeleteAsync(id, cancellationToken);
        return Results.Ok(ApiResponse.Ok(null, "Delete course successfully."));
    }

    private async Task EnsureCoursePermissionAsync(CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value;
        if (role == "1") return;
        if (role != "2") throw new UnauthorizedAccessException("Only admin or academic manager can manage courses.");

        var dep = await GetDepartmentAsync(cancellationToken);
        if (dep != 2) throw new UnauthorizedAccessException("Only admin or academic manager can manage courses.");
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
