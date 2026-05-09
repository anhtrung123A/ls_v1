using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.DTOs.Enrollments;
using app.Repositories.Enrollments;

namespace app.Controllers;

[ApiController]
[Route("api/enrollments")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentRepository _repo;
    private readonly AppDbContext _db;

    public EnrollmentsController(IEnrollmentRepository repo, AppDbContext db) { _repo = repo; _db = db; }

    [HttpGet]
    public async Task<IResult> List([FromQuery] EnrollmentListQuery query, CancellationToken cancellationToken)
    {
        await EnsureEnrollmentPermissionAsync(cancellationToken);
        var result = await _repo.GetAllAsync(query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Get enrollments successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IResult> Detail(long id, CancellationToken cancellationToken)
    {
        await EnsureEnrollmentPermissionAsync(cancellationToken);
        var result = await _repo.GetByIdAsync(id, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Enrollment not found.");
        return Results.Ok(ApiResponse.Ok(result, "Get enrollment successfully."));
    }

    [HttpPost]
    public async Task<IResult> Create([FromBody] CreateEnrollmentRequest request, CancellationToken cancellationToken)
    {
        var staffId = await EnsureEnrollmentPermissionAsync(cancellationToken);
        var result = await _repo.CreateAsync(new EnrollmentRequest
        {
            StudentId = request.StudentId,
            ClassId = request.ClassId,
            TuitionFee = request.TuitionFee,
            Discount = request.Discount,
            DiscountReason = request.DiscountReason,
            FinalFee = request.FinalFee,
            CompletedAt = request.CompletedAt,
            Notes = request.Notes
        }, staffId, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Create enrollment successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IResult> Update(long id, [FromBody] EnrollmentRequest request, CancellationToken cancellationToken)
    {
        await EnsureEnrollmentPermissionAsync(cancellationToken);
        var result = await _repo.UpdateAsync(id, request, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Enrollment not found.");
        return Results.Ok(ApiResponse.Ok(result, "Update enrollment successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IResult> Delete(long id, CancellationToken cancellationToken)
    {
        await EnsureEnrollmentPermissionAsync(cancellationToken);
        await _repo.DeleteAsync(id, cancellationToken);
        return Results.Ok(ApiResponse.Ok(null, "Delete enrollment successfully."));
    }

    private async Task<long> EnsureEnrollmentPermissionAsync(CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value;
        if (role != "2") throw new UnauthorizedAccessException("Only sales or academic staff can manage enrollments.");

        var userId = GetCurrentUserId();
        var staff = await _db.Staff.AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new { x.Id, x.Department })
            .FirstOrDefaultAsync(cancellationToken);

        if (staff is null || (staff.Department != 1 && staff.Department != 2))
        {
            throw new UnauthorizedAccessException("Only sales or academic staff can manage enrollments.");
        }

        return staff.Id;
    }

    private long GetCurrentUserId()
    {
        var subject = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(subject, out var userId)) throw new UnauthorizedAccessException("Invalid access token.");
        return userId;
    }
}
