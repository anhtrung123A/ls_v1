using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.DTOs.ClassAttendances;
using app.Repositories.ClassAttendances;

namespace app.Controllers;

[ApiController]
[Route("api/class_attendances")]
[Authorize]
public class ClassAttendancesController : ControllerBase
{
    private readonly IClassAttendanceRepository _repo;
    private readonly AppDbContext _db;

    public ClassAttendancesController(IClassAttendanceRepository repo, AppDbContext db)
    {
        _repo = repo;
        _db = db;
    }

    [HttpGet]
    public async Task<IResult> List([FromQuery] ClassAttendanceListQuery query, CancellationToken cancellationToken)
    {
        await EnsureClassAttendancePermissionAsync(cancellationToken);
        var result = await _repo.GetAllAsync(query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Get class attendances successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IResult> Detail(long id, CancellationToken cancellationToken)
    {
        await EnsureClassAttendancePermissionAsync(cancellationToken);
        var result = await _repo.GetByIdAsync(id, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Class attendance not found.");
        return Results.Ok(ApiResponse.Ok(result, "Get class attendance successfully."));
    }

    [HttpPost]
    public async Task<IResult> BulkCreate([FromBody] List<ClassAttendanceRequest> requests, CancellationToken cancellationToken)
    {
        var userId = await EnsureClassAttendancePermissionAsync(cancellationToken);
        var result = await _repo.BulkCreateAsync(requests, userId, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Create class attendances successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IResult> Update(long id, [FromBody] ClassAttendanceRequest request, CancellationToken cancellationToken)
    {
        var userId = await EnsureClassAttendancePermissionAsync(cancellationToken);
        var result = await _repo.UpdateAsync(id, request, userId, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Class attendance not found.");
        return Results.Ok(ApiResponse.Ok(result, "Update class attendance successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IResult> Delete(long id, CancellationToken cancellationToken)
    {
        await EnsureClassAttendancePermissionAsync(cancellationToken);
        await _repo.DeleteAsync(id, cancellationToken);
        return Results.Ok(ApiResponse.Ok(null, "Delete class attendance successfully."));
    }

    private async Task<long> EnsureClassAttendancePermissionAsync(CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value;
        if (role == "1") return GetCurrentUserId();
        if (role != "2") throw new UnauthorizedAccessException("Only operator, manager or admin can manage class attendances.");

        var userId = GetCurrentUserId();
        var staff = await _db.Staff.AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new { x.Department })
            .FirstOrDefaultAsync(cancellationToken);

        if (staff is null || (staff.Department != 3 && staff.Department != 4))
        {
            throw new UnauthorizedAccessException("Only operator, manager or admin can manage class attendances.");
        }

        return userId;
    }

    private long GetCurrentUserId()
    {
        var subject = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(subject, out var userId)) throw new UnauthorizedAccessException("Invalid access token.");
        return userId;
    }
}
