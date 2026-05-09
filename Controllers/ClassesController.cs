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
        await EnsureClassPermissionAsync(cancellationToken);
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
