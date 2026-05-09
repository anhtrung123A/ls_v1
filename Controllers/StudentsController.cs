using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.DTOs.Students;
using app.Repositories.Students;

namespace app.Controllers;

[ApiController]
[Route("api/students")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentRepository _studentRepository;
    private readonly AppDbContext _dbContext;

    public StudentsController(IStudentRepository studentRepository, AppDbContext dbContext)
    {
        _studentRepository = studentRepository;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IResult> GetAll([FromQuery] StudentListQuery query, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);
        if (!context.CanManage && !context.CanViewOwnAssigned)
        {
            throw new UnauthorizedAccessException("You do not have permission to view students.");
        }

        if (context.CanViewOwnAssigned)
        {
            query.AssignedStaffId = context.StaffId;
        }

        var result = await _studentRepository.GetAllAsync(query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Get students successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IResult> GetById(long id, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);
        var result = await _studentRepository.GetByIdAsync(id, cancellationToken);
        if (result is null)
        {
            throw new KeyNotFoundException("Student not found.");
        }

        if (context.CanViewOwnAssigned && result.AssignedStaffId != context.StaffId)
        {
            throw new UnauthorizedAccessException("You can only view students assigned to you.");
        }

        if (!context.CanManage && !context.CanViewOwnAssigned)
        {
            throw new UnauthorizedAccessException("You do not have permission to view this student.");
        }

        return Results.Ok(ApiResponse.Ok(result, "Get student successfully."));
    }

    [HttpPost]
    public async Task<IResult> Create([FromBody] CreateStudentRequest request, CancellationToken cancellationToken)
    {
        await EnsureManagePermissionAsync(cancellationToken);
        var result = await _studentRepository.CreateAsync(request, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Create student successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IResult> Update(long id, [FromBody] UpdateStudentRequest request, CancellationToken cancellationToken)
    {
        await EnsureManagePermissionAsync(cancellationToken);
        var result = await _studentRepository.UpdateAsync(id, request, cancellationToken);
        if (result is null)
        {
            throw new KeyNotFoundException("Student not found.");
        }

        return Results.Ok(ApiResponse.Ok(result, "Update student successfully."));
    }

    [HttpPost("{id:long}/create_user")]
    public async Task<IResult> CreateUser(long id, CancellationToken cancellationToken)
    {
        await EnsureManagePermissionAsync(cancellationToken);
        var result = await _studentRepository.CreateUserAsync(id, cancellationToken);
        if (result is null)
        {
            throw new KeyNotFoundException("Student not found.");
        }

        return Results.Ok(ApiResponse.Ok(result, "Create user successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IResult> Delete(long id, CancellationToken cancellationToken)
    {
        await EnsureManagePermissionAsync(cancellationToken);
        await _studentRepository.DeleteAsync(id, cancellationToken);
        return Results.Ok(ApiResponse.Ok(null, "Delete student successfully."));
    }

    private async Task EnsureManagePermissionAsync(CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);
        if (!context.CanManage)
        {
            throw new UnauthorizedAccessException("Only admin or management staff can manage students.");
        }
    }

    private async Task<(bool CanManage, bool CanViewOwnAssigned, long? StaffId)> GetAccessContextAsync(CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value;
        if (role == "1")
        {
            return (true, false, null);
        }

        if (role != "2")
        {
            return (false, false, null);
        }

        var staffIdClaim = User.FindFirst("staff_id")?.Value;
        var departmentClaim = User.FindFirst("department")?.Value;

        if (long.TryParse(staffIdClaim, out var claimStaffId) && byte.TryParse(departmentClaim, out var claimDepartment))
        {
            return claimDepartment switch
            {
                4 => (true, false, claimStaffId),
                1 => (false, true, claimStaffId),
                _ => (false, false, claimStaffId)
            };
        }

        var userId = GetCurrentUserId();
        var staff = await _dbContext.Staff
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .Select(s => new { s.Id, s.Department })
            .FirstOrDefaultAsync(cancellationToken);

        if (staff is null)
        {
            return (false, false, null);
        }

        return staff.Department switch
        {
            4 => (true, false, staff.Id),
            1 => (false, true, staff.Id),
            _ => (false, false, staff.Id)
        };
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
